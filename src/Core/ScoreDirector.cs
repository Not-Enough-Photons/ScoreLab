using UnityEngine;

using BoneLib;
using Il2CppPuppetMasta;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Combat;
using Il2CppSLZ.Marrow.PuppetMasta;
using Il2CppSLZ.Marrow.AI;
using Il2CppSLZ.Marrow.Data;
using Il2CppSLZ.Marrow.Interaction;
using NEP.NEDebug;
using NEP.ScoreLab.Data;

using Avatar = Il2CppSLZ.VRMK.Avatar;
using EventType = NEP.ScoreLab.Data.EventType;

namespace NEP.ScoreLab.Core
{
    public static class ScoreDirector
    {
        internal static readonly string LocalPlayer = "RigManager(bonelab) [0]";
        
        public static class Patches
        {
            [HarmonyLib.HarmonyPatch(typeof(Projectile), nameof(Projectile.Awake))]
            public static class ProjectilePatch
            {
                public static void Postfix(Projectile __instance)
                {
                    Action<Collider, Vector3, Vector3> action = ((col, world, normal) =>
                    {
                        OnProjectileCollision(__instance, col, world, normal);
                    });
                    
                    __instance.onCollision.AddListener(action);
                }

                private static void OnProjectileCollision(Projectile __instance, Collider collider, Vector3 world, Vector3 normal)
                {
                    MarrowBody head = MarrowBody.Cache.Get(collider.gameObject);

                    if (head == null)
                    {
                        return;
                    }
                    
                    TriggerRefProxy proxy = head.GetComponent<TriggerRefProxy>();

                    if (proxy == null)
                    {
                        return;
                    }

                    if (proxy.aiManager.isDead)
                    {
                        return;
                    }

                    TriggerRefProxy playerProxy = __instance._proxy;

                    if (playerProxy.triggerType != TriggerRefProxy.TriggerType.Player)
                    {
                        return;
                    }

                    if (playerProxy.root.name != LocalPlayer)
                    {
                        Main.Logger.Msg("Projectile came from non-local player!");
                        return;
                    }

                    if (__instance._proxy.root.name != LocalPlayer)
                    {
                        return;
                    }
                    
                    if (proxy.targetHead.gameObject == head.gameObject)
                    {
                        // ScoreTracker.Add(EventType.Score.Headshot);
                    }
                }
            }

            #if DEBUG
            [HarmonyLib.HarmonyPatch(typeof(BehaviourBaseNav), nameof(BehaviourBaseNav.OnUpdate))]
            public static class TestPatch
            {
                public static void Postfix(BehaviourBaseNav __instance)
                {
                    Transform head = __instance.sensors.selfTrp.chestTran.transform;
                    Vector3 topPos = head.position + new Vector3(0f, 0.75f, 0f);

                    string output = $"Health: {__instance.health.cur_hp * __instance.health.maxHitPoints}\n";

                    NEDraw.DrawText(output, 0.5f, topPos);
                }
            }
            #endif
            
            [HarmonyLib.HarmonyPatch(typeof(SubBehaviourHealth), nameof(SubBehaviourHealth.TakeDamage))]
            public static class NPCDamagePatch
            {
                public static bool Prefix(SubBehaviourHealth __instance, int m, Attack attack)
                {
                    if (attack.proxy == null || attack.proxy.root == null)
                    {
                        return true;
                    }
                    
                    if (attack.proxy.triggerType != TriggerRefProxy.TriggerType.Player || attack.proxy.root.name != LocalPlayer)
                    {
                        return true;
                    }

                    if (!__instance.behaviour.puppetMaster.isAlive)
                    {
                        return true;
                    }
                    
                    float health = __instance.cur_hp * __instance.maxHitPoints;
                    float damage = GetAdjustedDamage(attack, ref __instance);

                    SubBehaviourHealth.StunGroup group = __instance.muscles[m];

                    if (!attack.attackType.HasFlag(AttackType.Piercing))
                    {
                        return true;
                    }
                    
                    const float headMultiplier = 4 * 1.025f;
                    
                    if (group == SubBehaviourHealth.StunGroup.Head)
                    {
                        damage *= headMultiplier;
                        
                        if (damage >= health)
                        {
                            ScoreTracker.Add(EventType.Score.Headshot);
                        }
                    }
                    
                    return true;
                }

