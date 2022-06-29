using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NEP.Scoreworks.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class UIRegion : MonoBehaviour
    {
        public UIRegion(System.IntPtr ptr) : base(ptr) { }

        public enum Location
        {
            Left,
            Right,
            Top,
            Bottom,
            Null
        }

        public Location location;
        public List<Modules.UIModule> modules;

        private void Awake()
        {
            modules = new List<Modules.UIModule>();

            string regionName = transform.name;

            if (IsValidRegion(regionName))
            {
                location = (Location)System.Enum.Parse(typeof(Location), regionName.Substring(7));
            }

            for(int i = 0; i < transform.childCount; i++)
            {
                Transform currentTransform = transform.GetChild(i);

                string name = currentTransform.name;

                if(name == "Module_Score")
                {
                    Modules.UIModule module = currentTransform.gameObject.AddComponent<Modules.UIModule>();
                    modules?.Add(module);
                }
                else if(name == "Module_Multiplier")
                {
                    Modules.UIModule module = currentTransform.gameObject.AddComponent<Modules.UIModule>();
                    modules?.Add(module);
                }
                else if(name == "Module_HighScore")
                {
                    Modules.UIModule module = currentTransform.gameObject.AddComponent<Modules.UIModule>();
                    modules?.Add(module);
                }
            }
        }

        public bool IsValidRegion(string region)
        {
            string _region = region.ToLower();

            if (!_region.StartsWith("region_"))
            {
                return false;
            }

            switch (region)
            {
                case "Region_Left": return true;
                case "Region_Right": return true;
                case "Region_Top": return true;
                case "Region_Bottom": return true;
                default:
                    throw new System.Exception("Region needs to be Left, Right, Top, or Bottom!");
            }
        }
    }
}