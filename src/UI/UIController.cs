using UnityEngine;

using NEP.ScoreLab.Core;
using NEP.ScoreLab.Data;

namespace NEP.ScoreLab.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class UIController : MonoBehaviour
    {
        public UIController(System.IntPtr ptr) : base(ptr) { }

        public UIModule ScoreModule { get; set; }
        public UIModule MultiplierModule { get; set; }
        public UIModule HighScoreModule { get; set; }

        public Transform followTarget;

        private void Awake()
        {
            ScoreModule = transform.Find("Main_Score").GetComponent<UIScoreModule>();
            MultiplierModule = transform.Find("Main_Multiplier").GetComponent<UIMultiplierModule>();
        }

        private void OnEnable()
        {
            UpdateModule(null, ScoreModule);

            API.Score.OnScoreAdded += (data) => UpdateModule(data, ScoreModule);

            API.Multiplier.OnMultiplierAdded += (data) => UpdateModule(data, MultiplierModule);
            API.Multiplier.OnMultiplierRemoved += (data) => UpdateModule(data, MultiplierModule);
        }

        private void OnDisable()
        {
            API.Score.OnScoreAdded -= (data) => UpdateModule(data, ScoreModule);

            API.Multiplier.OnMultiplierAdded -= (data) => UpdateModule(data, MultiplierModule);
            API.Multiplier.OnMultiplierRemoved -= (data) => UpdateModule(data, MultiplierModule);
        }

        private void Start()
        {
            if(BoneLib.Player.GetPhysicsRig() != null)
            {
                followTarget = BoneLib.Player.GetPhysicsRig().m_chest;
            }
        }

        private void Update()
        {
            if (followTarget == null)
            {
                return;
            }

            Vector3 move = Vector3.Lerp(transform.position, followTarget.position + followTarget.forward * 3f, 8f * Time.deltaTime);
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

        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }

        public void SetScoreModule(UIModule module)
        {
            this.ScoreModule = module;
        }

        public void SetMultiplierModule(UIModule module)
        {
            this.MultiplierModule = module;
        }

        public void UpdateModule(PackedValue data, UIModule module)
        {
            if (module.WasCollected)
            {
                return;
            }

            module.AssignPackedData(data);
            module.OnModuleEnable();
        }
    }
}
