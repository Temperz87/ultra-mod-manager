using UnityEngine;
using BepInEx;
using HarmonyLib;

namespace UMM.Loader
{
    [BepInPlugin("UMM", "UMM", VersionHandler.VERSION)]
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
