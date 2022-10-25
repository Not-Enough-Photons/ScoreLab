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

        public PackedValue.PackedType packedType { get; set; }
        public GameObject modulePrefab { get; set; }
        public int count = 6;

        public List<UIModule> modules;

        private void Awake()
        {
            modules = new List<UIModule>();

            for(int i = 0; i < transform.childCount; i++)
            {
                var current = transform.GetChild(i);
                var module = current.GetComponent<UIModule>();

                module.ModuleType = UIModule.UIModuleType.Descriptor;
                modules.Add(module);
            }
        }

        private void OnEnable()
        {
            API.Score.OnScoreAdded += SetModuleActive;
            API.Multiplier.OnMultiplierAdded += SetModuleActive;
        }

        private void OnDisable()
        {
            API.Score.OnScoreAdded -= SetModuleActive;
            API.Multiplier.OnMultiplierAdded -= SetModuleActive;
        }

        public void SetPackedType(int packedType)
        {
            this.packedType = (PackedValue.PackedType)packedType;
        }

        public void SetModuleActive(PackedValue value)
        {
            if(modules == null || modules.Count == 0)
            {
                return;
            }

            if(value.PackedValueType != packedType)
            {
                return;
            }

            foreach(var module in modules)
            {
                if (!module.gameObject.activeInHierarchy)
                {
                    module.AssignPackedData(value);

                    module.SetDecayTime(value.DecayTime);
                    module.SetPostDecayTime(0.5f);

                    module.gameObject.SetActive(true);

                    return;
                }
                else
                {
                    module.OnModuleEnable();
                    module.SetDecayTime(value.DecayTime);
                    module.SetPostDecayTime(0.5f);

                    return;
                }
            }
        }
    }
}

