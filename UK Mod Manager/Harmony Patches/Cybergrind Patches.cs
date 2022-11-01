using HarmonyLib;
using UnityEngine;

namespace UMM.HarmonyPatches
{
    [HarmonyPatch(typeof(FinalCyberRank), "GameOver")]
    internal static class Ensure_NoSubmitBadScore
    {
        private static bool Prefix()
        {
            bool flag = UKAPI.CanSubmitCybergrindScore;
            if (!flag)
                StatsManager.Instance.majorUsed = true;
            Debug.Log("Should submit Cybergrind score is " + flag);
            return true;
        }
    }

    [HarmonyPatch(typeof(SteamController), "SubmitCyberGrindScore")]
    internal static class Ensure_NoSubmitBadScoreRedundant // I am very paranoid
    {
        private static bool Prefix()
        {
            bool flag = UKAPI.CanSubmitCybergrindScore;
            Debug.Log("Should submit cybergrind score is " + flag);
            return flag;
        }
    }
}