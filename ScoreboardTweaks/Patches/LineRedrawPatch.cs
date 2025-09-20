using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace ScoreboardTweaks.Patches
{
    [HarmonyPatch(typeof(GorillaScoreBoard), nameof(GorillaScoreBoard.RedrawPlayerLines))]
    [HarmonyPriority(Priority.VeryHigh)]
    internal class LineRedrawPatch
    {
        public static bool Prefix(GorillaScoreBoard __instance)
        {
            // string beginningString = __instance.GetBeginningString();
            // if (beginningString.Split('\n') is string[] beginningLines && beginningLines.Length > 0) beginningString = beginningLines[0];

            __instance.stringBuilder.Clear();
            __instance.stringBuilder.AppendLine($"ROOM ID: {(NetworkSystem.Instance.SessionIsPrivate ? "-PRIVATE-" : NetworkSystem.Instance.RoomName)}");
            // __instance.stringBuilder.AppendLine(beginningString);
            __instance.stringBuilder.Append("  PLAYER STATUS           REPORT");
            __instance.buttonStringBuilder.Clear();

            bool nametagsAllowed = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
            for (int i = 0; i < __instance.lines.Count; i++)
            {
                try
                {
                    if (__instance.lines[i].gameObject.activeInHierarchy)
                    {
                        __instance.lines[i].GetComponent<RectTransform>().localPosition = new Vector3(0f, (float)(__instance.startingYValue - __instance.lineHeight * i), 0f);

                        Text playerName = __instance.lines[i].playerName;
                        playerName?.text = nametagsAllowed ? __instance.lines[i].playerNameVisible : __instance.lines[i].linePlayer.DefaultName;
                        playerName?.color = __instance.lines[i].playerVRRig?.playerText1?.color ?? Color.white;
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
