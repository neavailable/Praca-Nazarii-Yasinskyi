using System;
using UnityEngine;


namespace MainScene.Feedback
{
    // One place that answers game events with a sound and a burst of particles.
    [RequireComponent(typeof(AudioSource))]
    public class FeedbackPlayer : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _burstPrefab;
        [SerializeField] private FeedbackEntry[] _entries;

        private AudioSource _audioSource;


        private void Awake() => _audioSource = GetComponent<AudioSource>();

        public void Play(FeedbackType type, Vector3 position)
        {
            FeedbackEntry entry = Array.Find(_entries, candidate => candidate.Type == type);
            if (entry == null) return;

            if (entry.Clip != null) _audioSource.PlayOneShot(entry.Clip, entry.Volume);
            if (entry.Particles > 0) Burst(entry, position);
        }


        private void Burst(FeedbackEntry entry, Vector3 position)
        {
            ParticleSystem burst = Instantiate(_burstPrefab, position, Quaternion.identity);

            ParticleSystem.MainModule main = burst.main;
            main.startColor = entry.Color;
            main.startSpeed = entry.Speed;

            burst.Play();
            burst.Emit(entry.Particles);
        }
    }

    [Serializable]
    public class FeedbackEntry
    {
        public FeedbackType Type;
        public AudioClip Clip;
        [Range(0.0f, 1.0f)] public float Volume = 0.8f;
        public Color Color = Color.white;
        public int Particles;
        public float Speed = 4.0f;
    }
}
