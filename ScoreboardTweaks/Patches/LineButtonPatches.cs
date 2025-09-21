using HarmonyLib;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ScoreboardTweaks.Patches
{
    [HarmonyPatch(typeof(GorillaPlayerLineButton))]
    internal class LineButtonPatches
    {
        internal static float m_flNextPress = 0.0f;

        [HarmonyPatch(nameof(GorillaPlayerLineButton.OnTriggerEnter))]
        [HarmonyPrefix]
        public static bool ButtonTriggerPatch(GorillaPlayerLineButton __instance, Collider collider)
        {
            if (!__instance.enabled || m_flNextPress > Time.realtimeSinceStartup || __instance.touchTime + __instance.debounceTime >= Time.realtimeSinceStartup) return false;

            if (collider.GetComponent<GorillaTriggerColliderHandIndicator>())
                m_flNextPress = Time.realtimeSinceStartup + Constants.Scoreboard_GlobalButtonDebounce;

            return true;
        }

        [HarmonyPatch(nameof(GorillaPlayerLineButton.UpdateColor))]
        [HarmonyPrefix]
        public static void ButtonColourUpdatePatch(GorillaPlayerLineButton __instance)
        {
            GorillaPlayerScoreboardLine parentLine = __instance.parentLine;
            if (Main.m_lineTextOverride.TryGetValue(parentLine, out Dictionary<object, TMP_Text> overrideTextDictionary) && overrideTextDictionary.TryGetValue(__instance.buttonType, out TMP_Text buttonText))
            {
                string text = __instance.isOn ? __instance.onText : (__instance.isAutoOn ? __instance.autoOnText : __instance.offText);
                buttonText.text = text;
            }
        }
    }
}
