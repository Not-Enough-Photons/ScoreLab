using UnityEngine;

using MelonLoader;

using NEP.ScoreLab.Core.Data;

using System;
using System.Reflection;
using System.Runtime.InteropServices;

using SLZ.Combat;
using SLZ.Marrow.Data;

namespace NEP.ScoreLab.Utilities
{
    public static class Utils
    {
        public static string customMapName;

        private static Type mapLoaderType;

        public static class ImpactPropertiesPatch
        {
            private static AttackPatchDelegate _original;

            public delegate void AttackPatchDelegate(IntPtr instance, IntPtr attack, IntPtr method);

            public static event Action<Attack> OnAttackRecieved;

            public static unsafe void Patch()
            {
                AttackPatchDelegate patch = Patch;

                string nativeInfoName = "NativeMethodInfoPtr_ReceiveAttack_Public_Virtual_Final_New_Void_Attack_0";
                var tgtPtr = *(IntPtr*)(IntPtr)typeof(ImpactProperties).GetField(nativeInfoName, BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
                var dstPtr = patch.Method.MethodHandle.GetFunctionPointer();

                MelonUtils.NativeHookAttach((IntPtr)(&tgtPtr), dstPtr);
                _original = Marshal.GetDelegateForFunctionPointer<AttackPatchDelegate>(tgtPtr);
            }

            public static void Patch(IntPtr instance, IntPtr _attack, IntPtr method)
            {
                unsafe
                {
                    try
                    {
                        var addrAttack = *(Attack_*)_attack;

                        Attack attack = new Attack()
                        {
                            damage = addrAttack.damage,
                            normal = addrAttack.normal,
                            origin = addrAttack.origin,
                            backFacing = addrAttack.backFacing == 1 ? true : false,
                            OrderInPool = addrAttack.OrderInPool,
                            collider = addrAttack.Collider,
                            attackType = addrAttack.attackType,
                            proxy = addrAttack.Proxy
                        };

                        OnAttackRecieved?.Invoke(attack);
                    }
                    catch
                    {

                    }
                }

                _original(instance, _attack, method);
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct Attack_
            {
                public float damage;
                public Vector3 normal;
                public Vector3 origin;
                public Vector3 force;
                public byte backFacing;
                public int OrderInPool;
                public IntPtr collider;
                public AttackType attackType;
                public IntPtr proxy;

                public Collider Collider
                {
                    get
                    {
                        return new Collider(collider);
                    }

                    set
                    {
                        collider = value.Pointer;
                    }
                }

                public SLZ.AI.TriggerRefProxy Proxy
                {
                    get
                    {
                        return new SLZ.AI.TriggerRefProxy(proxy);
                    }

                    set
                    {
                        proxy = value.Pointer;
                    }
                }
            }
        }

        public static class RigidbodyProjectilePatch
        {
            private static AttackPatchDelegate _original;

            public delegate void AttackPatchDelegate(IntPtr instance, IntPtr attack, IntPtr method);

            public static event Action<Attack> OnAttackRecieved;

            public static unsafe void Patch()
            {
                AttackPatchDelegate patch = Patch;

                string nativeInfoName = "NativeMethodInfoPtr_ReceiveAttack_Public_Virtual_Final_New_Void_Attack_0";
                var tgtPtr = *(IntPtr*)(IntPtr)typeof(RigidbodyProjectile).GetField(nativeInfoName, BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
                var dstPtr = patch.Method.MethodHandle.GetFunctionPointer();

                MelonUtils.NativeHookAttach((IntPtr)(&tgtPtr), dstPtr);
                _original = Marshal.GetDelegateForFunctionPointer<AttackPatchDelegate>(tgtPtr);
            }

            public static void Patch(IntPtr instance, IntPtr _attack, IntPtr method)
            {
                unsafe
                {
                    try
                    {
                        var addrAttack = *(Attack_*)_attack;

                        Attack attack = new Attack()
                        {
                            damage = addrAttack.damage,
                            normal = addrAttack.normal,
                            origin = addrAttack.origin,
                            backFacing = addrAttack.backFacing == 1 ? true : false,
                            OrderInPool = addrAttack.OrderInPool,
                            collider = addrAttack.Collider,
                            attackType = addrAttack.attackType,
                            proxy = addrAttack.Proxy
                        };

                        OnAttackRecieved?.Invoke(attack);
                    }
                    catch
                    {

                    }
                }

                _original(instance, _attack, method);
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct Attack_
            {
                public float damage;
                public Vector3 normal;
                public Vector3 origin;
                public Vector3 force;
                public byte backFacing;
                public int OrderInPool;
                public IntPtr collider;
                public SLZ.Marrow.Data.AttackType attackType;
                public IntPtr proxy;

                public Collider Collider
                {
                    get
                    {
                        return new Collider(collider);
                    }

                    set
                    {
                        collider = value.Pointer;
                    }
                }

                public SLZ.AI.TriggerRefProxy Proxy
                {
                    get
                    {
                        return new SLZ.AI.TriggerRefProxy(proxy);
                    }

                    set
                    {
                        proxy = value.Pointer;
                    }
                }
            }
        }

        public static string GetLevelFromSceneName(string currentScene)
        {
            switch (currentScene)
            {
                case "scene_mainMenu": return "Main Menu";
                case "scene_theatrigon_movie01": return "Theatrigon 01";
                case "scene_breakroom": return "Breakroom";
                case "scene_museum": return "Museum of Technical Demonstrations";
                case "scene_streets": return "Streets";
                case "scene_runoff": return "Runoff";
                case "scene_sewerStation": return "Sewers";
                case "scene_warehouse": return "Warehouse";
                case "scene_subwayStation": return "Central Station";
                case "scene_tower": return "Tower";
                case "scene_towerBoss": return "Clock Tower";
                case "scene_theatrigon_movie02": return "Theatrigon 02";
                case "scene_dungeon": return "Fantasy Land Dungeon";
                case "scene_arena": return "Arena (Story)";
                case "scene_throneRoom": return "Throne Room";
                case "sandbox_museumBasement": return "Museum Basement";
                case "sandbox_blankBox": return "Blankbox";
                case "scene_Tuscany": return "Tuscany";
                case "scene_redactedChamber": return "[REDACTED] Chamber";
                case "sandbox_handgunBox": return "Handgun Range";
                case "scene_hoverJunkers": return "Hover Junkers";
                case "arena_fantasy": return "Fantasy Arena";
                case "zombie_warehouse": return "Zombie Warehouse";
                case "custom_map_bbl": return "Custom Map";
                case "MelonVault": return "Melon Vault";
            }

            return "Unknown Scene";
        }
    }
}
