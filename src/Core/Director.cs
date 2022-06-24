using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

using PuppetMasta;

namespace NEP.Scoreworks.Core
{
    public class Director 
    {
        public class Patches
        {
            [HarmonyLib.HarmonyPatch(typeof(BehaviourBaseNav))]
            [HarmonyLib.HarmonyPatch(nameof(BehaviourBaseNav.KillStart))]
            public static class Patch_KillStart
            {
                public static void Postfix()
                {
                    new Data.SWValue(Data.SWScoreType.SW_SCORE_KILL);
                }
            }
        }
    }
}