                private static float GetAdjustedDamage(Attack attack, ref SubBehaviourHealth health)
                {
                    AttackType type = attack.attackType;
                    float damage = attack.damage;
                    
                    if (type.HasFlag(AttackType.Piercing))
                    {
                        damage *= health.pierceMult;
                    }

                    return damage;
                }
            }
            
            [HarmonyLib.HarmonyPatch(typeof(RigManager), nameof(RigManager.SwitchAvatar))]
            public static class RigManagerSwapAvatarPatch
            {
                public static void Postfix(Avatar newAvatar)
                {
                    _playerRecentlySwappedAvatars = true;
                }
            }
            
            [HarmonyLib.HarmonyPatch(typeof(BehaviourCrablet))]
            [HarmonyLib.HarmonyPatch(nameof(BehaviourCrablet.AttachToFace))]
            public static class CrabletAttachToFacePatch
            {
                public static void Postfix(Rigidbody face, TriggerRefProxy trp, bool preAttach = false, bool isPlayer = true)
                {
                    if (isPlayer)
                    {
                        return;
                    }

                    if (trp.npcType == TriggerRefProxy.NpcType.Crablet)
                    {
                        ScoreTracker.Add(EventType.Score.Crabcest);
                    }
                    else
                    {
                        ScoreTracker.Add(EventType.Score.Facehug);
                    }
                }
            }
            
            [HarmonyLib.HarmonyPatch(typeof(Seat))]
            [HarmonyLib.HarmonyPatch(nameof(Seat.Register))]
            public static class RegisterSeatPatch
            {
                public static void Postfix(RigManager rM)
                {
                    if (rM.name != LocalPlayer)
                    {
                        return;
                    }
                    
                    IsPlayerSeated = true;
                    ScoreTracker.Add(EventType.Mult.Seated);
                }
            }

            [HarmonyLib.HarmonyPatch(typeof(Seat))]
            [HarmonyLib.HarmonyPatch(nameof(Seat.DeRegister))]
            public static class DeRegisterSeatPatch
            {
                public static void Prefix(Seat __instance)
                {
                    if (__instance._rig.name == LocalPlayer)
                    {
                        IsPlayerSeated = false;
                    }
                }
            }

            [HarmonyLib.HarmonyPatch(typeof(Player_Health))]
            [HarmonyLib.HarmonyPatch(nameof(Player_Health.LifeSavingDamgeDealt))]
            public static class SecondWindPatch
            {
                public static void Postfix(Player_Health __instance)
                {
                    if (__instance._rigManager.name != LocalPlayer)
                    {
                        return;
                    }
                    
                    ScoreTracker.Add(EventType.Mult.SecondWind);
                }
            }

            [HarmonyLib.HarmonyPatch(typeof(Arena_GameController))]
            [HarmonyLib.HarmonyPatch(nameof(Arena_GameController.StartNextWave))]
            public static class StartNextWavePatch
            {
                public static void Postfix(Arena_GameController __instance)
                {
                    ScoreTracker.Add(EventType.Score.GameWaveCompleted);
                }
            }
            
            [HarmonyLib.HarmonyPatch(typeof(Arena_GameController))]
            [HarmonyLib.HarmonyPatch(nameof(Arena_GameController.EndOfRound))]
            public static class StartNextRoundPatch
            {
                public static void Postfix(Arena_GameController __instance)
                {
                    ScoreTracker.Add(EventType.Score.GameRoundCompleted);
                }
            }

            [HarmonyLib.HarmonyPatch(typeof(PhysicsRig))]
            [HarmonyLib.HarmonyPatch(nameof(PhysicsRig.OnUpdate))]
            public static class PhysRigPatch
            {
                private static bool _midAirTargetBool;
                private static float _tMidAirDelay = 0.5f;
                private static float _tAirTime;

