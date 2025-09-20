using HarmonyLib;
using UnityEngine;

namespace ScoreboardTweaks.Patches
{
    [HarmonyPatch(typeof(GorillaPlayerLineButton), nameof(GorillaPlayerLineButton.OnTriggerEnter))]
    internal class LineButtonTriggerPatch
    {
        internal static float m_flNextPress = 0.0f;

        private static bool Prefix(GorillaPlayerLineButton __instance, Collider collider)
        {
            if (!__instance.enabled || m_flNextPress > Time.realtimeSinceStartup || __instance.touchTime + __instance.debounceTime >= Time.realtimeSinceStartup) return false;
           
            if (collider.GetComponent<GorillaTriggerColliderHandIndicator>())
                m_flNextPress = Time.realtimeSinceStartup + 0.125f;

            return true;
        }
    }
}
