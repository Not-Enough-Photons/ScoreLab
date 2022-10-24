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
            //modulePrefab.hideFlags = HideFlags.DontUnloadUnusedAsset;
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
                obj.hideFlags = HideFlags.DontUnloadUnusedAsset;

                var module = obj.GetComponent<UIModule>();

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
                if (!modules[i].gameObject.activeInHierarchy)
                {
                    modules[i].AssignPackedData(packedValue);

                    modules[i].SetDecayTime(packedValue.DecayTime);
                    modules[i].SetPostDecayTime(0.5f);

                    modules[i].gameObject.SetActive(true);
                    return;
                }
                else
                {
                    if (modules[i].PackedValue != null)
                    {
                        modules[i].OnModuleEnable();
                        modules[i].SetDecayTime(packedValue.DecayTime);
                        modules[i].SetPostDecayTime(0.5f);

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
                if (!modules[i].gameObject.activeInHierarchy)
                {
                    modules[i].AssignPackedData(packedValue);

                    modules[i].SetDecayTime(packedValue.DecayTime);
                    modules[i].SetPostDecayTime(0.5f);

                    modules[i].gameObject.SetActive(true);
                    return;
                }
                else
                {
                    if (modules[i].PackedValue != null)
                    {
                        modules[i].OnModuleEnable();
                        modules[i].SetDecayTime(packedValue.DecayTime);
                        modules[i].SetPostDecayTime(0.5f);

                        return;
                    }
                }
            }

        }
    }
}

