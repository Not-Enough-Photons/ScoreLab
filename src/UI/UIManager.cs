using UnityEngine;

using NEP.ScoreLab.Core;
using NEP.ScoreLab.Data;

namespace NEP.ScoreLab.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class UIManager : MonoBehaviour
    {
        public UIManager(System.IntPtr ptr) : base(ptr) { }

        public UIModule ScoreModule;
        public UIModule MultiplierModule;
        public UIModule HighScoreModule;

        public Transform followTarget;

        private void Awake()
        {
            API.Score.OnScoreAdded += (data) => UpdateModule(data, ScoreModule);

            API.Multiplier.OnMultiplierAdded += (data) => UpdateModule(data, MultiplierModule);
            API.Multiplier.OnMultiplierRemoved += (data) => UpdateModule(data, MultiplierModule);
        }

        private void Start()
        {
            ScoreModule = transform.Find("Main_Score").GetComponent<UIModule>();
            MultiplierModule = transform.Find("Main_Multiplier").GetComponent<UIModule>();
            followTarget = BoneLib.Player.GetPlayerHead().transform;
        }

        private void Update()
        {
            if(followTarget == null)
            {
                return;
            }

            Vector3 move = Vector3.Lerp(transform.position, followTarget.position + followTarget.forward * 3f, 6f * Time.deltaTime);
            Quaternion lookRot = Quaternion.LookRotation(followTarget.forward);

            transform.position = move;
            transform.rotation = lookRot;

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i) != null)
                {
                    //transform.GetChild(i).LookAt(followTarget);
                }
            }
        }

        public void UpdateModule(PackedValue data, UIModule module)
        {
            if(module == null)
            {
                return;
            }

            module.AssignPackedData(data);
            module.OnModuleEnable();
        }
    }
}
