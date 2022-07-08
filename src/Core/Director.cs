using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

using PuppetMasta;

using StressLevelZero.AI;
using StressLevelZero.Arena;
using StressLevelZero.Combat;

namespace NEP.Scoreworks.Core
{
    public class Director 
    {
        public class Patches
        {
            [HarmonyLib.HarmonyPatch(typeof(Arena_GameManager))]
            [HarmonyLib.HarmonyPatch(nameof(Arena_GameManager.StartWave))]
            public static class Patch_StartWave
            {
                public static void Postfix(Arena_GameManager __instance)
                {
                    if(__instance.wave <= 1)
                    {
                        return;
                    }

                    new Data.SWValue(Data.SWScoreType.SW_SCORE_FINISH_ARENA_WAVE);
                }
            }

            [HarmonyLib.HarmonyPatch(typeof(BehaviourBaseNav))]
            [HarmonyLib.HarmonyPatch(nameof(BehaviourBaseNav.KillStart))]
            public static class Patch_KillStart
            {
                public static void Postfix()
                {
                    new Data.SWValue(Data.SWScoreType.SW_SCORE_KILL);
                    new Data.SWValue(Data.SWMultiplierType.SW_MULTIPLIER_KILL);
                }
            }

            [HarmonyLib.HarmonyPatch(typeof(BehaviourCrablet))]
            [HarmonyLib.HarmonyPatch(nameof(BehaviourCrablet.AttachToFace))]
            public static class Patch_AttachToFace
            {
                public static void Postfix(Rigidbody face, TriggerRefProxy trp, bool preAttach = false, bool isPlayer = true)
                {
                    if(face == null || isPlayer)
                    {
                        return;
                    }

                    if(trp.npcType == TriggerRefProxy.NpcType.Crablet)
                    {
                        new Data.SWValue(Data.SWScoreType.SW_SCORE_CRABCEST);
                    }
                    else
                    {
                        new Data.SWValue(Data.SWScoreType.SW_SCORE_ATTACH_CRABLET);
                    }
                }
            }

            [HarmonyLib.HarmonyPatch(typeof(Player_Health))]
            [HarmonyLib.HarmonyPatch(nameof(Player_Health.LifeSavingDamgeDealt))]
            public static class Patch_SecondWind
            {
                public static void Postfix()
                {
                    new Data.SWValue(Data.SWMultiplierType.SW_MULTIPLIER_SECOND_WIND);
                }
            }

            [HarmonyLib.HarmonyPatch(typeof(Projectile))]
            [HarmonyLib.HarmonyPatch(nameof(Projectile.Awake))]
            public static class Patch_ProjectileCollision
            {
                public static void Postfix(Projectile __instance)
                {
                    var action = new System.Action<Collider, Vector3, Vector3>((collider, world, normal) =>
                    {
                        BehaviourBaseNav baseNav = collider.transform.root.GetComponentInParent<BehaviourBaseNav>();

                        if(baseNav == null)
                        {
                            return;
                        }

                        MelonLoader.MelonLogger.Msg("BLAH BLAH REACHED BASE NAV");

                        if(baseNav.health.cur_hp > 0f)
                        {
                            if (collider.attachedRigidbody.name == "Head_M"
                            || collider.attachedRigidbody.name == "Jaw_M"
                            || collider.attachedRigidbody.name == "Neck_01")
                            {
                                new Data.SWValue(Data.SWMultiplierType.SW_MULTIPLIER_HEADSHOT);
                            }
                        }
                    });

                    __instance.onCollision.AddListener(action);
                }
            }

            [HarmonyLib.HarmonyPatch(typeof(Zombie_GameControl))]
            [HarmonyLib.HarmonyPatch(nameof(Zombie_GameControl.StartNextWave))]
            public static class Patch_StartNextWave
            {
                public static void Postfix(Zombie_GameControl __instance)
                {
                    if(__instance.currWaveIndex > 1)
                    {
                        switch (__instance.difficulty)
                        {
                            case Zombie_GameControl.Difficulty.EASY:
                                new Data.SWValue(Data.SWScoreType.SW_SCORE_ROUND_EASY);
                                break;
                            case Zombie_GameControl.Difficulty.MEDIUM:
                                new Data.SWValue(Data.SWScoreType.SW_SCORE_ROUND_MEDIUM);
                                break;
                            case Zombie_GameControl.Difficulty.HARD:
                                new Data.SWValue(Data.SWScoreType.SW_SCORE_ROUND_HARD);
                                break;
                            case Zombie_GameControl.Difficulty.HARDER:
                                new Data.SWValue(Data.SWScoreType.SW_SCORE_ROUND_HARDER);
                                break;
                            case Zombie_GameControl.Difficulty.HARDEST:
                                new Data.SWValue(Data.SWScoreType.SW_SCORE_ROUND_HARDEST);
                                break;
                        }
                    }
                }
            }
        }
    }
}
