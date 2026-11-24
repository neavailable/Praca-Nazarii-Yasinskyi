using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using MainScene.Feedback;
using MainScene.UI.Timer;

namespace BallFinisher.Editor
{
    // Sounds and particle bursts for game events. The sounds are synthesised here and saved as WAV files,
    // so the game needs no downloaded audio.
    public static partial class FeatureSetup
    {
        private const string SoundsFolder = "Assets/Audio/Generated/";
        private const int SampleRate = 44100;

        private class FeedbackSetting
        {
            public FeedbackType Type;
            public Func<float, float> Wave;
            public float Duration, Volume, Speed;
            public Color Color;
            public int Particles;
        }

        // Built on demand: colours from the other parts of this class may not be initialised yet
        // when static fields of this file are.
        private static FeedbackSetting[] CreateFeedbackSettings() => new FeedbackSetting[]
        {
            new () { Type = FeedbackType.Coin,          Duration = 0.25f, Volume = 0.5f, Color = new Color(1.0f, 0.85f, 0.2f), Particles = 14, Speed = 3.0f,
                     Wave = t => Tone(t < 0.07f ? 988.0f : 1319.0f, t) * Decay(t, 0.12f) },
            new () { Type = FeedbackType.Heal,          Duration = 0.4f,  Volume = 0.6f, Color = new Color(0.3f, 0.9f, 0.4f),  Particles = 18, Speed = 3.0f,
                     Wave = t => Arpeggio(t, 0.08f, 523.0f, 659.0f, 784.0f, 1047.0f) * Decay(t, 0.3f) },
            new () { Type = FeedbackType.TimeBonus,     Duration = 0.6f,  Volume = 0.6f, Color = TimeCoinColor,                   Particles = 20, Speed = 4.0f,
                     Wave = t => (Tone(1568.0f, t) + 0.6f * Tone(2093.0f, t)) * 0.6f * Decay(t, 0.25f) },
            new () { Type = FeedbackType.TimePenalty,   Duration = 0.4f,  Volume = 0.6f, Color = TimeTrapColor,                   Particles = 20, Speed = 4.0f,
                     Wave = t => Square(Mathf.Lerp(600.0f, 200.0f, t / 0.4f), t) * 0.5f * Decay(t, 0.2f) },
            new () { Type = FeedbackType.Bumper,        Duration = 0.3f,  Volume = 0.7f, Color = BumperColor,                     Particles = 16, Speed = 6.0f,
                     Wave = t => Tone(Mathf.Lerp(420.0f, 160.0f, t / 0.3f) + 25.0f * Mathf.Sin(t * 60.0f), t) * Decay(t, 0.1f) },
            new () { Type = FeedbackType.Boost,         Duration = 0.4f,  Volume = 0.6f, Color = BoostPadColor,                   Particles = 18, Speed = 5.0f,
                     Wave = t => (0.6f * Tone(Mathf.Lerp(200.0f, 900.0f, t / 0.4f), t) + 0.4f * Noise(t)) * Decay(t, 0.15f) },
            new () { Type = FeedbackType.Knock,         Duration = 0.2f,  Volume = 0.7f, Color = new Color(0.85f, 0.85f, 0.85f), Particles = 10, Speed = 4.0f,
                     Wave = t => (Tone(90.0f, t) + (t < 0.03f ? Noise(t) : 0.0f)) * Decay(t, 0.06f) },
            new () { Type = FeedbackType.Damage,        Duration = 0.3f,  Volume = 0.6f, Color = new Color(1.0f, 0.2f, 0.2f),  Particles = 16, Speed = 5.0f,
                     Wave = t => (0.6f * Square(140.0f, t) + 0.4f * Noise(t)) * Decay(t, 0.1f) },
            new () { Type = FeedbackType.EnemyDefeated, Duration = 0.2f,  Volume = 0.6f, Color = new Color(0.8f, 0.5f, 1.0f),  Particles = 24, Speed = 5.0f,
                     Wave = t => Tone(Mathf.Lerp(700.0f, 1400.0f, t / 0.12f), t) * Decay(t, 0.06f) },
            new () { Type = FeedbackType.Win,           Duration = 0.9f,  Volume = 0.7f, Color = new Color(1.0f, 0.85f, 0.2f), Particles = 60, Speed = 7.0f,
                     Wave = t => Arpeggio(t, 0.12f, 523.0f, 659.0f, 784.0f, 1047.0f) * Decay(Mathf.Max(t - 0.36f, 0.0f), 0.35f) },
            new () { Type = FeedbackType.Lose,          Duration = 0.7f,  Volume = 0.7f, Color = new Color(0.4f, 0.4f, 0.45f), Particles = 30, Speed = 3.0f,
                     Wave = t => Arpeggio(t, 0.18f, 392.0f, 330.0f, 262.0f) * Decay(Mathf.Max(t - 0.36f, 0.0f), 0.25f) }
        };

        private static readonly System.Random SoundNoise = new (7);


