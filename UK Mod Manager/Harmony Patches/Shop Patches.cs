using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UKMM.HarmonyPatches
{
    [HarmonyPatch(typeof(ShopZone), "Start")]
    public static class Inject_SoundPackShops
    {
        public static void Prefix(ShopZone __instance)
        {
            return;
            if (__instance.transform.Find("Canvas").Find("Weapons") != null) // just a sanity check that we're not messing with a testament
            {
                // Grab parents and such
                Transform enemies = GameObject.Instantiate(__instance.transform.Find("Canvas").Find("Enemies"), __instance.transform.Find("Canvas").Find("Weapons"));
                enemies.Find("BackButton (2)").gameObject.SetActive(false);
                Text editingText = enemies.Find("Panel").Find("Title").GetComponent<Text>();
                editingText.text = "MODDED WEAPONS";
                editingText.transform.parent = enemies.parent;
                editingText.gameObject.SetActive(false);
                Transform contentParent = enemies.Find("Panel").Find("Scroll View").Find("Viewport").Find("Content");

                // Making the button that opens the sound packs page
                Transform newArmsButton = GameObject.Instantiate(__instance.transform.Find("Canvas").Find("Weapons").Find("ArmButton").gameObject, __instance.transform.Find("Canvas").Find("Weapons")).transform;
                newArmsButton.localPosition = new Vector3(-180f, -86f, -45.00326f);
                newArmsButton.gameObject.GetComponentInChildren<Text>(true).text = "MODDED WEAPONS";
                ShopButton button = newArmsButton.GetComponent<ShopButton>();
                button.toActivate = new GameObject[]
                {
                    enemies.gameObject,
                    contentParent.gameObject,
                    editingText.gameObject
                };
                button.toDeactivate = new GameObject[]
                {
                    __instance.transform.Find("Canvas").Find("Weapons").Find("RevolverWindow").gameObject,
                    __instance.transform.Find("Canvas").Find("Weapons").Find("ShotgunWindow").gameObject,
                    __instance.transform.Find("Canvas").Find("Weapons").Find("NailgunWindow").gameObject,
                    __instance.transform.Find("Canvas").Find("Weapons").Find("RailcannonWindow").gameObject,
                    __instance.transform.Find("Canvas").Find("Weapons").Find("ArmWindow").gameObject,
                };
                newArmsButton.GetComponent<Button>().onClick.AddListener(delegate
                {
                    editingText.text = "MODDED WEAPONS";
                });

                // Grabbing buttons
                Transform revButton = __instance.transform.Find("Canvas").Find("Weapons").Find("RevolverButton");
                ShopButton revShopButton = revButton.gameObject.GetComponent<ShopButton>();
                Transform sgButton = __instance.transform.Find("Canvas").Find("Weapons").Find("ShotgunButton");
                ShopButton sgShopButton = sgButton.gameObject.GetComponent<ShopButton>();
                Transform ngButton = __instance.transform.Find("Canvas").Find("Weapons").Find("NailgunButton");
                ShopButton ngShopButton = ngButton.gameObject.GetComponent<ShopButton>();
                Transform rcButton = __instance.transform.Find("Canvas").Find("Weapons").Find("RailcannonButton");
                ShopButton rcShopButton = rcButton.gameObject.GetComponent<ShopButton>();
                ShopButton armShopButton = __instance.transform.Find("Canvas").Find("Weapons").Find("ArmButton").gameObject.GetComponent<ShopButton>();
                ShopButton backShopButton = __instance.transform.Find("Canvas").Find("Weapons").Find("BackButton (1)").gameObject.GetComponent<ShopButton>();
                GameObject newPageParent = GameObject.Instantiate(new GameObject(), revButton.parent);
                newPageParent.SetActive(false);
                newPageParent.transform.localPosition = new Vector3(90f, 0, 0);

                // Messing with preexisting buttons to close the custom pages
                revShopButton.toDeactivate = revShopButton.toDeactivate.AddToArray(newPageParent);
                revShopButton.toDeactivate = revShopButton.toDeactivate.AddToArray(editingText.gameObject);
                revShopButton.toDeactivate = revShopButton.toDeactivate.AddToArray(enemies.gameObject);

                sgShopButton.toDeactivate = sgShopButton.toDeactivate.AddToArray(newPageParent);
                sgShopButton.toDeactivate = sgShopButton.toDeactivate.AddToArray(editingText.gameObject);
                sgShopButton.toDeactivate = sgShopButton.toDeactivate.AddToArray(enemies.gameObject);

                ngShopButton.toDeactivate = ngShopButton.toDeactivate.AddToArray(newPageParent);
                ngShopButton.toDeactivate = ngShopButton.toDeactivate.AddToArray(editingText.gameObject);
                ngShopButton.toDeactivate = ngShopButton.toDeactivate.AddToArray(enemies.gameObject);

                rcShopButton.toDeactivate = rcShopButton.toDeactivate.AddToArray(newPageParent);
                rcShopButton.toDeactivate = rcShopButton.toDeactivate.AddToArray(editingText.gameObject);
                rcShopButton.toDeactivate = rcShopButton.toDeactivate.AddToArray(enemies.gameObject);

                armShopButton.toDeactivate = armShopButton.toDeactivate.AddToArray(newPageParent);
                armShopButton.toDeactivate = armShopButton.toDeactivate.AddToArray(editingText.gameObject);
                armShopButton.toDeactivate = armShopButton.toDeactivate.AddToArray(enemies.gameObject);

                backShopButton.toDeactivate = backShopButton.GetComponent<ShopButton>().toDeactivate.AddToArray(newPageParent);
                backShopButton.toDeactivate = backShopButton.toDeactivate.AddToArray(editingText.gameObject);
                backShopButton.toDeactivate = backShopButton.toDeactivate.AddToArray(enemies.gameObject);

                Transform newBack = GameObject.Instantiate(rcButton, newPageParent.transform);
                newBack.localPosition = new Vector3(0f, -85f, -45f);
                newBack.localScale = new Vector3(0.74757f, 0.74757f, 0.74757f);
                newBack.GetComponent<ShopButton>().toActivate = new GameObject[] { contentParent.gameObject };
                newBack.GetComponent<ShopButton>().toDeactivate = new GameObject[] { newPageParent };
                newBack.gameObject.GetComponentInChildren<Text>().text = "BACK";
                newBack.GetComponent<Button>().onClick.AddListener(delegate
                {
                    editingText.text = "MODDED WEAPONS";
                });

                // hiding any existing content
                GameObject packTemplate = contentParent.Find("Enemy Button Template").gameObject;
                for (int i = 0; i < contentParent.childCount; i++)
                    contentParent.GetChild(i).gameObject.SetActive(false);
                packTemplate.gameObject.SetActive(true);

                /*
                foreach (UKAPI.WeaponInformation info in UKAPI.RetrieveAllWeapons())
                {
                    GameObject newPack = GameObject.Instantiate(packTemplate, contentParent);
                    newPack.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = Color.gray;
                    if (info.icon != null && info.icon)
                    {
                        Image img = newPack.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<Image>();
                        IEnumerator setImageNextFrame() // So unity waits a frame before destroying a component, hence this
                        {
                            yield return null;
                            img.sprite = info.icon.weaponIcon;
                            img.raycastTarget = false;
                        }
                        __instance.StartCoroutine(setImageNextFrame());
                    }
                    else
                        newPack.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
                    ShopButton sButton = newPack.GetComponent<ShopButton>();
                    sButton.toActivate = new GameObject[] { };
                    newPack.GetComponent<ShopButton>().toDeactivate = new GameObject[] { };
                    newPack.GetComponent<Button>().onClick.AddListener(delegate
                    {
                        contentParent.gameObject.SetActive(false);
                    });

                    GameObject newText = GameObject.Instantiate(__instance.transform.Find("Canvas").Find("Weapons").Find("ArmButton").gameObject, newPack.transform.GetChild(0).GetChild(0)); // yes I tried making a "prefab" out of this and instantiating it, didn't work
                    GameObject.Destroy(newText.GetComponent<ShopButton>());
                    newText.transform.localPosition = Vector3.zero;
                    newText.transform.localScale = new Vector3(0.4167529f, 0.4167529f, 0.4167529f);
                    newText.layer = 5;
                    Text text = newText.GetComponentInChildren<Text>();
                    text.text = info.weaponPrefab.name;
                    text.raycastTarget = false;
                }
                */
            }   
        }
    }
}