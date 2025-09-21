using HarmonyLib;
using System.Collections.Generic;
using TMPro;

namespace ScoreboardTweaks.Patches
{
    [HarmonyPatch(typeof(GorillaPlayerLineButton), nameof(GorillaPlayerLineButton.UpdateColor))]
    internal class LineButtonUpdatePatch
    {
        public static void Prefix(GorillaPlayerLineButton __instance)
        {
            GorillaPlayerScoreboardLine line = __instance.parentLine;
            if (Main.m_lineTextOverride.TryGetValue(line, out Dictionary<object, TMP_Text> overrideTextDictionary) && overrideTextDictionary.TryGetValue(__instance.buttonType, out TMP_Text buttonText))
            {
                string text = __instance.isOn ? __instance.onText : (__instance.isAutoOn ? __instance.autoOnText : __instance.offText);
                buttonText.text = text;
            }
        }
    }
}
