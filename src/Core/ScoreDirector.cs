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
            public static class RegisterSeatPatch
            {
                public static void Postfix(RigManager rM)
                {
                    IsPlayerSeated = true;
                    ScoreTracker.Instance.Add(EventType.Mult.Test);
                }
            }

            [HarmonyLib.HarmonyPatch(typeof(Seat))]
            [HarmonyLib.HarmonyPatch(nameof(Seat.DeRegister))]
            public static class DeRegisterSeatPatch
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
                ScoreTracker.Instance.Add(EventType.Score.Kill);
                ScoreTracker.Instance.Add(EventType.Mult.Kill);
            }
        }

        public static bool IsPlayerMoving = false;
        public static bool IsPlayerInAir = false;
        public static bool IsPlayerSeated = false;
    }
}
