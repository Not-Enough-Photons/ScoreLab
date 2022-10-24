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

        public PackedValue.PackedType packedType;

        public GameObject modulePrefab { get; set; }
        public int count = 6;

        public List<UIModule> modules;

        private void Awake()
        {
            modules = new List<UIModule>();
        }

        private void Start()
        {
            API.Score.OnScoreAdded += SetScoreModuleActive;

            API.Multiplier.OnMultiplierAdded += (data) => SetMultiplierModuleActive(data, true);
        }

        public void SetPackedType(int packedType)
        {
            this.packedType = (PackedValue.PackedType)packedType;
        }

        public void PopulateList()
        {
            for (int i = 0; i < count; i++)
            {
                var obj = GameObject.Instantiate(modulePrefab.gameObject, transform);
                var module = obj.GetComponent<UIModule>();

                obj.hideFlags = HideFlags.DontUnloadUnusedAsset;

                module.ModuleType = UIModule.UIModuleType.Descriptor;
                modules.Add(module);
                obj.SetActive(false);
            }
        }

        public void SetScoreModuleActive(PackedScore packedValue)
        {
            if (modules == null || modules.Count == 0)
            {
                return;
            }

            if (packedValue.PackedValueType != packedType)
            {
                return;
            }

            for (int i = 0; i < modules.Count; i++)
            {
                UIScoreModule scoreModule = (UIScoreModule)modules[i];

                if (!modules[i].gameObject.activeInHierarchy)
                {
                    scoreModule.AssignPackedData(packedValue);

                    scoreModule.SetDecayTime(packedValue.DecayTime);
                    scoreModule.SetPostDecayTime(0.5f);

                    modules[i].gameObject.SetActive(true);
                    return;
                }
                else
                {
                    if (scoreModule.PackedValue != null)
                    {
                        scoreModule.OnModuleEnable();
                        scoreModule.SetDecayTime(packedValue.DecayTime);
                        scoreModule.SetPostDecayTime(0.5f);

                        return;
                    }
                }
            }
        }

        public void SetMultiplierModuleActive(PackedMultiplier packedValue, bool active)
        {
            if (modules == null || modules.Count == 0)
            {
                return;
            }

            if (packedValue.PackedValueType != packedType)
            {
                return;
            }

            for (int i = 0; i < modules.Count; i++)
            {
                UIMultiplierModule multiplierModule = (UIMultiplierModule)modules[i];

                if (!modules[i].gameObject.activeInHierarchy)
                {
                    multiplierModule.AssignPackedData(packedValue);

                    multiplierModule.SetDecayTime(packedValue.DecayTime);
                    multiplierModule.SetPostDecayTime(0.5f);

                    modules[i].gameObject.SetActive(true);
                    return;
                }
                else
                {
                    if (multiplierModule.PackedValue != null)
                    {
                        multiplierModule.OnModuleEnable();
                        multiplierModule.SetDecayTime(packedValue.DecayTime);
                        multiplierModule.SetPostDecayTime(0.5f);

                        return;
                    }
                }
            }

        }
    }
}

