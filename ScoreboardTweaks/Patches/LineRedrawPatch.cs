using HarmonyLib;
using System;
using TMPro;
using UnityEngine;

namespace ScoreboardTweaks.Patches
{
    [HarmonyPatch(typeof(GorillaScoreBoard), nameof(GorillaScoreBoard.RedrawPlayerLines))]
    [HarmonyPriority(Priority.VeryHigh)]
    internal class LineRedrawPatch
    {
        public static bool Prefix(GorillaScoreBoard __instance)
        {
            string beginningString = __instance.GetBeginningString();
            if (beginningString.Contains('\n')) beginningString = beginningString.Split('\n', StringSplitOptions.RemoveEmptyEntries)[0];

            __instance.stringBuilder.Clear();
            __instance.stringBuilder.AppendLine(beginningString);
            __instance.stringBuilder.Append((GorillaScoreboardTotalUpdater.instance?.playersInRoom?.Count ?? 0) > 1 ? "  PLAYER STATUS              REPORT" : "  PLAYER STATUS");
            __instance.buttonStringBuilder.Clear();

            bool isFeatureEnabled = KIDManager.CheckFeatureSettingEnabled(EKIDFeatures.Custom_Nametags);

            for (int i = 0; i < __instance.lines.Count; i++)
            {
                try
                {
                    GorillaPlayerScoreboardLine line = __instance.lines[i];

                    if (line.gameObject.activeInHierarchy)
                    {
                        line.GetComponent<RectTransform>().localPosition = new Vector3(0f, __instance.startingYValue - __instance.lineHeight * i, 0f);

                        if (Main.m_lineTextOverride[line].TryGetValue("PlayerText", out TMP_Text playerText))
                        {
                            playerText.text = isFeatureEnabled ? line.playerNameVisible : line.linePlayer.DefaultName;
                            playerText.color = line.playerVRRig?.playerText1?.color ?? Color.white;
                        }
                    }
                }
                catch
                {

                }
            }

            __instance.boardText.text = __instance.stringBuilder.ToString();
            __instance.buttonText.text = __instance.buttonStringBuilder.ToString();
            __instance._isDirty = false;

            return false;
        }
    }
}
