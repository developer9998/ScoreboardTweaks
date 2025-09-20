using BepInEx.Bootstrap;
using GorillaExtensions;
using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace ScoreboardTweaks.Patches
{
    [HarmonyPatch(typeof(GorillaScoreBoard), nameof(GorillaScoreBoard.Start))]
    [HarmonyPriority(Priority.VeryHigh)]
    internal class ScoreboardAwakePatch
    {
        public static void Prefix(GorillaScoreBoard __instance)
        {
            if (!Main.m_listScoreboards.Add(__instance)) return;

            Main.Logger.LogMessage($"Added GorillaScoreBoard: {__instance.transform.GetPath()}");

            foreach (var plugin in Chainloader.PluginInfos.Values)
            {
                if (!Main.dependencyGUIDList?.Contains(plugin.Metadata.GUID) ?? false) continue;

                try
                {
                    foreach (MethodInfo method in AccessTools.GetDeclaredMethods(plugin.Instance.GetType()))
                    {
                        if (method.Name == "OnScoreboardTweakerProcessedPre")
                        {
                            Main.Logger.LogMessage($"Called OnScoreboardTweakerProcessedPre for {plugin.Metadata.Name}/{plugin.Metadata.GUID}");
                            method.Invoke(plugin.Instance, [__instance]);
                            break;
                        }
                    }
                }
                catch
                {

                }
            }

            __instance.boardText.margin = new Vector4(4f, 2f);
            __instance.boardText.lineSpacing = 60f;

            if (Main.m_spriteGizmoOriginal == null && __instance.scoreBoardLinePrefab is GameObject prefab && prefab)
            {
                Transform child = prefab.transform.Find("gizmo-speaker");
                if (child != null && child.TryGetComponent(out SpriteRenderer spriteRenderer)) Main.m_spriteGizmoOriginal = spriteRenderer.sprite;
            }

            Text text;
            GorillaPlayerLineButton lineButton;

            foreach (GorillaPlayerScoreboardLine line in __instance.lines)
            {
                // Main.Logger.LogMessage($"{line.transform.name}");

                foreach (Transform t in line.transform)
                {
                    // Main.Logger.LogMessage($"{t.name}: [localPosition: {t.localPosition}]");

                    if (t.name == "Player Name")
                    {
                        t.localPosition = new Vector3(-48.0f, 0.0f, 0.0f);
                        t.gameObject.SetActive(true);

                        continue;
                    }

                    if (t.name == "Color Swatch")
                    {
                        t.localPosition = new Vector3(-115.0f, 0.0f, 0.3f);

                        continue;
                    }

                    if (t.name == "Mute Button")
                    {
                        t.localPosition = new Vector3(-115.0f, 0.0f, 0.0f);
                        t.localScale = new Vector3(t.localScale.x, t.localScale.y, 0.25f * t.localScale.z);

                        t.GetComponent<Renderer>().forceRenderingOff = true;

                        if (t.TryGetComponent(out lineButton))
                        {
                            lineButton.debounceTime = 0.25f;
                        }

                        continue;
                    }

                    if (t.name == "gizmo-speaker")
                    {
                        Main.m_spriteGizmoOriginal ??= t.GetComponent<SpriteRenderer>().sprite;

                        t.localPosition = new Vector3(-115.0f, 0.0f, 0.0f);
                        t.localScale = new Vector3(1.8f, 1.8f, 1.8f);
                        t.GetComponent<SpriteRenderer>().sortingOrder++;

                        continue;
                    }

                    if (t.name == "ReportButton")
                    {
                        t.localPosition = new Vector3(32.0f, 0.0f, 0.0f);
                        t.localScale = new Vector3(t.localScale.x, t.localScale.y, 0.4f * t.localScale.z);
                        t.GetChild(0).gameObject.SetActive(true);
                        t.GetChild(0).localScale = new Vector3(0.028f, 0.028f, 1.0f);

                        continue;
                    }

                    if (t.name == "HateSpeech")
                    {
                        t.localPosition = new Vector3(46.0f, 0.0f, 0.0f);
                        t.localScale = new Vector3(t.localScale.x, t.localScale.y, 0.4f * t.localScale.z);

                        text = t.GetChild(0).GetComponent<Text>();
                        text.text = "HATE\nSPEECH";
                        text.gameObject.SetActive(true);
                        text.transform.localScale = new Vector3(0.025f, 0.025f, 1.0f);

                        if (t.TryGetComponent(out lineButton))
                        {
                            lineButton.offMaterial = new Material(lineButton.offMaterial)
                            {
                                color = new Color(0.85f, 0.85f, 0.85f)
                            };
                            t.GetComponent<MeshRenderer>().material = lineButton.offMaterial;
                        }

                        continue;
                    }

                    if (t.name == "Toxicity")
                    {
                        t.localPosition = new Vector3(60.0f, 0.0f, 0.0f);
                        t.localScale = new Vector3(t.localScale.x, t.localScale.y, 0.4f * t.localScale.z);

                        text = t.GetChild(0).GetComponent<Text>();
                        text.text = "TOXIC\nPERSON";
                        text.gameObject.SetActive(true);
                        text.transform.localScale = new Vector3(0.025f, 0.025f, 1.0f);

                        if (t.TryGetComponent(out lineButton))
                        {
                            lineButton.offMaterial = new Material(lineButton.offMaterial)
                            {
                                color = new Color(0.85f, 0.85f, 0.85f)
                            };
                            t.GetComponent<MeshRenderer>().material = lineButton.offMaterial;
                        }

                        continue;
                    }

                    if (t.name == "Cheating")
                    {
                        t.localPosition = new Vector3(74.0f, 0.0f, 0.0f);
                        t.localScale = new Vector3(t.localScale.x, t.localScale.y, 0.4f * t.localScale.z);

                        text = t.GetChild(0).GetComponent<Text>();
                        text.text = "CHEATER";
                        text.gameObject.SetActive(true);
                        text.transform.localScale = new Vector3(0.025f, 0.025f, 1.0f);

                        if (t.TryGetComponent(out lineButton))
                        {
                            lineButton.offMaterial = new Material(lineButton.offMaterial)
                            {
                                color = new Color(0.85f, 0.85f, 0.85f)
                            };
                            t.GetComponent<MeshRenderer>().material = lineButton.offMaterial;
                        }

                        continue;
                    }

                    if (t.name == "Cancel")
                    {
                        t.localPosition = new Vector3(32.0f, 0.0f, 0.0f);
                        t.localScale = new Vector3(t.localScale.x, t.localScale.y, 0.4f * t.localScale.z);

                        t.GetChild(0).gameObject.SetActive(true);
                        t.GetChild(0).localScale = new Vector3(0.03f, 0.03f, 1.0f);

                        if (t.TryGetComponent(out lineButton))
                        {
                            lineButton.offMaterial = new Material(lineButton.offMaterial)
                            {
                                color = new Color(0.85f, 0.85f, 0.85f)
                            };
                            t.GetComponent<MeshRenderer>().material = lineButton.offMaterial;
                        }

                        continue;
                    }
                }
            }
        }
    }
}
