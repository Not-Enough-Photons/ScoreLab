using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NEP.Scoreworks.Audio
{
    public class AudioManager
    {
        public AudioManager()
        {
            Start();
        }

        public AudioClip[] scoreGet;
        public AudioClip[] multiplierGet;

        private AudioSource source;

        private void Awake()
        {
            source = new GameObject("Scoreworks Audio Manager").AddComponent<AudioSource>();
        }

        private void Start()
        {
            Core.ScoreworksManager.OnScoreAdded += (data) => OnScoreAdded();
            Core.ScoreworksManager.OnMultiplierAdded += (data) => OnMultiplierAdded();
        }

        private void OnScoreAdded()
        {
            if(source == null)
            {
                return;
            }

            int rand = Random.Range(0, scoreGet.Length);
            AudioClip random = scoreGet[rand];

            if(random == null)
            {
                return;
            }

            source.clip = random;
            source.Play();
        }

        private void OnMultiplierAdded()
        {
            if (source == null)
            {
                return;
            }

            int rand = Random.Range(0, multiplierGet.Length);
            AudioClip random = multiplierGet[rand];

            if (random == null)
            {
                return;
            }

            source.clip = random;
            source.pitch = 1f + (Core.ScoreworksManager.instance.currentMultiplier / 10f);
            source.Play();
        }
    }
}
