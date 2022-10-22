using BoneLib;
using NEP.ScoreLab.Data;
using PuppetMasta;
using System.Reflection;

using SLZ.Rig;
using SLZ.Vehicle;

namespace NEP.ScoreLab.Core
{
    public static class ScoreDirector
    {
        public static class Patches
        {
            [HarmonyLib.HarmonyPatch(typeof(Seat))]
            [HarmonyLib.HarmonyPatch(nameof(Seat.Register))]
            public static class SeatPatch
            {
                public static void Postfix(RigManager rM)
                {
                    IsPlayerSeated = true;
                    ScoreTracker.Instance.Add(new PackedMultiplier("SEATED", 1.25f, API.GameConditions.IsPlayerSeated));
                }
            }

            [HarmonyLib.HarmonyPatch(typeof(Seat))]
            [HarmonyLib.HarmonyPatch(nameof(Seat.DeRegister))]
            public static class SeatPatch2
            {
                public static void Postfix()
                {
                    IsPlayerSeated = false;
                }
            }

            public static void InitPatches()
            {
                Hooking.OnNPCKillStart += OnAIDeath;
            }

            public static void OnAIDeath(BehaviourBaseNav behaviour)
            {
                ScoreTracker.Instance.Add(new PackedScore("KILL", 200));
            }
        }

        public static bool IsPlayerMoving = false;
        public static bool IsPlayerInAir = false;
        public static bool IsPlayerSeated = false;
    }
}
