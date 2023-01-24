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

        internal const string UKMOD_DEPRECATION_MESSAGE = "The UKMod system has been deprecated. Learn more: <insert link here>";

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

        public void Update()
        {
            UKAPI.Update();
        }

        private void OnApplicationQuit()
        {
            UKAPI.SaveFileHandler.DumpFile();
        }
    }
}
