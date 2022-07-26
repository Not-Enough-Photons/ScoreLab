using System.Collections.Generic;
using System.Reflection;

using System.Runtime.InteropServices;

using UnityEngine;

using PuppetMasta;

using StressLevelZero.AI;
using StressLevelZero.Arena;
using StressLevelZero.Combat;
using StressLevelZero.Rig;

namespace NEP.Scoreworks.Core
{
    public class Director 
    {
        public Director()
        {
            Initialize();
        }

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

                    if (playerInAir)
                    {
                        new Data.SWValue(Data.SWScoreType.SW_SCORE_MIDAIR_KILL);
                    }
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

        public static Director instance;

        public static bool playerInAir;
        public static int enemiesKilled;

        public static float t_enemiesKilled;

        private static RigManager playerRig;
        private static PhysicsRig physicsRig;

        private static float t_maxEnemiesKilled = 1f;

        private RigManager GetPlayerRig()
        {
            return ModThatIsNotMod.Player.GetRigManager().GetComponent<RigManager>();
        }

        private void Initialize()
        {
            playerRig = GetPlayerRig();
            physicsRig = playerRig.physicsRig;

            Utilities.Utils.AttackPatch.OnAttackRecieved += OnAttackRecieved;
        }

        private void OnAttackRecieved(Attack attack)
        {
            AIBrain brain = attack.collider.GetComponentInParent<AIBrain>();

            if (brain)
            {
                if(brain.behaviour.health.cur_hp <= 0f)
                {
                    return;
                }
            }

            if (attack.attackType == AttackType.Piercing)
            {
                if (attack.collider.name == "Head_M" || attack.collider.name == "Neck_M" || attack.collider.name == "Jaw_M")
                {
                    new Data.SWValue(Data.SWScoreType.SW_SCORE_HEADSHOT);
                    new Data.SWValue(Data.SWMultiplierType.SW_MULTIPLIER_HEADSHOT);
                }
            }
        }

        public static void Update()
        {
            playerInAir = !physicsRig.physBody.physG.isGrounded;

            EnemiesKilledUpdate();
        }

        private static void EnemiesKilledUpdate()
        {
            if(enemiesKilled < 1)
            {
                return;
            }

            t_enemiesKilled += Time.deltaTime;

            if(t_enemiesKilled > t_maxEnemiesKilled)
            {
                enemiesKilled = 0;
                t_enemiesKilled = 0f;
            }
        }
    }
}
