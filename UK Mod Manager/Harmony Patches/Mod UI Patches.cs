using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UMM.Loader;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace UMM.HarmonyPatches
{
    [HarmonyPatch(typeof(OptionsMenuToManager), "Start")]
    internal static class Inject_ModsButton
    {
        private static Dictionary<ModInformation, Button> currentButtons = new Dictionary<ModInformation, Button>();

        public static void ReportModLoaded(ModInformation info)
        {
            if (currentButtons.ContainsKey(info) && currentButtons[info] != null)
            {
                currentButtons[info].targetGraphic.color = info.loaded ? Color.green : Color.red;
            }
        }
        
        private static void Prefix(OptionsMenuToManager __instance)
        {
            if (__instance.pauseMenu.name == "Main Menu (1)") // check to see that we're patching out the main menu's menu, not like an in game menu one
            {
                // Inject the mods button                

                bool wasOn = MonoSingleton<PrefsManager>.Instance.GetBool("variationMemory", false);
                __instance.pauseMenu.transform.Find("Panel").localPosition = new Vector3(0, 325, 0);

                void Halve(Transform tf, bool left)
                {
                    bool wasActive = tf.gameObject.activeSelf;
                    tf.gameObject.SetActive(false);
                    //tf.localScale = new Vector3(.5f, 1f, 1f);
                    tf.GetComponent<RectTransform>().sizeDelta = new Vector2(240, 80);
                    //tf.Find("Text").localScale = new Vector3(2f, 1f, 1f);
                    if (left)
                        tf.localPosition -= new Vector3(120f, 0, 0);
                    else
                        tf.localPosition += new Vector3(120f, 0, 0);
                    Traverse hudEffect = Traverse.Create(tf.gameObject.GetComponent<HudOpenEffect>());
                    hudEffect.Field("originalWidth").SetValue(1f);
                    hudEffect.Field("originalHeight").SetValue(1f);
                    tf.gameObject.SetActive(wasActive);
                }

                Transform options = __instance.pauseMenu.transform.Find("Options");
                Halve(options, true);

                GameObject newModsButton = GameObject.Instantiate(__instance.pauseMenu.transform.Find("Continue").gameObject, __instance.pauseMenu.transform, true);
                newModsButton.SetActive(false);
                newModsButton.transform.localPosition = new Vector3(0, options.localPosition.y, 0);
                Halve(newModsButton.transform, false);
                newModsButton.GetComponentInChildren<Text>(true).text = "MODS";

                Transform panel = __instance.pauseMenu.transform.Find("Panel");
                GameObject discordButton = panel.Find("Discord").gameObject;
                discordButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-164f, -350f);
                discordButton.transform.parent.Find("Twitter").GetComponent<RectTransform>().anchoredPosition = new Vector2(-329f, -405.5f);
                discordButton.transform.parent.Find("Youtube").GetComponent<RectTransform>().anchoredPosition = new Vector2(-494f, -461f);

                GameObject gitButton = GameObject.Instantiate(discordButton, discordButton.transform.parent);
                gitButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(492f, -350f);
                gitButton.GetComponentInChildren<Text>().text = "UMM SOURCE";
                gitButton.GetComponentInChildren<Image>().color = new Color32(191, 191, 191, 255);
                gitButton.GetComponentInChildren<WebButton>().url = "https://github.com/Temperz87/ultra-mod-manager";

                GameObject moreModsButton = GameObject.Instantiate(discordButton, discordButton.transform.parent);
                moreModsButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(492f, -405.5f);
                moreModsButton.GetComponentInChildren<Text>().text = "BROWSE MODS";
                moreModsButton.GetComponentInChildren<Image>().color = new Color32(211, 218, 114, 255);
                moreModsButton.GetComponentInChildren<WebButton>().url = "https://docs.google.com/spreadsheets/d/1x8P3GcdfWraZX1kz3bbHJIiY4hozxe8k1oieOm_fuL0/edit?usp=sharing";

                GameObject thunderstoreButton = GameObject.Instantiate(discordButton, discordButton.transform.parent);
                thunderstoreButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(492f, -461f);
                thunderstoreButton.GetComponentInChildren<Text>().text = "THUNDER STORE";
                thunderstoreButton.GetComponentInChildren<Image>().color = new Color32(55, 90, 127, 255);
                thunderstoreButton.GetComponentInChildren<WebButton>().url = "https://thunderstore.io/c/ultrakill/";

                if (UltraModManager.outdated)
                {
                    GameObject outDatedButton = GameObject.Instantiate(discordButton, discordButton.transform.parent);
                    outDatedButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(492f, -516.5f);
                    outDatedButton.GetComponentInChildren<Text>().text = "NEW VERSION FOUND: " + UltraModManager.newLoaderVersion.Trim();
                    outDatedButton.GetComponentInChildren<Image>().color = new Color32(69, 0, 69, 255);
                    outDatedButton.GetComponentInChildren<WebButton>().url = "https://github.com/Temperz87/ultra-mod-manager/releases/tag/" + UltraModManager.newLoaderVersion;
                }
                else if (UltraModManager.devBuild)
                {
                    GameObject devBuildButton = GameObject.Instantiate(discordButton, discordButton.transform.parent);
                    devBuildButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(492f, -516.5f);
                    devBuildButton.GetComponentInChildren<Text>().text = "DEV BUILD: " + UltraModManager.newLoaderVersion.Trim();
                    devBuildButton.GetComponentInChildren<Image>().color = new Color32(69, 0, 69, 255);
                    devBuildButton.GetComponentInChildren<WebButton>().url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";
                }

                GameObject modsMenu = GameObject.Instantiate(__instance.optionsMenu, __instance.transform);
                modsMenu.SetActive(false);
                modsMenu.GetComponent<Image>().enabled = false;
                for (int i = 0; i < modsMenu.transform.childCount; i++)
                    modsMenu.transform.GetChild(i).gameObject.SetActive(false);

                Transform colorBlind = modsMenu.transform.Find("ColorBlindness Options");
                colorBlind.gameObject.SetActive(true);
                GameObject.Destroy(colorBlind.GetComponent<GamepadObjectSelector>()); // sorry gamepad players, but without this the mod manager breaks
                Text modHeaderText = colorBlind.transform.Find("Text (1)").GetComponent<Text>();
                modHeaderText.text = "--MODS--";

                Transform content = colorBlind.transform.Find("Scroll Rect").Find("Contents");
                RectTransform cRect = content.GetComponent<RectTransform>();
                content.Find("Enemies").gameObject.SetActive(false);
                content.gameObject.SetActive(true);
                GameObject template = content.Find("HUD").Find("Health").gameObject;
                content.Find("HUD").gameObject.SetActive(false);
                template.SetActive(false);
                __instance.variationMemory.gameObject.SetActive(false);

                GameObject hoverTextGo = content.Find("Default").gameObject;
                hoverTextGo.transform.parent = modsMenu.transform;
                Text hoverText = hoverTextGo.GetComponentInChildren<Text>();
                hoverTextGo.transform.localPosition -= new Vector3(0f, 520f, 0f);
                GameObject.Destroy(hoverTextGo.GetComponent<BackSelectOverride>());
                GameObject.Destroy(hoverTextGo.GetComponent<Button>());
                hoverTextGo.SetActive(false);

                ModInformation[] information = UKAPI.AllModInfoClone.Values.ToArray();
                currentButtons = new Dictionary<ModInformation, Button>();
                if (information.Length > 0)
                {
                    Array.Sort(information);
                    for (int i = 0; i < information.Length; i++)
                    {
                        ModInformation info = information[i];
                        GameObject newInformation = GameObject.Instantiate(template, content);
                        GameObject.Destroy(newInformation.GetComponent<ColorBlindSetter>());
                        Image infoImage = newInformation.GetComponent<Image>();
                        infoImage.color = new Color32(0, 0, 0, 150);

                        Text modText = newInformation.transform.Find("Text").GetComponent<Text>();
                        modText.text = info.modName + " " + info.modVersion;
                        modText.fontSize = (int)(modText.fontSize/1.5f);
                        modText.alignment = TextAnchor.UpperCenter;
                        modText.transform.localPosition = new Vector3(0f, -3f, 0f);
                        modText.transform.localScale = new Vector3(0.66764f, 0.66764f, 0.66764f);
                        modText.color = new Color32(255, 255, 255, 255);

                        Button newButton = newInformation.AddComponent<Button>();
                        newButton.transition = Selectable.Transition.ColorTint;
                        newButton.targetGraphic = modText;
                        newButton.targetGraphic.color = info.loaded ? Color.green : Color.red;
                        newButton.onClick = new Button.ButtonClickedEvent();
                        newButton.onClick.AddListener(delegate
                        {
                            info.Clicked();
                            newButton.targetGraphic.color = info.loaded ? Color.green : Color.red;
                        });
                        currentButtons.Add(info, newButton);
                        
                        newInformation.transform.Find("Red").gameObject.SetActive(false);
                        newInformation.transform.Find("Green").gameObject.SetActive(false);
                        newInformation.transform.Find("Blue").gameObject.SetActive(false);
                        newInformation.transform.localScale = new Vector3(1.64415f, 1.64415f, 1.64415f);
                        RectTransform infoRect = newInformation.GetComponent<RectTransform>();
                        infoRect.sizeDelta = new Vector2(infoRect.sizeDelta.x, infoRect.sizeDelta.y / 1.2f);
                        newInformation.transform.localPosition = new Vector3(0f, (infoRect.sizeDelta.y + 70f) * -1f * i, 0f);

                        Text descriptionText = GameObject.Instantiate(modText.gameObject, modText.transform.parent).GetComponent<Text>();
                        descriptionText.alignment = TextAnchor.MiddleCenter;
                        descriptionText.rectTransform.offsetMin = new Vector2(-3.58125f, -170f);
                        descriptionText.rectTransform.offsetMax = new Vector2(3.58125f, -16.4f);
                        //descriptionText.transform.localScale = new Vector3(0.54764f, 0.66764f, 0.66764f);
                        descriptionText.transform.localScale = Vector3.one * 0.56f;
                        descriptionText.fontSize = 14;
                        descriptionText.text = info.modDescription;
                        descriptionText.transform.localPosition = new Vector3(0f, descriptionText.transform.localPosition.y + 10f, 0);
                        descriptionText.color = new Color32(255, 255, 255, 255);
                        
                        Transform imageTransform = newInformation.transform.Find("Image");
                        if (info.previewIcon == null)
                            imageTransform.gameObject.SetActive(false);
                        else
                        {
                            IEnumerator setImageNextFrame() // So unity waits a frame before destroying a component, hence this badness
                            {
                                yield return null;
                                RawImage img = imageTransform.gameObject.AddComponent<RawImage>();
                                img.texture = info.previewIcon;
                                img.raycastTarget = false;
                                imageTransform.gameObject.SetActive(true);
                            }
                            imageTransform.localPosition += new Vector3(-6f, 21f, 0f);
                            descriptionText.transform.localPosition += new Vector3(4f, 0f, 0f);
                            __instance.StartCoroutine(setImageNextFrame());
                            GameObject.Destroy(imageTransform.GetComponent<Image>());
                        }

                        GameObject loadOnStart = GameObject.Instantiate(__instance.variationMemory.gameObject, newInformation.transform);
                        loadOnStart.transform.localPosition = new Vector3(247f, -19f, 0f);
                        Toggle loadOnStartToggle = loadOnStart.GetComponent<Toggle>();
                        loadOnStartToggle.isOn = info.loadOnStart;
                        loadOnStartToggle.onValueChanged = new Toggle.ToggleEvent();
                        loadOnStartToggle.onValueChanged.AddListener(delegate
                        {
                            info.loadOnStart = loadOnStartToggle.isOn;
                            UKAPI.SaveFileHandler.SetModData(info.modName, "LoadOnStart", info.loadOnStart.ToString());
                        });
                        EventTrigger loadOnStartTrigger = loadOnStartToggle.gameObject.AddComponent<EventTrigger>();
                        EventTrigger.Entry loadOnStartHoverEntry = new EventTrigger.Entry();
                        loadOnStartHoverEntry.eventID = EventTriggerType.PointerEnter;
                        loadOnStartHoverEntry.callback.AddListener(delegate
                        {
                            hoverText.text = "Toggles auto loading on game start";
                            hoverTextGo.SetActive(true);
                        });
                        EventTrigger.Entry loadOnStartUnHoverEntry = new EventTrigger.Entry();
                        loadOnStartUnHoverEntry.eventID = EventTriggerType.PointerExit;
                        loadOnStartUnHoverEntry.callback.AddListener(delegate
                        {
                            hoverTextGo.SetActive(false);
                        });
                        loadOnStartTrigger.triggers.Add(loadOnStartHoverEntry);
                        loadOnStartTrigger.triggers.Add(loadOnStartUnHoverEntry);

                        /*
                        GameObject settings = GameObject.Instantiate(__instance.variationMemory.gameObject, newInformation.transform);
                        settings.transform.localPosition = new Vector3(247f, -39f, 0f);
                        Toggle settingsToggle = settings.GetComponent<Toggle>();
                        //settingsToggle.isOn = info.settings;
                        settingsToggle.onValueChanged = new Toggle.ToggleEvent();
                        settingsToggle.onValueChanged.AddListener(delegate
                        {

                        });
                        EventTrigger settingsTrigger = settingsToggle.gameObject.AddComponent<EventTrigger>();
                        EventTrigger.Entry settingsHoverEntry = new EventTrigger.Entry();
                        settingsHoverEntry.eventID = EventTriggerType.PointerEnter;
                        settingsHoverEntry.callback.AddListener(delegate
                        {
                            hoverText.text = "Settings";
                            hoverTextGo.SetActive(true);
                        });
                        EventTrigger.Entry settingsUnHoverEntry = new EventTrigger.Entry();
                        settingsUnHoverEntry.eventID = EventTriggerType.PointerExit;
                        settingsUnHoverEntry.callback.AddListener(delegate
                        {
                            hoverTextGo.SetActive(false);
                        });
                        settingsTrigger.triggers.Add(settingsHoverEntry);
                        settingsTrigger.triggers.Add(settingsUnHoverEntry);
                        */

                        loadOnStart.SetActive(true);
                        //settings.SetActive(true);
                        newInformation.SetActive(true);
                    }

                    cRect.sizeDelta = new Vector2(600f, information.Length * 170); // setting the scrollbar to fit all of the mods
                }
                else
                {
                    content.gameObject.SetActive(false);
                    hoverTextGo.transform.localPosition += new Vector3(0f, 260f, 0f);
                    hoverText = hoverTextGo.GetComponentInChildren<Text>();
                    hoverText.text = "NO MODS FOUND IN MOD FOLDER:\n" + UltraModManager.modsDirectory;
                    hoverText.horizontalOverflow = HorizontalWrapMode.Wrap;
                    hoverText.verticalOverflow = VerticalWrapMode.Overflow;
                    hoverText.fontSize /= 2;
                    hoverTextGo.SetActive(true);
                }

                __instance.variationMemory.gameObject.SetActive(true);
                MonoSingleton<PrefsManager>.Instance.SetBool("variationMemory", wasOn); // there's a bug where this patch sets variation memory to always be on once you get to the menu for some reason, this fixes that

                Button.ButtonClickedEvent modsButton = newModsButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
                modsButton.AddListener(delegate
                {
                    __instance.CheckIfTutorialBeaten();
                    __instance.pauseMenu.SetActive(false);
                    modsMenu.SetActive(true);
                });
                newModsButton.SetActive(true);

                Transform quitButton = __instance.pauseMenu.transform.Find("Quit");
                Halve(quitButton, true);

                GameObject restartButton = GameObject.Instantiate(__instance.pauseMenu.transform.Find("Continue").gameObject, __instance.pauseMenu.transform, true);
                restartButton.SetActive(false);
                Button.ButtonClickedEvent restartButtonEvent = restartButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent(); // I have no memory of writing this, when did this get here?
                restartButtonEvent.AddListener(delegate
                {
                    UKAPI.Restart();
                });
                restartButton.transform.localPosition = new Vector3(0, quitButton.localPosition.y, 0);
                restartButton.GetComponentInChildren<Text>(true).text = "RESTART";
                restartButton.SetActive(true);
                Halve(restartButton.transform, false);
            }

            // Here we don't care what menu we're patching (main menu or in game one), hence the stuff here is outside the if

            // Inject mod control options
            GameObject controlOptions = __instance.optionsMenu.transform.Find("Controls Options").gameObject;
            Transform controlContent = controlOptions.transform.Find("Scroll Rect").Find("Contents");
            controlContent.GetComponent<RectTransform>().sizeDelta = new Vector2(620f, 1558.6f);

            GameObject modOptions = GameObject.Instantiate(controlContent.Find("Weapons Settings").gameObject, controlContent);
            modOptions.transform.localPosition -= new Vector3(0f, 410f, 0f);
            GameObject bindTemplate = modOptions.transform.Find("Slot 1").gameObject;
            bindTemplate.SetActive(false);
            modOptions.transform.Find("Slot 2").gameObject.SetActive(false);
            modOptions.transform.Find("Slot 3").gameObject.SetActive(false);
            modOptions.transform.Find("Slot 4").gameObject.SetActive(false);
            modOptions.transform.Find("Slot 5").gameObject.SetActive(false);
            modOptions.transform.Find("Mouse Wheel Settings").gameObject.SetActive(false);
            modOptions.transform.Find("Text (1)").GetComponent<Text>().text = "-- MODDED --";

            List<string> binds = UKAPI.KeyBindHandler.moddedKeyBinds.Keys.ToList().Where(x => UKAPI.KeyBindHandler.moddedKeyBinds[x].enabled).ToList(); // CoPilot wrote that Where statement, I am so fucking bamboozled
            int bindIndex;
            for (bindIndex = 0; bindIndex < binds.Count; bindIndex++)
            {
                string keybindName = binds[bindIndex];
                UKKeyBind keybind = UKAPI.KeyBindHandler.moddedKeyBinds[keybindName];
                GameObject newKeyBind = GameObject.Instantiate(bindTemplate, modOptions.transform);
                if (bindIndex % 2 == 0)
                    newKeyBind.transform.localPosition = new Vector3(0f, -80f * ((bindIndex / 2) + 1), 0f); // I love math
                else
                    newKeyBind.transform.localPosition = new Vector3(315f, -80f * ((bindIndex + 1) / 2), 0f); // So much fun
                newKeyBind.SetActive(true);
                newKeyBind.transform.Find("Text").GetComponent<Text>().text = keybindName;
                Text toSet = newKeyBind.transform.Find("Slot1").Find("Text").GetComponent<Text>();
                toSet.text = keybind.keyBind.ToString();

                Button button = newKeyBind.transform.Find("Slot1").GetComponent<Button>();
                button.onClick = new Button.ButtonClickedEvent();
                button.onClick.AddListener(delegate
                {
                    OptionsManager.Instance.dontUnpause = true;
                    __instance.StartCoroutine(UKAPI.KeyBindHandler.SetKeyBindRoutine(button.gameObject, keybindName));
                    button.gameObject.GetComponent<Image>().color = new Color32(255, 103, 0, 255);
                });

                Text keyText = button.gameObject.GetComponentInChildren<Text>();
                keybind.OnBindingChanged.AddListener(delegate (KeyCode newBind)
                {
                    if (keyText != null)
                        keyText.text = newBind.ToString().Trim();
                });
            }

            RectTransform controlContentRect = controlContent.GetComponent<RectTransform>();
            controlContentRect.sizeDelta = new Vector2(620f, 1558.6f + (binds.Count * 20)); // setting the scrollbar to fit all of the binds
            UKAPI.KeyBindHandler.OnKeyBindEnabled.AddListener(delegate (UKKeyBind keybind)
            {
                if (bindTemplate == null)
                    return;
                GameObject newKeyBind = GameObject.Instantiate(bindTemplate, modOptions.transform);
                if (bindIndex % 2 == 0)
                    newKeyBind.transform.localPosition = new Vector3(0f, -80f * ((bindIndex / 2) + 1), 0f); // I love math
                else
                    newKeyBind.transform.localPosition = new Vector3(315f, -80f * ((bindIndex + 1) / 2), 0f); // So much fun
                newKeyBind.SetActive(true);
                newKeyBind.transform.Find("Text").GetComponent<Text>().text = keybind.bindName;
                Text toSet = newKeyBind.transform.Find("Slot1").Find("Text").GetComponent<Text>();
                toSet.text = keybind.keyBind.ToString();

                Button button = newKeyBind.transform.Find("Slot1").GetComponent<Button>();
                button.onClick = new Button.ButtonClickedEvent();
                button.onClick.AddListener(delegate
                {
                    OptionsManager.Instance.dontUnpause = true;
                    __instance.StartCoroutine(UKAPI.KeyBindHandler.SetKeyBindRoutine(button.gameObject, keybind.bindName));
                    button.gameObject.GetComponent<Image>().color = new Color32(255, 103, 0, 255);
                });
                bindIndex++;

                Text keyText = button.gameObject.GetComponentInChildren<Text>();
                keybind.OnBindingChanged.AddListener(delegate (KeyCode newBind)
                {
                    if (keyText != null)
                        keyText.text = newBind.ToString().Trim();
                });
                controlContentRect.sizeDelta += new Vector2(0f, 20f); // setting the scrollbar to fit all of the binds
            });
        }
    }
}
