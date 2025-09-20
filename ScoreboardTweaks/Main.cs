using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ScoreboardTweaks
{
    [BepInDependency("net.rusjj.gorillafriends", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(Constants.GUID, Constants.Name, Constants.Version)]
    internal class Main : BaseUnityPlugin
    {
        public static new ManualLogSource Logger;

        public static HashSet<GorillaScoreBoard> m_listScoreboards = [];

        public static Sprite m_spriteGizmoManualMuted = null, m_spriteGizmoAutoMuted = null, m_spriteGizmoOriginal = null;

        public static string[] dependencyGUIDList;

        public void Awake()
        {
            Logger = base.Logger;

            Harmony.CreateAndPatchAll(GetType().Assembly, Constants.GUID);

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

            ThreadingHelper.Instance.StartSyncInvoke(async () =>
            {
                m_spriteGizmoManualMuted = await FindSprite("gizmo-speaker-manual-mute.png");
                m_spriteGizmoAutoMuted = await FindSprite("gizmo-speaker-auto-mute.png");
            });
        }

        private async Task<Sprite> FindSprite(string fileName)
        {
            string path = Path.Combine(Path.GetDirectoryName(typeof(Main).Assembly.Location), fileName);
            FileInfo file = new(path);

            Texture2D texture;

            Logger.LogMessage(path);

            if (file.Exists)
            {
                texture = new(2, 2, TextureFormat.RGBA32, false);
                texture.LoadImage(await File.ReadAllBytesAsync(path));
            }
            else
            {
                string url = $"{Constants.RepositoryContentUrl}/{fileName}";
                Logger.LogMessage(url);

                UnityWebRequest webRequest = UnityWebRequest.Get(url);
                UnityWebRequestAsyncOperation asyncOperation = webRequest.SendWebRequest();
                await asyncOperation;

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Logger.LogFatal("Unsuccessful");
                    return null;
                }

                texture = new(2, 2, TextureFormat.RGBA32, false);
                byte[] bytes = webRequest.downloadHandler.data;
                texture.LoadImage(bytes);
                await File.WriteAllBytesAsync(path, bytes);
            }

            if (texture == null) return null;

            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            sprite.name = Path.GetFileNameWithoutExtension(path);

            return sprite;
        }
    }
}