using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace UMM.Loader
{
    [BepInPlugin("UMM", "umm.mainManager", VersionHandler.VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private static bool initialized = false;

        internal static ManualLogSource logger;

        private void Start()
        {
            if (!initialized)
            {
                logger = Logger;
                logger.LogMessage("UMM initializing!");
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
