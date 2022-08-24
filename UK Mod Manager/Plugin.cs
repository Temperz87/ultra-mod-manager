using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using BepInEx;
using HarmonyLib;
using Newtonsoft.Json.Linq;

namespace UMM.Loader
{
    [BepInPlugin("UMM", "UMM", VersionHandler.versionString)]
    public class Plugin : BaseUnityPlugin
    {
        private static bool initialized = false;
        public void Start()
        {
            if (!initialized)
            {
                Debug.Log("UMM initializing!");
                new Harmony("umm.mainManager").PatchAll();
                StartCoroutine(UKAPI.InitializeAPI());
                StartCoroutine(VersionHandler.CheckVersion());
                initialized = true;
            }
        }

        public void OnApplicationQuit()
        {
            UKAPI.SaveFileHandler.DumpFile();
        }
    }
}