        // Sits in the level's HUD: the sound source, the particle burst and the event wiring.
        private static GameObject CreateFeedbackPrefab()
        {
            EnsureFolder("Assets", "Audio");
            EnsureFolder("Assets/Audio", "Generated");
            ParticleSystem burst = CreateBurstPrefab();

            GameObject feedback = new ("Feedback", typeof(RectTransform));
            AudioSource audioSource = feedback.AddComponent<AudioSource>();
            audioSource.playOnAwake  = false;
            audioSource.spatialBlend = 0.0f;
            feedback.AddComponent<LevelFeedback>();

            SerializedObject player = new (feedback.AddComponent<FeedbackPlayer>());
            player.FindProperty("_burstPrefab").objectReferenceValue = burst;
            SerializedProperty entries = player.FindProperty("_entries");
            FeedbackSetting[] settings = CreateFeedbackSettings();
            entries.arraySize = settings.Length;

            for (int i = 0; i < settings.Length; i++)
            {
                FeedbackSetting setting = settings[i];
                SerializedProperty entry = entries.GetArrayElementAtIndex(i);

                entry.FindPropertyRelative("Type")     .enumValueIndex       = (int) setting.Type;
                entry.FindPropertyRelative("Clip")     .objectReferenceValue = CreateSound(setting);
                entry.FindPropertyRelative("Volume")   .floatValue           = setting.Volume;
                entry.FindPropertyRelative("Color")    .colorValue           = setting.Color;
                entry.FindPropertyRelative("Particles").intValue             = setting.Particles;
                entry.FindPropertyRelative("Speed")    .floatValue           = setting.Speed;
            }
            player.ApplyModifiedPropertiesWithoutUndo();

            return SavePrefab(feedback, UIFolder + "Feedback.prefab");
        }

        // Sparks that fly out, fall a little and fade; each event colours them.
        private static ParticleSystem CreateBurstPrefab()
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Effects/Feedback Particle.mat");
            if (material == null)
            {
                material = new Material(Shader.Find("Sprites/Default"))
                {
                    mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Images/Effects/Glow.png")
                };
                AssetDatabase.CreateAsset(material, "Assets/Materials/Effects/Feedback Particle.mat");
            }

            GameObject burstObject = new ("Feedback Burst");
            ParticleSystem burst = burstObject.AddComponent<ParticleSystem>();
            burst.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            ParticleSystem.MainModule main = burst.main;
            main.duration        = 1.0f;
            main.loop            = false;
            main.playOnAwake     = false;
            main.startLifetime   = new ParticleSystem.MinMaxCurve(0.4f, 0.8f);
            main.startSize       = new ParticleSystem.MinMaxCurve(0.15f, 0.35f);
            main.gravityModifier = 0.6f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.stopAction      = ParticleSystemStopAction.Destroy;
            main.maxParticles    = 100;

            ParticleSystem.EmissionModule emission = burst.emission;
            emission.rateOverTime = 0.0f;

            ParticleSystem.ShapeModule shape = burst.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius    = 0.3f;

            ParticleSystem.ColorOverLifetimeModule fade = burst.colorOverLifetime;
            fade.enabled = true;
            Gradient gradient = new ();
            gradient.SetKeys
            (
                new [] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 1.0f) },
                new [] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
            );
            fade.color = gradient;

            ParticleSystem.SizeOverLifetimeModule shrink = burst.sizeOverLifetime;
            shrink.enabled = true;
            shrink.size    = new ParticleSystem.MinMaxCurve(1.0f, AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 0.2f));

            burstObject.GetComponent<ParticleSystemRenderer>().sharedMaterial = material;

            return SavePrefab(burstObject, "Assets/Prefabs/Main Scene/Effects/Feedback Burst.prefab").GetComponent<ParticleSystem>();
        }


        // ---------- Sound synthesis ----------

        private static AudioClip CreateSound(FeedbackSetting setting)
        {
            string path = SoundsFolder + setting.Type + ".wav";
            int count = Mathf.CeilToInt(setting.Duration * SampleRate);
            short[] samples = new short[count];

            for (int i = 0; i < count; i++)
            {
                float time = (float) i / SampleRate;
                float attack = Mathf.Clamp01(time / 0.005f);
                float release = Mathf.Clamp01((setting.Duration - time) / 0.02f);
                samples[i] = (short) (Mathf.Clamp(setting.Wave(time) * attack * release * 0.7f, -1.0f, 1.0f) * short.MaxValue);
            }
            WriteWav(path, samples);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            return AssetDatabase.LoadAssetAtPath<AudioClip>(path);
        }

        private static void WriteWav(string path, short[] samples)
        {
            using BinaryWriter writer = new (File.Create(path));
            int dataSize = samples.Length * 2;

            writer.Write("RIFF".ToCharArray());
            writer.Write(36 + dataSize);
            writer.Write("WAVEfmt ".ToCharArray());
            writer.Write(16);
            writer.Write((short) 1);
            writer.Write((short) 1);
            writer.Write(SampleRate);
            writer.Write(SampleRate * 2);
            writer.Write((short) 2);
            writer.Write((short) 16);
            writer.Write("data".ToCharArray());
            writer.Write(dataSize);
            foreach (short sample in samples) writer.Write(sample);
        }

        private static float Tone(float frequency, float time) => Mathf.Sin(2.0f * Mathf.PI * frequency * time);

        private static float Square(float frequency, float time) => Mathf.Sign(Tone(frequency, time)) * 0.6f;

        private static float Noise(float _) => (float) (SoundNoise.NextDouble() * 2.0 - 1.0);

        private static float Decay(float time, float halfLife) => Mathf.Pow(0.5f, time / halfLife);

        // Plays the notes one after another, the last one keeps ringing.
        private static float Arpeggio(float time, float noteLength, params float[] notes)
        {
            int note = Mathf.Min((int) (time / noteLength), notes.Length - 1);
            float noteTime = time - note * noteLength;

            return Tone(notes[note], time) * Decay(noteTime, noteLength * 1.5f);
        }
    }
}