                private static bool _ragdolledTargetBool;
                private static float _tRagdollDelay = 0.5f;
                private static float _tRagdollTime;

                public static void Postfix(PhysicsRig __instance)
                {
                    if (__instance.manager.name != LocalPlayer)
                    {
                        return;
                    }
                    
                    IsPlayerInAir = !__instance.physG.isGrounded;

                    if (IsPlayerInAir)
                    {
                        if (!_midAirTargetBool)
                        {
                            _tAirTime += Time.deltaTime;

                            if(_tAirTime > _tMidAirDelay)
                            {
                                ScoreTracker.Add(EventType.Mult.MidAir);
                                _midAirTargetBool = true;
                            }
                        }
                    }
                    else
                    {
                        _tAirTime = 0f;
                        _midAirTargetBool = false;
                    }

                    if (IsPlayerRagdolled)
                    {
                        if (!_ragdolledTargetBool)
                        {
                            // ScoreTracker.Instance.Add(Data.EventType.Mult.Ragolled);
                            _ragdolledTargetBool = true;
                        }
                    }
                    else
                    {
                        _ragdolledTargetBool = false;
                    }

                    if (_playerRecentlySwappedAvatars)
                    {
                        ScoreTracker.Add(EventType.Mult.SwappedAvatars);
                        _playerRecentlySwappedAvatars = false;
                    }
                }
            }

            [HarmonyLib.HarmonyPatch(typeof(PhysicsRig))]
            [HarmonyLib.HarmonyPatch(nameof(PhysicsRig.RagdollRig))]
            public static class PhysRigRagdollPatch
            {
                public static void Postfix(PhysicsRig __instance)
                {
                    IsPlayerRagdolled = true;
                }
            }

            [HarmonyLib.HarmonyPatch(typeof(PhysicsRig))]
            [HarmonyLib.HarmonyPatch(nameof(PhysicsRig.UnRagdollRig))]
            public static class PhysRigUnRagdollPatch
            {
                public static void Postfix(PhysicsRig __instance)
                {
                    IsPlayerRagdolled = false;
                }
            }

            [HarmonyLib.HarmonyPatch(typeof(TimeManager), nameof(TimeManager.OnPostTimeUpdate))]
            public static class TimeManagerUpdatePatch
            {
                private static bool _slowmoSwitch = false;
                
                public static void Postfix()
                {
                    
                }
            }
            
            [HarmonyLib.HarmonyPatch(typeof(TimeManager), nameof(TimeManager.INCREASE_TIMESCALE))]
            public static class TimeManagerIncreaseTimePatch
            {
                public static void Postfix()
                {
                    
                }
            }
            
            [HarmonyLib.HarmonyPatch(typeof(TimeManager), nameof(TimeManager.DECREASE_TIMESCALE))]
            public static class TimeManagerDecreaseTimePatch
            {
                public static void Postfix()
                {
                    
                }
            }

            public static void InitPatches()
            {
                Hooking.OnNPCKillStart += OnAIDeath;
            }

            public static void OnAIDeath(BehaviourBaseNav behaviour)
            {
                ScoreTracker.Add(ValueManager.Get(EventType.Score.Kill));
                ScoreTracker.Add(ValueManager.Get(EventType.Mult.Kill));
                
                if(!behaviour.sensors.isGrounded)
                {
                    ScoreTracker.Add(EventType.Score.EnemyMidAirKill);
                }

                if (behaviour.sensors.target == null)
                {
                    ScoreTracker.Add(EventType.Score.StealthKill);
                }
            }
        }

        public static readonly string PlayerName = "";
        
        public static bool IsPlayerMoving = false;
        public static bool IsPlayerInAir = false;
        public static bool IsPlayerSeated = false;
        public static bool IsPlayerRagdolled = false;
        public static bool IsSlowMoEnabled = false;

        private static bool _playerRecentlySwappedAvatars = false;
    }
}
