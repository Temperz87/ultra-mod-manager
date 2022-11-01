using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace UMM.Loader
{
    [BepInPlugin("UMM", "UMM", VersionHandler.VERSION)]
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
