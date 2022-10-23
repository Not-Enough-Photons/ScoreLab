using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NEP.ScoreLab.Core;
using NEP.ScoreLab.Data;

namespace NEP.ScoreLab.UI
{
    public class UIDescriptorList : MonoBehaviour
    {
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
            API.Multiplier.OnMultiplierRemoved += (data) => SetMultiplierModuleActive(data, false);
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
                    UIScoreModule scoreModule = (UIScoreModule)modules[i];

                    scoreModule.AssignPackedData(packedValue);

                    scoreModule.SetDecayTime(5f);
                    scoreModule.SetPostDecayTime(0.5f);

                    modules[i].gameObject.SetActive(true);
                    return;
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
                    UIMultiplierModule multiplierModule = (UIMultiplierModule)modules[i];

                    multiplierModule.AssignPackedData(packedValue);

                    multiplierModule.SetDecayTime(packedValue.Timer);
                    multiplierModule.SetPostDecayTime(0.5f);

                    modules[i].gameObject.SetActive(active);
                    return;
                }
            }
        }
    }
}

