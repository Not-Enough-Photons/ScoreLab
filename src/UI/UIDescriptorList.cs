using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NEP.ScoreLab.Core;
using NEP.ScoreLab.Data;

namespace NEP.ScoreLab.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class UIDescriptorList : MonoBehaviour
    {
        public UIDescriptorList(System.IntPtr ptr) : base(ptr) { }

        public List<UIModule> ActiveModules;

        public PackedValue.PackedType packedType { get; set; }
        public GameObject modulePrefab { get; set; }
        public int count = 6;

        public List<UIModule> modules;

        private void Awake()
        {
            modules = new List<UIModule>();
            ActiveModules = new List<UIModule>();

            for (int i = 0; i < transform.childCount; i++)
            {
                var current = transform.GetChild(i);
                var module = current.GetComponent<UIModule>();

                module.ModuleType = UIModule.UIModuleType.Descriptor;
                modules.Add(module);
            }
        }

        private void OnEnable()
        {
            API.UI.OnModulePostDecayed += (item) => ActiveModules.Remove(item);

            API.Score.OnScoreAdded += SetModuleActive;
            API.Score.OnScoreAccumulated += SetModuleActive;
            API.Score.OnScoreTierReached += SetModuleActive;

            API.Multiplier.OnMultiplierAdded += SetModuleActive;
            API.Multiplier.OnMultiplierAccumulated += SetModuleActive;
            API.Multiplier.OnMultiplierTierReached += SetModuleActive;
        }

        private void OnDisable()
        {
            API.UI.OnModulePostDecayed -= (item) => ActiveModules.Remove(item);

            API.Score.OnScoreAdded -= SetModuleActive;
            API.Score.OnScoreAccumulated -= SetModuleActive;
            API.Score.OnScoreTierReached -= SetModuleActive;

            API.Multiplier.OnMultiplierAdded -= SetModuleActive;
            API.Multiplier.OnMultiplierAccumulated -= SetModuleActive;
            API.Multiplier.OnMultiplierTierReached -= SetModuleActive;
        }

        public void SetPackedType(int packedType)
        {
            this.packedType = (PackedValue.PackedType)packedType;
        }

        public void SetModuleActive(PackedValue value)
        {
            if (modules == null || modules.Count == 0)
            {
                return;
            }

            if (value.PackedValueType != packedType)
            {
                return;
            }

            foreach (var module in modules)
            {
                if (!module.gameObject.activeInHierarchy)
                {
                    module.AssignPackedData(value);
                    module.SetDecayTime(value.DecayTime);
                    module.SetPostDecayTime(0.5f);

                    module.gameObject.SetActive(true);

                    ActiveModules.Add(module);
                    break;
                }
                else
                {
                    if (ActiveModules.Contains(module))
                    {
                        if (module.PackedValue.eventType == value.eventType)
                        {
                            if (value.Stackable)
                            {
                                module.AssignPackedData(value);
                                module.OnModuleEnable();
                                module.SetDecayTime(value.DecayTime);
                                module.SetPostDecayTime(0.5f);
                                break;
                            }

                            if (value.Tiers != null)
                            {
                                module.AssignPackedData(value);
                                module.OnModuleEnable();
                                module.SetDecayTime(value.DecayTime);
                                module.SetPostDecayTime(0.5f);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}

