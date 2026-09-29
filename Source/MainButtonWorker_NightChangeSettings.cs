using RimWorld;
using Verse;

namespace NightChange
{
    // Keep visibility under MainButtonDef control so customization mods can reveal the button.
    public class MainButtonWorker_NightChangeSettings : MainButtonWorker
    {
        public override void Activate()
        {
            Find.WindowStack.Add(new Dialog_ModSettings(NightChangeMod.Instance));
        }
    }
}
