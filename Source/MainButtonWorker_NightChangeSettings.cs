using RimWorld;
using Verse;

namespace NightChange
{
    // Keep visibility under MainButtonDef control so customization mods can reveal the button.
    public class MainButtonWorker_NightChangeSettings : MainButtonWorker
    {
        public override void Activate()
        {
            Mod mod = NightChangeMod.Instance ?? LoadedModManager.GetMod<NightChangeMod>();
            if (mod == null)
            {
                Log.Warning("[Night Change] The settings shortcut was activated before the mod was constructed.");
                return;
            }

            Find.WindowStack.Add(new Dialog_ModSettings(mod));
        }
    }
}
