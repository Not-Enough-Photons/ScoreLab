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

                // Not a valid UI
                if(controller == null)
                {
                    continue;
                }

                LoadedUIs.Add(controller);
                controller.SetParent(transform);
                controller.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            LoadHUD("Coda");
        }

        public void LoadHUD(string name)
        {
            MelonLoader.MelonLogger.Msg(1);
            UnloadHUD();
            MelonLoader.MelonLogger.Msg(2);
            foreach (var _controller in LoadedUIs)
            {
                MelonLoader.MelonLogger.Msg(3);
                if (DataManager.UI.GetHUDName(_controller.gameObject) == name)
                {
                    MelonLoader.MelonLogger.Msg(4);
                    ActiveUI = _controller;
                    MelonLoader.MelonLogger.Msg(6);
                    break;
                }
            }

            MelonLoader.MelonLogger.Msg(7);
            ActiveUI.gameObject.SetActive(true);
            MelonLoader.MelonLogger.Msg(8);
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