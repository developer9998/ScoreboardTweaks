﻿using BepInEx.Bootstrap;
using HarmonyLib;
using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ScoreboardTweaks.Patches
{
    /* Gorilla Tag v1.1.0 */

    /* Rebuilding buttons */
    [HarmonyPatch(typeof(GorillaScoreBoard))]
    [HarmonyPatch("RedrawPlayerLines", MethodType.Normal)]
    internal class GorillaScoreBoardRedrawPlayerLines
    {
        private static bool Prefix(GorillaScoreBoard __instance)
        {
            __instance.boardText.text = "ROOM ID: " + ((PhotonNetwork.CurrentRoom == null || !PhotonNetwork.CurrentRoom.IsVisible) ? "-PRIVATE-" : PhotonNetwork.CurrentRoom.Name) + "\n  PLAYER STATUS            REPORT";
            __instance.buttonText.text = "";
            __instance.lines.Sort((Comparison<GorillaPlayerScoreboardLine>)((line1, line2) => line1.playerActorNumber.CompareTo(line2.playerActorNumber)));
            for (int index = 0; index < __instance.lines.Count; ++index)
            {
                __instance.lines[index].gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0.0f, (float)(__instance.startingYValue - __instance.lineHeight * index), 0.0f);
            }
            return false;
        }
    }
    /* Gorilla Tag v1.1.0 */

    /* Rebuilding buttons */
    [HarmonyPatch(typeof(GorillaScoreBoard))]
    [HarmonyPatch("Awake", MethodType.Normal)]
    internal class GorillaScoreBoardAwake
    {
        public static bool initialized { get; internal set; } = false;
        //private static void Postfix(GorillaScoreBoard __instance)
        //{
        //    __instance.boardText.text = "ROOM ID: " + ((PhotonNetwork.CurrentRoom == null || !PhotonNetwork.CurrentRoom.IsVisible) ? "-PRIVATE-" : PhotonNetwork.CurrentRoom.Name) + "\n  PLAYER STATUS            REPORT";
        //}
        private static void Prefix(GorillaScoreBoard __instance)
        {
            Main.m_listScoreboardTexts.Add(__instance.boardText);
            __instance.boardText.transform.localPosition = new Vector3(
                __instance.boardText.transform.localPosition.x,
                __instance.boardText.transform.localPosition.y,
                0.25f * __instance.boardText.transform.localPosition.z
                );

            if (initialized) return;
            initialized = true;

            foreach (var plugin in Chainloader.PluginInfos.Values)
            {
                try { AccessTools.Method(plugin.Instance.GetType(), "OnScoreboardTweakerProcessedPre")?.Invoke(plugin.Instance, new object[] { __instance.scoreBoardLinePrefab }); } catch (Exception e) { }
            }

            Text tmpText;
            foreach (Transform t in __instance.scoreBoardLinePrefab.transform)
            {
                if (t.name == "Player Name")
                {
                    t.localPosition = new Vector3(-48.0f, 0.0f, 0.0f);
                    t.gameObject.SetActive(true);
                    //t.localScale = new Vector3(0.8f, 0.8f, 1.0f);
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
                    tmpText = t.GetChild(0).GetComponent<Text>();
                    tmpText.gameObject.SetActive(true); // GT 1.1.0
                    tmpText.color = Color.clear;
                    GameObject.Destroy(t.GetComponent<MeshRenderer>());

                    t.GetChild(0).localScale = new Vector3(0.04f, 0.04f, 1.0f);
                    continue;
                }
                if (t.name == "ReportButton")
                {
                    t.GetChild(0).localScale = new Vector3(0.028f, 0.028f, 1.0f);
                    t.localPosition = new Vector3(32.0f, 0.0f, 0.0f);
                    t.localScale = new Vector3(t.localScale.x, t.localScale.y, 0.4f * t.localScale.z);

                    // GT 1.1.0
                    tmpText = t.GetChild(0).GetComponent<Text>();
                    tmpText.gameObject.SetActive(true);
                    // GT 1.1.0

                    continue;
                }
                if (t.name == "gizmo-speaker")
                {
                    Main.m_spriteGizmoOriginal = t.GetComponent<SpriteRenderer>().sprite;
                    t.localPosition = new Vector3(-115.0f, 0.0f, 0.0f);
                    t.localScale = new Vector3(1.8f, 1.8f, 1.8f);
                    continue;
                }
                if (t.name == "HateSpeech")
                {
                    t.GetChild(0).localScale = new Vector3(0.03f, 0.03f, 1.0f);

                    // GT 1.1.0
                    tmpText = t.GetChild(0).GetComponent<Text>();
                    tmpText.gameObject.SetActive(true);
                    tmpText.GetComponent<Text>().text = "HATE\nSPEECH";
                    // GT 1.1.0
                    //t.GetChild(0).GetComponent<Text>().text = "HATE\nSPEECH";
                    t.localPosition = new Vector3(46.0f, 0.0f, 0.0f);
                    t.localScale = new Vector3(t.localScale.x, t.localScale.y, 0.4f * t.localScale.z);
                    GorillaPlayerLineButton controller = t.gameObject.GetComponent<GorillaPlayerLineButton>();
                    if (controller != null)
                    {
                        controller.offMaterial = new Material(controller.offMaterial);
                        controller.offMaterial.color = new Color(0.85f, 0.85f, 0.85f);
                        t.GetComponent<MeshRenderer>().material = controller.offMaterial;
                    }
                    continue;
                }
                if (t.name == "Toxicity")
                {
                    t.GetChild(0).localScale = new Vector3(0.03f, 0.03f, 1.0f);
                    // GT 1.1.0
                    tmpText = t.GetChild(0).GetComponent<Text>();
                    tmpText.gameObject.SetActive(true);
                    tmpText.GetComponent<Text>().text = "TOXIC\nPERSON";
                    // GT 1.1.0
                    //t.GetChild(0).GetComponent<Text>().text = "TOXIC\nPERSON";
                    t.localPosition = new Vector3(60.0f, 0.0f, 0.0f);
                    t.localScale = new Vector3(t.localScale.x, t.localScale.y, 0.4f * t.localScale.z);
                    GorillaPlayerLineButton controller = t.gameObject.GetComponent<GorillaPlayerLineButton>();
                    if (controller != null)
                    {
                        controller.offMaterial = new Material(controller.offMaterial);
                        controller.offMaterial.color = new Color(0.85f, 0.85f, 0.85f);
                        t.GetComponent<MeshRenderer>().material = controller.offMaterial;
                    }
                    continue;
                }
                if (t.name == "Cheating")
                {
                    t.GetChild(0).localScale = new Vector3(0.028f, 0.028f, 1.0f);
                    // GT 1.1.0
                    tmpText = t.GetChild(0).GetComponent<Text>();
                    tmpText.gameObject.SetActive(true);
                    tmpText.GetComponent<Text>().text = "CHEATER";
                    // GT 1.1.0
                    //t.GetChild(0).GetComponent<Text>().text = "CHEATER";
                    t.localPosition = new Vector3(74.0f, 0.0f, 0.0f);
                    t.localScale = new Vector3(t.localScale.x, t.localScale.y, 0.4f * t.localScale.z);
                    GorillaPlayerLineButton controller = t.gameObject.GetComponent<GorillaPlayerLineButton>();
                    if (controller != null)
                    {
                        controller.offMaterial = new Material(controller.offMaterial);
                        controller.offMaterial.color = new Color(0.85f, 0.85f, 0.85f);
                        t.GetComponent<MeshRenderer>().material = controller.offMaterial;
                    }
                    continue;
                }
                if (t.name == "Cancel")
                {
                    // GT 1.1.0
                    t.GetChild(0).GetComponent<Text>().gameObject.SetActive(true);
                    // GT 1.1.0

                    t.GetChild(0).localScale = new Vector3(0.03f, 0.03f, 1.0f);
                    t.localPosition = new Vector3(32.0f, 0.0f, 0.0f);
                    t.localScale = new Vector3(t.localScale.x, t.localScale.y, 0.4f * t.localScale.z);
                    GorillaPlayerLineButton controller = t.gameObject.GetComponent<GorillaPlayerLineButton>();
                    if (controller != null)
                    {
                        controller.offMaterial = new Material(controller.offMaterial);
                        controller.offMaterial.color = new Color(0.85f, 0.85f, 0.85f);
                        t.GetComponent<MeshRenderer>().material = controller.offMaterial;
                    }
                    continue;
                }
            }

            foreach (var plugin in Chainloader.PluginInfos.Values)
            {
                try { AccessTools.Method(plugin.Instance.GetType(), "OnScoreboardTweakerProcessed")?.Invoke(plugin.Instance, new object[] { __instance.scoreBoardLinePrefab }); } catch (Exception e) { }
            }
        }
    }

    /* Fixing log spamming from button's MeshRenderer deleted */
    [HarmonyPatch(typeof(GorillaPlayerScoreboardLine))]
    [HarmonyPatch("HideShowLine", MethodType.Normal)]
    internal class GorillaPlayerScoreboardLineHideShow
    {
        private static bool Prefix(Text[] ___texts, SpriteRenderer[] ___sprites, MeshRenderer[] ___meshes, Image[] ___images, bool active)
        {
            foreach (Text text in ___texts)
            {
                if (text.enabled != active)
                    text.enabled = active;
            }
            foreach (SpriteRenderer sprite in ___sprites)
            {
                if (sprite.enabled != active)
                    sprite.enabled = active;
            }
            foreach (MeshRenderer mesh in ___meshes)
            {
                if (mesh != null && mesh.enabled != active)
                    mesh.enabled = active;
            }
            foreach (Image image in ___images)
            {
                if (image.enabled != active)
                    image.enabled = active;
            }
            return false;
        }
    }

    /* Forcing a muted icon */
    [HarmonyPatch(typeof(GorillaPlayerScoreboardLine))]
    [HarmonyPatch("Update", MethodType.Normal)]
    internal class GorillaPlayerScoreboardLineUpdate
    {
        private static void Postfix(GorillaPlayerScoreboardLine __instance)
        {
            if (__instance.playerVRRig.muted && __instance.speakerIcon.activeSelf != true)
            {
                __instance.speakerIcon.SetActive(true);
            }
        }
    }

    /* UpdateColor log spamming after MeshRenderer delete */
    [HarmonyPatch(typeof(GorillaPlayerLineButton))]
    [HarmonyPatch("UpdateColor", MethodType.Normal)]
    internal class GorillaPlayerLineButtonUpdateColor
    {
        private static bool Prefix(GorillaPlayerLineButton __instance)
        {
            if (__instance.parentLine.muteButton == __instance) return false;
            return true;
        }
    }

    /* Fixing "Cancel" pressed after "Report" pressing */
    /* Changing mute icon! */
    [HarmonyPatch(typeof(GorillaPlayerLineButton))]
    [HarmonyPatch("OnTriggerEnter", MethodType.Normal)]
    internal class GorillaPlayerLineButtonTriggerEnter
    {
        internal static float m_flNextPress = 0.0f;
        private static bool Prefix(GorillaPlayerLineButton __instance, Collider collider)
        {
            if (!__instance.enabled || m_flNextPress > Time.time || __instance.touchTime + __instance.debounceTime >= Time.time) return false;
            m_flNextPress = Time.time + 0.05f;
            if (__instance.transform.name == "Mute Button")
            {
                if (__instance.isOn) // Unmuting
                {
                    __instance.parentLine.speakerIcon.GetComponent<SpriteRenderer>().sprite = Main.m_spriteGizmoOriginal;
                }
                else // Muting
                {
                    __instance.parentLine.speakerIcon.GetComponent<SpriteRenderer>().sprite = Main.m_spriteGizmoMuted;
                }
                return true;
            }
            return true;
        }
    }
}