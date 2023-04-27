using HarmonyLib;
using UMM.Loader;
using UnityEngine;

namespace UMM.HarmonyPatches
{
    [HarmonyPatch(typeof(FinalCyberRank), nameof(FinalCyberRank.GameOver))]
    internal static class Ensure_NoSubmitBadScore
    {
        private static bool Prefix()
        {
            bool flag = UKAPI.CanSubmitCybergrindScore;
            if (!flag)
                StatsManager.Instance.majorUsed = true;
            Plugin.logger.LogDebug("Should submit Cybergrind score is " + flag);
            return true;
        }
    }

    [HarmonyPatch(typeof(LeaderboardController), nameof(LeaderboardController.SubmitCyberGrindScore))]
    internal static class Ensure_NoSubmitBadScoreRedundant // I am very paranoid
    {
        private static bool Prefix()
        {
            bool flag = UKAPI.CanSubmitCybergrindScore;
            Plugin.logger.LogDebug("Should submit cybergrind score is " + flag);
            return flag;
        }
    }
}