using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UKMM.Loader;

namespace UKMM.HarmonyPatches
{
    [HarmonyPatch(typeof(OptionsMenuToManager), "Start")]
    public static class Inject_ModsButton
    {
        public static void Prefix(OptionsMenuToManager __instance)
        {
            if (__instance.pauseMenu.name == "Main Menu (1)") // sanity check
            {
                __instance.pauseMenu.transform.Find("Panel").localPosition = new Vector3(0, 325, 0);
                GameObject newModsButton = GameObject.Instantiate(__instance.pauseMenu.transform.Find("Continue").gameObject, __instance.pauseMenu.transform);
                newModsButton.SetActive(false);
                newModsButton.transform.localPosition = new Vector3(0, -280, 0);
                newModsButton.GetComponentInChildren<Text>(true).text = "MODS";

                GameObject modsMenu = GameObject.Instantiate(__instance.optionsMenu, __instance.transform);
                modsMenu.SetActive(false);
                for (int i = 0; i < modsMenu.transform.childCount; i++)
                    modsMenu.transform.GetChild(i).gameObject.SetActive(false);

                Transform colorBlind = modsMenu.transform.Find("ColorBlindness Options");
                colorBlind.gameObject.SetActive(true);
                GameObject.Destroy(colorBlind.GetComponent<GamepadObjectSelector>()); // sorry gamepad players, but without this the mod manager breaks
                colorBlind.transform.Find("Text (1)").GetComponent<Text>().text = "--MODS--";
                Transform content = colorBlind.transform.Find("Scroll Rect").Find("Contents");
                content.Find("Default").gameObject.SetActive(false);
                content.Find("Enemies").gameObject.SetActive(false);
                content.gameObject.SetActive(true);
                GameObject template = content.Find("HUD").Find("Health").gameObject;
                content.Find("HUD").gameObject.SetActive(false);
                template.SetActive(false);


                UKModManager.ModInformation[] information = UKModManager.GetAllModInformation();
                Debug.Log("Got " + information.Length + " mods to place.");
                for (int i = 0; i < information.Length; i++)
                {
                    UKModManager.ModInformation info = information[i];
                    GameObject newInformation = GameObject.Instantiate(template, content);
                    GameObject.Destroy(newInformation.GetComponent<ColorBlindSetter>());

                    Button newButton = newInformation.AddComponent<Button>();
                    newButton.transition = Selectable.Transition.ColorTint;
                    newButton.targetGraphic = newInformation.GetComponent<Image>();
                    newButton.onClick = new Button.ButtonClickedEvent();
                    newButton.onClick.AddListener(delegate 
                    {
                        info.LoadThisMod();
                        newButton.targetGraphic.color = Color.green;
                    });

                    newInformation.transform.Find("Red").gameObject.SetActive(false);
                    newInformation.transform.Find("Green").gameObject.SetActive(false);
                    newInformation.transform.Find("Blue").gameObject.SetActive(false);
                    newInformation.transform.Find("Image").gameObject.SetActive(false);
                    newInformation.transform.localScale = new Vector3(1.64415f, 1.64415f, 1.64415f);
                    newInformation.transform.localPosition = new Vector3(0f, -200f * i, 0f);
                    newInformation.transform.Find("Text").GetComponent<Text>().text = info.modName;
                    newInformation.SetActive(true);
                }

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
