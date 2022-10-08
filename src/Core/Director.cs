﻿using System.Collections.Generic;
using System.Reflection;

using System.Runtime.InteropServices;

using UnityEngine;

using PuppetMasta;

using SLZ.AI;
using SLZ.Arena;
using SLZ.Combat;
using SLZ.Interaction;
using SLZ.Rig;
using SLZ.Props.Weapons;

namespace NEP.ScoreLab.Core
{
    public class Director 
    {
        public Director()
        {
            Initialize();
        }

        public class Patches
        {
            [HarmonyLib.HarmonyPatch(typeof(Arena_GameController))]
            [HarmonyLib.HarmonyPatch(nameof(Arena_GameController.Start))]
            public static class Patch_StartWave
            {
                public static void Postfix(Arena_GameController __instance)
                {
                    var action = new System.Action(() =>
                    {
                        if(__instance.wave > 1)
                        {
                            return;
                        }

                        new Data.SLValue(Data.SLScoreType.SL_SCORE_FINISH_ARENA_WAVE);
                    });

                    __instance.onWaveBegin.AddListener(action);
                }
            }

            [HarmonyLib.HarmonyPatch(typeof(BehaviourBaseNav))]
            [HarmonyLib.HarmonyPatch(nameof(BehaviourBaseNav.KillStart))]
            public static class Patch_KillStart
            {
                public static void Postfix()
                {
                    // stops scoreworks from finding that enemy that died on the first frame
                    // of which the level was loaded
                    if(Time.timeSinceLevelLoad > Mathf.Epsilon)
                    {
                        new Data.SLValue(Data.SLScoreType.SL_SCORE_KILL);
                        new Data.SLValue(Data.SLMultiplierType.SL_MULTIPLIER_KILL);

                        if (playerInAir)
                        {
                            new Data.SLValue(Data.SLScoreType.SL_SCORE_MIDAIR_KILL);
                        }
                    }
                }
            }

            [HarmonyLib.HarmonyPatch(typeof(Hand))]
            [HarmonyLib.HarmonyPatch(nameof(Hand.AttachObject))]
            public static class Patch_FindNimbus
            {
                public static void Postfix(GameObject objectToAttach)
                {
                    if(objectToAttach == null)
                    {
                        return;
                    }

                    nimbusGun = objectToAttach.GetComponentInParent<FlyingGun>();
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
                        new Data.SLValue(Data.SLScoreType.SL_SCORE_CRABCEST);
                    }
                    else
                    {
                        new Data.SLValue(Data.SLScoreType.SL_SCORE_ATTACH_CRABLET);
                    }
                }
            }

            [HarmonyLib.HarmonyPatch(typeof(Player_Health))]
            [HarmonyLib.HarmonyPatch(nameof(Player_Health.LifeSavingDamgeDealt))]
            public static class Patch_SecondWind
            {
                public static void Postfix()
                {
                    new Data.SLValue(Data.SLMultiplierType.SL_MULTIPLIER_SECOND_WIND);
                }
            }

            [HarmonyLib.HarmonyPatch(typeof(Projectile))]
            [HarmonyLib.HarmonyPatch(nameof(Projectile.Awake))]
            public static class Patch_ProjectileChecks
            {
                public static void Postfix(Projectile __instance)
                {
                    var action = new System.Action<Collider, Vector3, Vector3>((collider, world, normal) =>
                    {
                        var hitProjectile = collider.GetComponent<RigidbodyProjectile>();

                        if (hitProjectile)
                        {
                            Vector3 projectilePos = hitProjectile.transform.position;
                            Vector3 playerPos = GetPlayerRig().physicsRig.m_head.position;

                            if(Vector3.Distance(projectilePos, playerPos) < 0.5f)
                            {
                                new Data.SLValue(Data.SLScoreType.SL_SCORE_CLOSE_CALL);
                            }
                        }
                    });

                    __instance.onCollision.AddListener(action);
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

        private static FlyingGun nimbusGun;

        private static RigManager GetPlayerRig()
        {
            return GameObject.FindObjectOfType<RigManager>();
        }

        private void Initialize()
        {
            playerRig = GetPlayerRig();
            physicsRig = playerRig.physicsRig;

            Utilities.Utils.ImpactPropertiesPatch.OnAttackRecieved += ImpactPropertiesAttack;
            Utilities.Utils.RigidbodyProjectilePatch.OnAttackRecieved += RigidbodyProjectileAttack;
        }

        private void ImpactPropertiesAttack(Attack attack)
        {
            AIBrain brain = attack.collider.GetComponentInParent<AIBrain>();

            if (brain)
            {
                if(brain.behaviour.health.cur_hp <= 0f)
                {
                    return;
                }
            }

            if (attack.attackType == SLZ.Marrow.Data.AttackType.Piercing)
            {
                if (attack.collider.name == "Head_M" || attack.collider.name == "Neck_M" || attack.collider.name == "Jaw_M")
                {
                    if(brain.behaviour.health.cur_hp <= attack.damage)
                    {
                        new Data.SLValue(Data.SLScoreType.SL_SCORE_HEADSHOT);
                        new Data.SLValue(Data.SLMultiplierType.SL_MULTIPLIER_HEADSHOT);
                    }
                }
            }
        }

        private void RigidbodyProjectileAttack(Attack attack)
        {
            if(attack.attackType == SLZ.Marrow.Data.AttackType.Piercing)
            {
                Vector3 playerPos = GetPlayerRig().physicsRig.m_head.position;

                if(Vector3.Distance(attack.origin, playerPos) < 2f)
                {
                    new Data.SLValue(Data.SLScoreType.SL_SCORE_CLOSE_CALL);
                }
            }
        }

        public static void Update()
        {
            EnemiesKilledUpdate();

            playerInAir = !physicsRig.physG.isGrounded;
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
