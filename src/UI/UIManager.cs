using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NEP.ScoreLab.Data;

namespace NEP.ScoreLab.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class UIManager : MonoBehaviour
    {
        public UIManager(System.IntPtr ptr) : base(ptr) { }

        public List<UIController> LoadedUIs { get; private set; }
        public UIController ActiveUI { get; private set; }

        private void Awake()
        {
            LoadedUIs = new List<UIController>();

            for(int i = 0; i < DataManager.UI.LoadedUIObjects.Count; i++)
            {
                var _object = GameObject.Instantiate(DataManager.UI.LoadedUIObjects[i]);
                _object.name = _object.name.Remove(_object.name.Length - 7);

                var controller = _object.GetComponent<UIController>();
                LoadedUIs.Add(controller);
                controller.SetParent(transform);
                controller.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            LoadHUD(DataManager.UI.DefaultUIName);
        }

        public void LoadHUD(string name)
        {
            UnloadHUD();

            foreach(var _controller in LoadedUIs)
            {
                if(DataManager.UI.GetHUDName(_controller.gameObject) == name)
                {
                    ActiveUI = _controller;
                    break;
                }
            }

            ActiveUI.gameObject.SetActive(true);
            ActiveUI.SetParent(null);
        }

        public void UnloadHUD()
        {
            if(ActiveUI != null)
            {
                ActiveUI.SetParent(transform);
                ActiveUI.gameObject.SetActive(false);
                ActiveUI = null;
            }
        }
    }
}