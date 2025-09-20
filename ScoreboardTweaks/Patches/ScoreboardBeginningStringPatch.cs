using HarmonyLib;

namespace ScoreboardTweaks.Patches
{
    [HarmonyPatch(typeof(GorillaScoreBoard), nameof(GorillaScoreBoard.GetBeginningString))]
    [HarmonyPriority(Priority.LowerThanNormal)]
    internal class ScoreboardBeginningStringPatch
    {
        public static bool Prefix(ref string __result)
        {
            __result = $"ROOM ID: {(NetworkSystem.Instance.SessionIsPrivate ? "-PRIVATE-" : NetworkSystem.Instance.RoomName)}";
            return false;
        }
    }
}
