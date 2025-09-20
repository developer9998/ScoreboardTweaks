using HarmonyLib;
using UnityEngine;

namespace ScoreboardTweaks.Patches
{
    [HarmonyPatch(typeof(GorillaPlayerScoreboardLine), nameof(GorillaPlayerScoreboardLine.UpdateLine))]
    internal class LineUpdatePatch
    {
        public static void Postfix(GorillaPlayerScoreboardLine __instance)
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
