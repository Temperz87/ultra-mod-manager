using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace UMM.Loader
{
    [BepInPlugin("UMM", "umm.mainManager", VersionHandler.VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private static bool initialized = false;
        private void Start()
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

        private void OnApplicationQuit()
        {
            UKAPI.SaveFileHandler.DumpFile();
        }
    }
}
