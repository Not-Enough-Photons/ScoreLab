using UnityEngine;

using MelonLoader;

using ModThatIsNotMod.BoneMenu;

using NEP.Scoreworks.Core.Data;

using System;
using System.Reflection;
using System.Runtime.InteropServices;

using StressLevelZero.Combat;

namespace NEP.Scoreworks.Utilities
{
    public static class Utils
    {
        public static string customMapName;

        private static Type mapLoaderType;

        public static class BoneMenu
        {
            public static void SetupBonemenu()
            {
                MenuCategory mainCategory = MenuManager.CreateCategory("Scoreworks Settings", Color.white);
                MenuCategory hudCategory = mainCategory.CreateSubCategory("HUDs", Color.blue);
                MenuCategory hudSettingsCategory = mainCategory.CreateSubCategory("HUD Settings", Color.white);
                MenuCategory highScoreCategory = mainCategory.CreateSubCategory("High Score Settings", Color.white);
                SetupHUDSettings(hudSettingsCategory);
                SetupHighScoreSettings(highScoreCategory);

                foreach (GameObject uiObject in Main.instance.customUIs)
                {
                    hudCategory.CreateFunctionElement(uiObject.name, Color.white, () => Main.instance.SpawnHUD(uiObject));
                }
            }

            public static void SetupHUDSettings(MenuCategory category)
            {
                UI.UISettings settings = DataManager.ReadHUDSettings();

                float initialDistance = settings.followDistance;
                float initialLerp = settings.followLerp;
                bool initialFollowHead = settings.followHead;

                category.CreateFloatElement("Follow Distance", Color.white, initialDistance, (newValue) => UpdateHUDFollowDistance(newValue), 1f, 0f, float.PositiveInfinity, true);
                category.CreateFloatElement("Follow Lerp", Color.white, initialLerp, (newValue) => UpdateHUDFollowLerp(newValue), 1f, 0f, float.PositiveInfinity, true);
                category.CreateBoolElement("Follow Head", Color.white, initialFollowHead, (newValue) => UpdateHUDFollowHead(newValue));
            }

            public static void SetupHighScoreSettings(MenuCategory category)
            {
                category.CreateFunctionElement("Delete High Score", Color.red, () => DataManager.DeleteHighScore());
                category.CreateFunctionElement("Delete All High Scores", Color.red, () => DataManager.DeleteAllHighScores());
                category.CreateFunctionElement("BE VERY CAREFUL WITH THIS", Color.red, null);
            }

            public static void UpdateHUDFollowDistance(float value)
            {
                UI.UIManager manager = Main.instance.uiObject.GetComponent<UI.UIManager>();

                if (manager == null)
                {
                    return;
                }

                UI.UISettings settings = new UI.UISettings()
                {
                    followDistance = value,
                    followLerp = manager.hudSettings.followLerp,
                    followHead = manager.hudSettings.followHead
                };

                manager.hudSettings = settings;

                DataManager.SaveHUDSettings();
            }

            public static void UpdateHUDFollowLerp(float value)
            {
                UI.UIManager manager = Main.instance.uiObject.GetComponent<UI.UIManager>();

                if (manager == null)
                {
                    return;
                }

                UI.UISettings settings = new UI.UISettings()
                {
                    followDistance = manager.hudSettings.followDistance,
                    followLerp = value,
                    followHead = manager.hudSettings.followHead
                };

                manager.hudSettings = settings;

                DataManager.SaveHUDSettings();
            }

            public static void UpdateHUDFollowHead(bool value)
            {
                UI.UIManager manager = Main.instance.uiObject.GetComponent<UI.UIManager>();

                if (manager == null)
                {
                    return;
                }

                UI.UISettings settings = new UI.UISettings()
                {
                    followDistance = manager.hudSettings.followDistance,
                    followLerp = manager.hudSettings.followLerp,
                    followHead = value
                };

                manager.hudSettings = settings;

                DataManager.SaveHUDSettings();
            }
        }

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
                            force = addrAttack.force,
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

                public StressLevelZero.AI.TriggerRefProxy Proxy
                {
                    get
                    {
                        return new StressLevelZero.AI.TriggerRefProxy(proxy);
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
                            force = addrAttack.force,
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

                public StressLevelZero.AI.TriggerRefProxy Proxy
                {
                    get
                    {
                        return new StressLevelZero.AI.TriggerRefProxy(proxy);
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

        public static void HookCustomMaps()
        {
            // From ModThatIsNotMod
            foreach (MelonMod mod in MelonHandler.Mods)
            {
                if (mod.Info.Name == "Custom Maps")
                {
                    try
                    {
                        Assembly assembly = mod.Assembly;
                        Type customMapsType = assembly.GetType("CustomMaps.CustomMaps");
                        mapLoaderType = assembly.GetType("CustomMaps.MapLoader");
                        EventInfo eventInfo = customMapsType.GetEvent("OnCustomMapLoad", BindingFlags.Static | BindingFlags.Public);
                        MethodInfo hookMethod = typeof(Utils).GetMethod("OnCustomMapLoaded", BindingFlags.Static | BindingFlags.Public);
                        eventInfo.AddEventHandler(null, Delegate.CreateDelegate(eventInfo.EventHandlerType, hookMethod));
                    }
                    catch
                    {

                    }

                    break;
                }
            }
        }

        public static void OnCustomMapLoaded(string name)
        {
            // From ModThatIsNotMod
            if (name == "map.bcm")
            {
                try
                {
                    FieldInfo mapInfoField = mapLoaderType.GetField("mapInfo", BindingFlags.Public | BindingFlags.Static);
                    dynamic mapInfo = mapInfoField.GetValue(null);
                    customMapName = mapInfo.mapName;

                    Core.API.OnHighScoreReached.Invoke(null);
                    Main.instance.ResetScoreManager(customMapName, true);
                }
                catch { }
            }
            else
            {
                Core.API.OnHighScoreReached.Invoke(null);
                Main.instance.ResetScoreManager(name.Replace(".bcm", ""), true);
            }
        }

        public static string GetCustomMapName()
        {
            return customMapName;
        }
    }
}
