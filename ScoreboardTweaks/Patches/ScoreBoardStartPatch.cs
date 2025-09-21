using BepInEx.Bootstrap;
using GorillaExtensions;
using GorillaTag;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ButtonType = GorillaPlayerLineButton.ButtonType;

namespace ScoreboardTweaks.Patches
{
    [HarmonyPatch(typeof(GorillaScoreBoard), nameof(GorillaScoreBoard.Start))]
    [HarmonyPriority(Priority.VeryHigh)]
    internal class ScoreBoardStartPatch
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

            __instance.boardText.margin = new Vector4(5.55f, 3f);
            __instance.boardText.lineSpacing = 60f;
            __instance.boardText.fontSize = 70f;

            if (Main.m_spriteGizmoOriginal == null && __instance.scoreBoardLinePrefab is GameObject prefab && prefab)
            {
                Transform child = prefab.transform.Find("gizmo-speaker");
                if (child != null && child.TryGetComponent(out SpriteRenderer spriteRenderer)) Main.m_spriteGizmoOriginal = spriteRenderer.sprite;
            }

            foreach (GorillaPlayerScoreboardLine line in __instance.lines)
            {
                // Main.Logger.LogMessage($"{line.transform.name}");

                Dictionary<object, TMP_Text> overrideTextDictionary = [];
                Main.m_lineTextOverride.TryAdd(line, overrideTextDictionary);

                foreach (Transform child in line.transform)
                {
                    // Main.Logger.LogMessage($"{t.name}: [localPosition: {t.localPosition}]");

                    if (child.name == "Player Name")
                    {
                        child.localPosition = new Vector3(-48.0f, 0.0f, 0.0f);

                        GameObject textMeshProObject = new("TextMeshPro : Player Text");
                        textMeshProObject.transform.parent = child.parent;
                        textMeshProObject.transform.localPosition = new Vector3(child.localPosition.x, 0f, 0f);
                        textMeshProObject.transform.localRotation = Quaternion.identity;
                        textMeshProObject.transform.localScale = Vector3.one;

                        RectTransform rectTransform = textMeshProObject.GetOrAddComponent<RectTransform>();
                        rectTransform.sizeDelta = child.GetComponent<RectTransform>().sizeDelta;

                        TextMeshPro textMeshPro = textMeshProObject.AddComponent<TextMeshPro>();
                        textMeshPro.font = __instance.boardText.font;
                        textMeshPro.fontSize = __instance.boardText.fontSize;
                        textMeshPro.horizontalAlignment = HorizontalAlignmentOptions.Left;
                        textMeshPro.verticalAlignment = VerticalAlignmentOptions.Geometry;
                        textMeshPro.margin = new Vector4(0.8f, 0f);
                        textMeshPro.characterSpacing = __instance.boardText.characterSpacing;
                        textMeshPro.color = __instance.boardText.color;
                        textMeshPro.text = child.GetComponent<Text>()?.text ?? "PLAYER NAME";

                        overrideTextDictionary.TryAdd("PlayerText", textMeshPro);

                        if (__instance.TryGetComponent(out StaticLodGroup lodGroup))
                        {
                            int index = lodGroup.index;
                            if (StaticLodManager.groupInfos.ElementAtOrDefault(index) is StaticLodManager.GroupInfo groupInfo)
                            {
                                groupInfo.uiTMPs = [.. groupInfo.uiTMPs, textMeshPro];
                                StaticLodManager.groupInfos[index] = groupInfo;
                            }
                        }

                        continue;
                    }

                    if (child.name == "Color Swatch")
                    {
                        child.localPosition = new Vector3(-115.0f, 0.0f, 0.0f);

                        continue;
                    }

                    if (child.name == "gizmo-speaker")
                    {
                        Main.m_spriteGizmoOriginal ??= child.GetComponent<SpriteRenderer>().sprite;

                        child.localPosition = new Vector3(-115.0f, 0.0f, 0.15f);
                        child.localScale = new Vector3(1.8f, 1.8f, 1.8f);
                        child.GetComponent<SpriteRenderer>().sortingOrder++;

                        continue;
                    }

                    if (child.TryGetComponent(out GorillaPlayerLineButton lineButton))
                    {
                        ButtonType buttonType = lineButton.buttonType;

                        GorillaPlayerLineButton mainButtonInstance = buttonType switch
                        {
                            ButtonType.Mute => lineButton.parentLine.muteButton,
                            ButtonType.Report => lineButton.parentLine.reportButton,
                            ButtonType.HateSpeech => lineButton.parentLine.hateSpeechButton.GetComponent<GorillaPlayerLineButton>(),
                            ButtonType.Toxicity => lineButton.parentLine.toxicityButton.GetComponent<GorillaPlayerLineButton>(),
                            ButtonType.Cheating => lineButton.parentLine.cheatingButton.GetComponent<GorillaPlayerLineButton>(),
                            ButtonType.Cancel => lineButton.parentLine.cancelButton.GetComponent<GorillaPlayerLineButton>(),
                            _ => null
                        };

                        if (mainButtonInstance != null && mainButtonInstance && mainButtonInstance != lineButton) continue;

                        if (buttonType == ButtonType.Mute)
                        {
                            lineButton.debounceTime = 0.25f;
                            child.localPosition = new Vector3(-115.0f, 0.0f, 0.0f);
                            child.localScale = new Vector3(child.localScale.x, child.localScale.y, 0.25f * child.localScale.z);
                            child.GetComponent<Renderer>().forceRenderingOff = true;
                        }
                        else
                        {
                            child.localPosition = buttonType switch
                            {
                                ButtonType.HateSpeech => new Vector3(44.0f, 0.0f, 0.0f),
                                ButtonType.Toxicity => new Vector3(58.0f, 0.0f, 0.0f),
                                ButtonType.Cheating => new Vector3(72.0f, 0.0f, 0.0f),
                                _ => new Vector3(30.0f, 0.0f, 0.0f)
                            };
                            child.localScale = new Vector3(child.localScale.x, child.localScale.y, 0.4f * child.localScale.z);

                            GameObject textMeshProObject = new($"TextMeshPro : {buttonType.GetName()} Button");
                            textMeshProObject.transform.parent = lineButton.myText.transform.parent;
                            textMeshProObject.transform.localPosition = new Vector3(0f, 0f, lineButton.myText.transform.localPosition.z);
                            textMeshProObject.transform.localRotation = Quaternion.identity;
                            textMeshProObject.transform.localScale = lineButton.myText.transform.localScale;

                            RectTransform rectTransform = textMeshProObject.GetOrAddComponent<RectTransform>();
                            rectTransform.sizeDelta = lineButton.myText.GetComponent<RectTransform>().sizeDelta;

                            TextMeshPro textMeshPro = textMeshProObject.AddComponent<TextMeshPro>();
                            textMeshPro.font = __instance.boardText.font;
                            textMeshPro.fontSize = 100f;
                            textMeshPro.fontSizeMin = 0f;
                            textMeshPro.fontSizeMax = 100f;
                            textMeshPro.enableAutoSizing = true;
                            textMeshPro.alignment = TextAlignmentOptions.Center;
                            textMeshPro.margin = new Vector4(57f, 5.5f, 57f, 5.5f);
                            textMeshPro.characterSpacing = -10f;
                            textMeshPro.lineSpacing = 10f;
                            textMeshPro.color = lineButton.myText.color;
                            textMeshPro.text = buttonType switch
                            {
                                ButtonType.HateSpeech => "HATE\nSPEECH",
                                ButtonType.Toxicity => "TOXIC\nPLAYER",
                                ButtonType.Cheating => "CHEATER",
                                ButtonType.Cancel => "CANCEL",
                                _ => "REPORT"
                            };

                            lineButton.offText = textMeshPro.text;
                            lineButton.myText.enabled = false;

                            overrideTextDictionary.TryAdd(buttonType, textMeshPro);

                            if (__instance.TryGetComponent(out StaticLodGroup lodGroup))
                            {
                                int index = lodGroup.index;
                                if (StaticLodManager.groupInfos.ElementAtOrDefault(index) is StaticLodManager.GroupInfo groupInfo)
                                {
                                    groupInfo.uiTMPs = [.. groupInfo.uiTMPs, textMeshPro];
                                    StaticLodManager.groupInfos[index] = groupInfo;
                                }
                            }

                            if (buttonType != ButtonType.Report)
                            {
                                lineButton.offMaterial = new Material(lineButton.offMaterial)
                                {
                                    color = new Color(0.85f, 0.85f, 0.85f)
                                };
                                child.GetComponent<MeshRenderer>().material = lineButton.offMaterial;
                            }
                        }

                        continue;
                    }
                }
            }
        }
    }
}
