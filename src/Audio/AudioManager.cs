using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NEP.ScoreLab.Core;
using NEP.ScoreLab.Data;

namespace NEP.ScoreLab.Audio
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class AudioManager : MonoBehaviour
    {
        public AudioManager(System.IntPtr ptr) : base(ptr) { }

        public static AudioManager Instance { get; private set; }

        private List<GameObject> _pooledObjects;

        private void Awake()
        {
            _pooledObjects = new List<GameObject>();

            GameObject list = new GameObject("Pooled Audio");
            list.transform.parent = transform;

            for(int i = 0; i < 64; i++)
            {
                GameObject pooledAudio = new GameObject("Poolee Audio");
                pooledAudio.transform.parent = list.transform;

                AudioSource source = pooledAudio.AddComponent<AudioSource>();
                source.playOnAwake = true;
                source.volume = 5f;

                pooledAudio.AddComponent<PooledAudio>();
                pooledAudio.SetActive(false);
                _pooledObjects.Add(pooledAudio);
            }
        }

        private void OnEnable()
        {
            API.Score.OnScoreAdded += OnValueReceived;
            API.Score.OnScoreTierReached += OnValueReceived;

            API.Multiplier.OnMultiplierAdded += OnValueReceived;
            API.Multiplier.OnMultiplierTierReached += OnValueReceived;
        }

        private void OnDisable()
        {
            API.Score.OnScoreAdded -= OnValueReceived;
            API.Score.OnScoreTierReached -= OnValueReceived;

            API.Multiplier.OnMultiplierAdded -= OnValueReceived;
            API.Multiplier.OnMultiplierTierReached -= OnValueReceived;
        }

        private void OnValueReceived(PackedValue value)
        {
            if(value.EventAudio != null)
            {
                Play(value.EventAudio);
            }
        }

        public void Play(AudioClip clip)
        {
            GameObject inactiveSource = GetFirstInactive();
            AudioSource source = inactiveSource.GetComponent<AudioSource>();

            if(source != null)
            {
                source.clip = clip;
                inactiveSource.SetActive(true);
            }
        }

        private GameObject GetFirstInactive()
        {
            for(int i = 0; i < _pooledObjects.Count; i++)
            {
                if (!_pooledObjects[i].gameObject.activeInHierarchy)
                {
                    return _pooledObjects[i];
                }
            }

            return null;
        }
    }
}
