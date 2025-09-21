using HarmonyLib;
using UnityEngine;

namespace ScoreboardTweaks.Patches
{
    [HarmonyPatch(typeof(GorillaPlayerScoreboardLine))]
    internal class LinePatches
    {
        [HarmonyPatch(nameof(GorillaPlayerScoreboardLine.PressButton))]
        [HarmonyPostfix]
        public static void LineButtonPressPatch(GorillaPlayerScoreboardLine __instance, GorillaPlayerLineButton.ButtonType buttonType)
        {
            if (buttonType == GorillaPlayerLineButton.ButtonType.Mute)
            {
                // Update speaker icon for player
                __instance.UpdateLine();
            }
        }

        [HarmonyPatch(nameof(GorillaPlayerScoreboardLine.UpdateLine))]
        [HarmonyPostfix]
        public static void LineUpdatePatch(GorillaPlayerScoreboardLine __instance)
        {
            SpriteRenderer speakerIcon = __instance.speakerIcon;
            GorillaPlayerLineButton muteButton = __instance.muteButton;

            if (muteButton?.isOn ?? false)
            {
                speakerIcon?.sprite = Main.m_spriteGizmoManualMuted;
                speakerIcon?.enabled = true;
                return;
            }

            if (muteButton?.isAutoOn ?? false)
            {
                speakerIcon?.sprite = Main.m_spriteGizmoAutoMuted;
                speakerIcon?.enabled = true;
                return;
            }

            speakerIcon?.sprite = Main.m_spriteGizmoOriginal;
        }
    }
}
