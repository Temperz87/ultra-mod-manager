using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UKMM.Loader;
using UnityEngine.EventSystems;
using System.Collections;

namespace UKMM.HarmonyPatches
{
    [HarmonyPatch(typeof(OptionsMenuToManager), "Start")]
    public static class Inject_ModsButton
    {
        public static void Prefix(OptionsMenuToManager __instance)
        {
            if (__instance.pauseMenu.name == "Main Menu (1)") // check to see that we're patching out the main menu's menu, not like an in game menu one
            {
                bool wasOn = MonoSingleton<PrefsManager>.Instance.GetBool("variationMemory", false);
                __instance.pauseMenu.transform.Find("Panel").localPosition = new Vector3(0, 325, 0); 
                GameObject newModsButton = GameObject.Instantiate(__instance.pauseMenu.transform.Find("Continue").gameObject, __instance.pauseMenu.transform, true);
                newModsButton.SetActive(false);
                newModsButton.transform.localPosition = new Vector3(0, -260, 0);
                newModsButton.GetComponentInChildren<Text>(true).text = "MODS";

                Traverse effect = Traverse.Create(newModsButton.GetComponent<HudOpenEffect>());
                effect.Field("originalWidth").SetValue(1f);
                effect.Field("originalHeight").SetValue(1f);
                
                Transform panel = __instance.pauseMenu.transform.Find("Panel");
                GameObject discordButton = panel.Find("Discord").gameObject;
                discordButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-164f, -350f);
                discordButton.transform.parent.Find("Twitter").GetComponent<RectTransform>().anchoredPosition = new Vector2(-329f, -405.5f);
                discordButton.transform.parent.Find("Youtube").GetComponent<RectTransform>().anchoredPosition = new Vector2(-494f, -461f);

                GameObject gitButton = GameObject.Instantiate(discordButton, discordButton.transform.parent);
                gitButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(492f, -350f);
                gitButton.GetComponentInChildren<Text>().text = "UKMM SOURCE";
                gitButton.GetComponentInChildren<Image>().color = new Color32(191, 191, 191, 255);
                gitButton.GetComponentInChildren<WebButton>().url = "https://github.com/Temperz87/UK-Mod-Manager";

                GameObject moreModsButton = GameObject.Instantiate(discordButton, discordButton.transform.parent);
                moreModsButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(492f, -405.5f);
                moreModsButton.GetComponentInChildren<Text>().text = "BROWSE MODS";
                moreModsButton.GetComponentInChildren<Image>().color = new Color32(211, 218, 114, 255);
                moreModsButton.GetComponentInChildren<WebButton>().url = "https://docs.google.com/spreadsheets/d/1x8P3GcdfWraZX1kz3bbHJIiY4hozxe8k1oieOm_fuL0/edit?usp=sharing";

                GameObject modsMenu = GameObject.Instantiate(__instance.optionsMenu, __instance.transform);
                modsMenu.SetActive(false);
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

                GameObject hoverText = content.Find("Default").gameObject;
                hoverText.transform.parent = modsMenu.transform;
                hoverText.GetComponentInChildren<Text>().text = "Toggles auto loading on game start";
                hoverText.transform.localPosition -= new Vector3(0f, 520f, 0f);
                GameObject.Destroy(hoverText.GetComponent<BackSelectOverride>());
                GameObject.Destroy(hoverText.GetComponent<Button>());
                hoverText.SetActive(false);

                ModInformation[] information = UKAPI.GetAllModInformation();
                if (information.Length > 0)
                {
                    Array.Sort(information);
                    for (int i = 0; i < information.Length; i++)
                    {
                        ModInformation info = information[i];
                        GameObject newInformation = GameObject.Instantiate(template, content);
                        GameObject.Destroy(newInformation.GetComponent<ColorBlindSetter>());

                        Button newButton = newInformation.AddComponent<Button>();
                        newButton.transition = Selectable.Transition.ColorTint;
                        newButton.targetGraphic = newInformation.GetComponent<Image>();
                        newButton.targetGraphic.color = info.loaded ? Color.green : Color.red;
                        newButton.onClick = new Button.ButtonClickedEvent();
                        newButton.onClick.AddListener(delegate
                        {
                            info.Clicked();
                            newButton.targetGraphic.color = info.loaded ? Color.green : Color.red;
                        });

                        newInformation.transform.Find("Red").gameObject.SetActive(false);
                        newInformation.transform.Find("Green").gameObject.SetActive(false);
                        newInformation.transform.Find("Blue").gameObject.SetActive(false);
                        newInformation.transform.Find("Image").gameObject.SetActive(false);
                        newInformation.transform.localScale = new Vector3(1.64415f, 1.64415f, 1.64415f);
                        newInformation.transform.localPosition = new Vector3(0f, -200f * i, 0f);

                        Text modText = newInformation.transform.Find("Text").GetComponent<Text>();
                        modText.text = info.modName + " " + info.modVersion;
                        modText.alignment = TextAnchor.UpperLeft;
                        modText.transform.localPosition = new Vector3(-49.2f, 0f, 0f);
                        modText.transform.localScale = new Vector3(0.66764f, 0.66764f, 0.66764f);

                        Text descriptionText = GameObject.Instantiate(modText.gameObject, modText.transform.parent).GetComponent<Text>();
                        descriptionText.alignment = TextAnchor.UpperLeft;
                        descriptionText.rectTransform.offsetMin = new Vector2(-73.58125f, -170f);
                        descriptionText.rectTransform.offsetMax = new Vector2(73.58125f, -16.4f);
                        descriptionText.transform.localScale = new Vector3(0.66764f, 0.66764f, 0.66764f);
                        descriptionText.fontSize = 16;
                        descriptionText.text = info.modDescription;

                        GameObject toggleObj = GameObject.Instantiate(__instance.variationMemory.gameObject, newInformation.transform);
                        toggleObj.transform.localPosition = new Vector3(247f, -9f, 0f);
                        Toggle toggle = toggleObj.GetComponent<Toggle>();
                        toggle.isOn = info.loadOnStart;
                        toggle.onValueChanged = new Toggle.ToggleEvent();
                        toggle.onValueChanged.AddListener(delegate
                        {
                            info.loadOnStart = !info.loadOnStart;
                            toggle.isOn = info.loadOnStart;
                            UKAPI.SaveFileHandler.SetModData(info.modName, "LoadOnStart", info.loadOnStart.ToString());
                        });
                        EventTrigger trigger = toggle.gameObject.AddComponent<EventTrigger>();
                        EventTrigger.Entry hoverEntry = new EventTrigger.Entry();
                        hoverEntry.eventID = EventTriggerType.PointerEnter;
                        hoverEntry.callback.AddListener(delegate
                        {
                            hoverText.SetActive(true);
                        });
                        EventTrigger.Entry unHoverEntry = new EventTrigger.Entry();
                        unHoverEntry.eventID = EventTriggerType.PointerExit;
                        unHoverEntry.callback.AddListener(delegate
                        {
                            hoverText.SetActive(false);
                        });
                        trigger.triggers.Add(hoverEntry);
                        trigger.triggers.Add(unHoverEntry);

                        toggleObj.SetActive(true);
                        newInformation.SetActive(true);
                    }

                    cRect.sizeDelta = new Vector2(600f, information.Length * 200); // setting the scrollbar fit all of the mods
                }
                else
                {
                    content.gameObject.SetActive(false);
                    hoverText.SetActive(true);
                    hoverText.transform.localPosition += new Vector3(0f, 260f, 0f);
                    hoverText.GetComponentInChildren<Text>().text = "NO MODS FOUND";
                }

                __instance.variationMemory.gameObject.SetActive(true);
                MonoSingleton<PrefsManager>.Instance.SetBool("variationMemory", wasOn); // there's a bug where this patch sets variation memory to always be on once you get to the menu fo romse reason, this fixes that

                Button.ButtonClickedEvent modsButton = newModsButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
                modsButton.AddListener(delegate
                {
                    __instance.CheckIfTutorialBeaten();
                    __instance.pauseMenu.SetActive(false);
                    modsMenu.SetActive(true);
                });

                newModsButton.SetActive(true);
            }
        }
    }
}
