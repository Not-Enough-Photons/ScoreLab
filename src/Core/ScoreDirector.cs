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
                    ScoreTracker.Instance.Add(EventType.Mult.Seated);
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

            [HarmonyLib.HarmonyPatch(typeof(Player_Health))]
            [HarmonyLib.HarmonyPatch(nameof(Player_Health.LifeSavingDamgeDealt))]
            public static class SecondWindPatch
            {
                public static void Postfix()
                {
                    ScoreTracker.Instance.Add(EventType.Mult.SecondWind);
                }
            }

            [HarmonyLib.HarmonyPatch(typeof(Arena_GameController))]
            [HarmonyLib.HarmonyPatch(nameof(Arena_GameController.EndOfRound))]
            public static class EndOfRoundPatch
            {
                public static void Postfix()
                {
                    ScoreTracker.Instance.Add(EventType.Score.GameRoundCompleted);
                }
            }

            [HarmonyLib.HarmonyPatch(typeof(PhysicsRig))]
            [HarmonyLib.HarmonyPatch(nameof(PhysicsRig.OnUpdate))]
            public static class PhysRigPatch
            {
                private static bool _targetBool;
                private static float _tMidAirDelay = 0.5f;
                private static float _tTime;

                public static void Postfix(PhysicsRig __instance)
                {
                    IsPlayerInAir = !__instance.physG.isGrounded;

                    if (IsPlayerInAir)
                    {
                        if (!_targetBool)
                        {
                            _tTime += UnityEngine.Time.deltaTime;

                            if(_tTime > _tMidAirDelay)
                            {
                                ScoreTracker.Instance.Add(EventType.Mult.MidAir);
                                _targetBool = true;
                            }
                        }
                    }
                    else
                    {
                        _tTime = 0f;
                        _targetBool = false;
                    }
                }
            }

            public static void InitPatches()
            {
                Hooking.OnNPCKillStart += OnAIDeath;
            }

            public static void OnAIDeath(BehaviourBaseNav behaviour)
            {
                if(!behaviour.sensors.isGrounded)
                {
                    ScoreTracker.Instance.Add(EventType.Score.EnemyMidAirKill);
                }

                ScoreTracker.Instance.Add(EventType.Score.Kill);
                ScoreTracker.Instance.Add(EventType.Mult.Kill);
            }
        }

        public static bool IsPlayerMoving = false;
        public static bool IsPlayerInAir = false;
        public static bool IsPlayerSeated = false;
    }
}
