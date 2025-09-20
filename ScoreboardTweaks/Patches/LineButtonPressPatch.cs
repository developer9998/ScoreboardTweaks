using HarmonyLib;

namespace ScoreboardTweaks.Patches
{
    [HarmonyPatch(typeof(GorillaPlayerScoreboardLine), nameof(GorillaPlayerScoreboardLine.PressButton))]
    internal class LineButtonPressPatch
    {
        public static void Postfix(GorillaPlayerScoreboardLine __instance, GorillaPlayerLineButton.ButtonType buttonType)
        {
            if (buttonType == GorillaPlayerLineButton.ButtonType.Mute)
            {
                // Update speaker icon for player
                __instance.UpdateLine();
            }
        }
    }
}
