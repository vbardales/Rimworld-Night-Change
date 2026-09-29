using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RimWorks.Pickle;
using RimWorld;
using UnityEngine;
using Verse;

namespace NightChange.PickleSteps
{
    /// <summary>
    /// The settings page, its hidden MainButtons shortcut, the settings file across a restart, and the
    /// translated text. RIMMSQOL itself is driven by PickleTools' RimmsqolSteps, not here: these steps
    /// move MainButtonDef.buttonVisible, which is all RIMMSQOL does to reveal a button, and then ask the
    /// game's own worker what the bar would do.
    /// </summary>
    [PickleSteps]
    public class SettingsSteps
    {
        private const string ShortcutDef = "NightChange_Settings";

        // ------------------------------------------------------------------ the dialog

        /// <summary>
        /// Waits for its own frames: Dialog_ModSettings force-pauses the game, so a tick wait in the
        /// scenario could never be satisfied.
        /// </summary>
        [When("Night Change: I open the settings page from Mod options")]
        public async Task OpenFromOptions(PickleContext ctx)
        {
            Mod mod = LoadedModManager.ModHandles.FirstOrDefault(m => m is NightChangeMod);
            ctx.Assert(mod != null, "Mod options does not list Night Change: its Mod handle is not loaded");
            ctx.Assert(!mod.SettingsCategory().NullOrEmpty(),
                "Night Change returns an empty SettingsCategory, so Mod options would not list it");
            Find.WindowStack.Add(new Dialog_ModSettings(mod));
            await ctx.WaitFrames(3);
        }

        [When("Night Change: the shortcut is activated")]
        public async Task ActivateShortcut(PickleContext ctx)
        {
            Shortcut(ctx).Worker.Activate();
            await ctx.WaitFrames(3);
        }

        /// <summary>
        /// The claim is not "a settings window opened" but "Night Change's settings opened": a dialog
        /// built for another mod looks the same in a capture, so the window is asked which mod it was
        /// built for.
        /// </summary>
        [Then("Night Change: the settings dialog is open for this mod")]
        public void DialogIsOpenForThisMod(PickleContext ctx)
        {
            List<Dialog_ModSettings> dialogs = Find.WindowStack.Windows.OfType<Dialog_ModSettings>().ToList();
            ctx.Assert(dialogs.Count > 0, "no Dialog_ModSettings is open");
            ctx.Assert(dialogs.Any(d => ModOf(ctx, d) is NightChangeMod),
                "a settings dialog is open, but not Night Change's: it was built for "
                + string.Join(", ", dialogs.Select(d => ModOf(ctx, d)?.Content?.Name ?? "an unknown mod").ToArray())
                + ". The shortcut and Mod options must lead to the same place");
        }

        private static Mod ModOf(PickleContext ctx, Dialog_ModSettings dialog)
        {
            var field = typeof(Dialog_ModSettings).GetFields(Driver.InstanceAny)
                .FirstOrDefault(f => typeof(Mod).IsAssignableFrom(f.FieldType));
            ctx.Require(field != null, "Dialog_ModSettings holds no Mod field in this version; it has: "
                + string.Join(", ", typeof(Dialog_ModSettings).GetFields(Driver.InstanceAny).Select(f => f.Name).ToArray()));
            return field.GetValue(dialog) as Mod;
        }

        // ------------------------------------------------------------------ the defaults

        [Then("Night Change: the settings are at their documented defaults")]
        public void AtDefaults(PickleContext ctx)
        {
            NightChangeSettings s = NightChangeMod.Settings;
            ctx.Assert(s.inheritOwnerFromBed && s.coldGuard && Math.Abs(s.coldGuardMargin - 2f) < 0.01f && s.maxStandDistance == 12,
                $"the settings read inherit={s.inheritOwnerFromBed}, cold guard={s.coldGuard}, margin={s.coldGuardMargin}, "
                + $"distance={s.maxStandDistance}; the documented defaults are true, true, 2 and 12");
        }

        // ------------------------------------------------------------------ the shortcut

        private static MainButtonDef Shortcut(PickleContext ctx)
        {
            MainButtonDef def = DefDatabase<MainButtonDef>.GetNamedSilentFail(ShortcutDef);
            ctx.Require(def != null, $"no MainButtonDef named '{ShortcutDef}': the shortcut RIMMSQOL is meant to reveal is not shipped");
            return def;
        }

        [Then("Night Change: the shortcut is hidden on a clean configuration")]
        public void HiddenByDefault(PickleContext ctx)
        {
            MainButtonDef def = Shortcut(ctx);
            ctx.Assert(!def.buttonVisible, "the shortcut ships with buttonVisible true: it would stand in every main bar unasked");
            NotDrawn(ctx);
        }

        private sealed class Revealed { }

        [When("Night Change: the shortcut is revealed, as a customization mod would")]
        public void Reveal(PickleContext ctx)
        {
            ctx.Set(new Revealed());
            Shortcut(ctx).buttonVisible = true;
        }

        [When("Night Change: the shortcut is hidden again")]
        public void Hide(PickleContext ctx) => Shortcut(ctx).buttonVisible = false;

