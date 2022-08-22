using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using BepInEx;
using HarmonyLib;
using Newtonsoft.Json.Linq;

namespace UMM.Loader
{
    [BepInPlugin("UMM", "UMM", versionString)]
    public class Plugin : BaseUnityPlugin
    {
        private const string versionString = "0.4.0"; // Should this be hardcoded? No it should not be
        private static bool initialized = false;
        public void Start()
        {
            if (!initialized)
            {
                Debug.Log("UMM initializing!");
                new Harmony("umm.mainManager").PatchAll();
                StartCoroutine(UKAPI.InitializeAPI());
                StartCoroutine(CheckVersion());
                initialized = true;
            }
        }
        private IEnumerator CheckVersion()
        {
            Debug.Log("UMM: Trying to get verison.");
            using (UnityWebRequest www = UnityWebRequest.Get("https://api.github.com/repos/Temperz87/ultra-mod-manager/tags"))
            {
                yield return www.SendWebRequest();
                if (www == null)
                {
                    Debug.Log("UMM: WWW was null when checking version!");
                    yield break;
                }
                if (www.isNetworkError)
                {
                    Debug.Log("UMM: Couldn't get the version from url, " + www.error);
                    yield break;
                }
                string text = www.downloadHandler.text;
                Debug.Log(text);
                JArray jObjects = JArray.Parse(text);
                string latestVersion = jObjects[0].Value<string>("name");
                if (latestVersion != versionString)
                {
                    Debug.Log("UMM: New version found: " + latestVersion + " while the current version is " + versionString);
                    UltraModManager.outdated = true;
                    UltraModManager.newLoaderVersion = latestVersion;
                }
                yield break;
            }
        }

        public void OnApplicationQuit()
        {
            UKAPI.SaveFileHandler.DumpFile();
        }
    }
}
