using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ScoreboardTweaks
{
    [BepInDependency("net.rusjj.gorillafriends", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(Constants.modGUID, Constants.modName, Constants.modVersion)]
    internal class Main : BaseUnityPlugin
    {
        public static new ManualLogSource Logger;

        public static HashSet<GorillaScoreBoard> m_listScoreboards = [];

        public static Sprite m_spriteGizmoMuted = null, m_spriteGizmoOriginal = null;

        public static string[] dependencyGUIDList;

        public void Awake()
        {
            Logger = base.Logger;

            Harmony.CreateAndPatchAll(GetType().Assembly, Constants.modGUID);

            IEnumerable<BepInDependency> attributes = GetType().GetCustomAttributes<BepInDependency>(false);
            dependencyGUIDList = [.. attributes.Select(attribute => attribute.DependencyGUID)];

            foreach (var plugin in Chainloader.PluginInfos.Values)
            {
                if (!dependencyGUIDList.Contains(plugin.Metadata.GUID)) continue;

                try
                {
                    foreach (MethodInfo method in AccessTools.GetDeclaredMethods(plugin.Instance.GetType()))
                    {
                        if (method.Name == "OnScoreboardTweakerStart")
                        {
                            Logger.LogMessage($"Called OnScoreboardTweakerStart for {plugin.Metadata.Name}/{plugin.Metadata.GUID}");
                            method.Invoke(plugin.Instance, []);
                            break;
                        }
                    }
                }
                catch
                {

                }
            }

            FileInfo file = new(Path.Combine(Path.GetDirectoryName(typeof(Main).Assembly.Location), "gizmo-speaker-muted.png"));
            if (!file.Exists)
            {
                return;
            }

            Texture2D texture = new(2, 2);
            texture.LoadImage(File.ReadAllBytes(file.FullName));

            m_spriteGizmoMuted = Sprite.Create(texture, new Rect(0.0f, 0.0f, 512.0f, 512.0f), new Vector2(0.5f, 0.5f), 100.0f);
            m_spriteGizmoMuted.name = "gizmo-speaker-muted";
        }
    }
}