        /// <summary>
        /// Both halves: Worker.Visible decides whether the bar draws the def at all, Worker.Disabled
        /// whether it draws it greyed. MOD_SETTINGS.md forbids a greyed shortcut as firmly as a visible one.
        /// </summary>
        [Then("Night Change: the shortcut is drawn in the bar")]
        public void Drawn(PickleContext ctx)
        {
            MainButtonDef def = Shortcut(ctx);
            ctx.Assert(def.Worker.Visible, "the shortcut is revealed and its worker still reports Visible false");
            ctx.Assert(!def.Worker.Disabled, "the shortcut is drawn but greyed out, which MOD_SETTINGS.md forbids");
        }

        [Then("Night Change: the shortcut is not drawn in the bar")]
        public void NotDrawn(PickleContext ctx)
        {
            MainButtonDef def = Shortcut(ctx);
            ctx.Assert(!def.Worker.Visible,
                $"the shortcut reports Visible true with buttonVisible {def.buttonVisible}: it shows without anything having revealed it");
        }

        [AfterScenario]
        public void PutShortcutBack(PickleContext ctx)
        {
            try { ctx.Get<Revealed>(); }
            catch (Exception) { return; }
            MainButtonDef def = DefDatabase<MainButtonDef>.GetNamedSilentFail(ShortcutDef);
            if (def != null) def.buttonVisible = false;
        }

        // ------------------------------------------------------------------ the settings file across a restart

        private static bool keptForNextLaunch;
        private static bool ownsKeptSettings;

        /// <summary>
        /// Launch 1 of 2. Leaves non-default values in the profile's settings file ON PURPOSE: the read
        /// launch proves the file is what the next process loads at startup. It is one step so a
        /// scenario that fails before it leaves nothing behind.
        /// </summary>
        [Given("Night Change: the settings are set to non-default values and kept for the next launch")]
        public void KeepForNextLaunch(PickleContext ctx)
        {
            NightChangeSettings s = NightChangeMod.Settings;
            s.inheritOwnerFromBed = false;
            s.coldGuard = false;
            s.coldGuardMargin = 7f;
            s.maxStandDistance = 30;
            NightChangeMod.Instance.WriteSettings();
            keptForNextLaunch = true;
        }

        /// <summary>
        /// Launch 2 of 2. Refuses to pass when the writer ran in THIS process: that would be a restart
        /// test that never restarted, with the values still in memory.
        /// </summary>
        [Then("Night Change: the settings kept by the previous launch are in place")]
        public void PreviousLaunchKept(PickleContext ctx)
        {
            ctx.Require(!keptForNextLaunch, "the writer ran in this process: this is not a restart, the values are still in memory");
            ownsKeptSettings = true;
            NightChangeSettings s = NightChangeMod.Settings;
            ctx.Assert(!s.inheritOwnerFromBed && !s.coldGuard && Math.Abs(s.coldGuardMargin - 7f) < 0.01f && s.maxStandDistance == 30,
                $"the settings read inherit={s.inheritOwnerFromBed}, cold guard={s.coldGuard}, margin={s.coldGuardMargin}, "
                + $"distance={s.maxStandDistance}; the previous launch kept false, false, 7 and 30");
        }

        [AfterScenario]
        public void PutFileBack(PickleContext ctx)
        {
            if (!ownsKeptSettings) return;
            ownsKeptSettings = false;
            NightChangeSettings s = NightChangeMod.Settings;
            s.inheritOwnerFromBed = true;
            s.coldGuard = true;
            s.coldGuardMargin = 2f;
            s.maxStandDistance = 12;
            NightChangeMod.Instance.WriteSettings();
        }

        // ------------------------------------------------------------------ the text

        /// <summary>
        /// Every key the English file declares resolves in the language of the pass. In developer mode a
        /// key missing from the active language shows as accented gibberish; this asks the language itself.
        /// </summary>
        [Then("Night Change: all {int} keys of the mod resolve in the language of the pass")]
        public void KeysResolve(PickleContext ctx, int expected)
        {
            LoadedLanguage english = LanguageDatabase.AllLoadedLanguages.FirstOrDefault(l => l.folderName == "English");
            ctx.Require(english != null, "the English language is not loaded");
            List<string> keys = english.keyedReplacements.Keys.Where(k => k.StartsWith("NightChange_")).ToList();
            ctx.Assert(keys.Count == expected, $"the English file declares {keys.Count} NightChange keys, expected {expected}");
            List<string> missing = keys.Where(k => !LanguageDatabase.activeLanguage.HaveTextForKey(k)).ToList();
            ctx.Assert(missing.Count == 0,
                $"in {Driver.Language(ctx)}, {missing.Count} key(s) do not resolve: {string.Join(", ", missing.ToArray())}");
        }

        [Then("Night Change: the stand offers the gizmo keyed {string}")]
        public void OffersGizmo(PickleContext ctx, string key)
        {
            string label = key.Translate();
            List<string> labels = Driver.Stand(ctx).GetGizmos().OfType<Command>().Select(c => c.defaultLabel).ToList();
            ctx.Assert(labels.Contains(label),
                $"the stand offers no gizmo labelled \"{label}\" (key {key}); it offers: {string.Join(", ", labels.ToArray())}");
        }

        [Then("Night Change: the stand's inspect line names {string}")]
        public void InspectLine(PickleContext ctx, string name)
        {
            string expected = "NightChange_InspectInUse".Translate(name);
            string actual = Driver.Comp(ctx).CompInspectStringExtra();
            ctx.Assert(actual == expected, $"the inspect line reads \"{actual}\", expected \"{expected}\"");
        }
    }
}
