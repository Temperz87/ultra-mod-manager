using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UKMM.Loader;

namespace UKMM.HarmonyPatches
{
    [HarmonyPatch(typeof(FinalCyberRank), "GameOver")]
    public static class Ensure_NoSubmitBadScore
    {
        public static bool Prefix()
        {
            majorWasUsed = StatsManager.Instance.majorUsed;
            StatsManager.Instance.majorUsed = !UKModManager.AllowCyberGrindSubmission || majorWasUsed;
            return true;
        }
        public static void Postfix()
        {
            StatsManager.Instance.majorUsed = majorWasUsed;
        }
        private static bool majorWasUsed = false;
    }

    [HarmonyPatch(typeof(SteamController), "SubmitCyberGrindScore")]
    public static class Ensure_NoSubmitBadScoreRedundant // I am very paranoid
    {
        public static bool Prefix()
        {
            return UKModManager.AllowCyberGrindSubmission;
        }
    }
}