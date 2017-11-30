using UnityEngine;
using Gunhouse;

namespace Necrosoft
{
    public static class Choom
    {
        static GameObject gameObject;
        static bool enabled;

        static int loadedHash;
        static AudioSource musicLayer;

        static int MAX_EFFECT_LAYERS = 50;
        static int[] loadedEffectHases = new int[MAX_EFFECT_LAYERS];
        static AudioSource[] effectLayers = new AudioSource[MAX_EFFECT_LAYERS];
        static int frameDelay = 2;

        static void Setup()
        {
            enabled = true;

            gameObject = new GameObject("Chooms");
            gameObject.AddComponent<AudioListener>();
            GameObject.DontDestroyOnLoad(gameObject);

            musicLayer = gameObject.AddComponent<AudioSource>();
            for (int i = 0; i < MAX_EFFECT_LAYERS; ++i) effectLayers[i] = gameObject.AddComponent<AudioSource>();
        }

        #region Music

        public static void Play(string file)
        {
            if (!enabled) Setup();

            if (loadedHash == file.GetHashCode()) return; /* NOTE(shane): This assumed the clip is already loaded/playing. */

            loadedHash = file.GetHashCode();
            musicLayer.clip = GetClip(file);
            musicLayer.loop = true;
            musicLayer.Play();
        }

        public static void Stop()
        {
            if (!enabled) Setup();

            musicLayer.Stop();
        }

        #endregion

        #region Effects

        public static void PlayEffect(Gunhouse.SoundInfo effect, bool loop = false)
        {
            if (!enabled) Setup();

            if (effect.lastPlayed + frameDelay > Time.frameCount) return;

            for (int i = 0; i < MAX_EFFECT_LAYERS; ++i)
            {
                if (loadedEffectHases[i] != effect.file.GetHashCode()) continue;
                if (effectLayers[i].isPlaying) continue;
                effectLayers[i].volume = EffectVolume * effect.volume;
                effectLayers[i].loop = loop;
                effectLayers[i].Play();
                effect.lastPlayed = Time.frameCount;

                return;
            }

            for (int i = 0; i < MAX_EFFECT_LAYERS; ++i)
            {
                if (effectLayers[i].isPlaying) continue;

                loadedEffectHases[i] = effect.file.GetHashCode();
                effectLayers[i].clip = GetClip("SoundEffects/" + effect.file);
                effectLayers[i].volume = EffectVolume * effect.volume;
                effectLayers[i].loop = loop;
                effectLayers[i].Play();
                effect.lastPlayed = Time.frameCount;
                return;
            }
        }

        public static void PlayEffect(Gunhouse.SoundInfo effect, float volumePercent, bool loop = false)
        {
            if (!enabled) Setup();

            if (effect.lastPlayed + frameDelay > Time.frameCount) return;

            for (int i = 0; i < MAX_EFFECT_LAYERS; ++i)
            {
                if (loadedEffectHases[i] != effect.file.GetHashCode()) continue;
                if (effectLayers[i].isPlaying) continue;
                effectLayers[i].volume = EffectVolume * (effect.volume * volumePercent);
                effectLayers[i].loop = loop;
                effectLayers[i].Play();
                effect.lastPlayed = Time.frameCount;

                return;
            }

            for (int i = 0; i < MAX_EFFECT_LAYERS; ++i)
            {
                if (effectLayers[i].isPlaying) continue;

                loadedEffectHases[i] = effect.file.GetHashCode();
                effectLayers[i].clip = GetClip("SoundEffects/" + effect.file);
                effectLayers[i].volume = EffectVolume * (effect.volume * volumePercent);
                effectLayers[i].loop = loop;
                effectLayers[i].Play();
                effect.lastPlayed = Time.frameCount;
                return;
            }
        }

        public static void StopEffect(Gunhouse.SoundInfo effect)
        {
            if (!enabled) Setup();

            for (int i = 0; i < MAX_EFFECT_LAYERS; ++i)
            {
                if (loadedEffectHases[i] != effect.file.GetHashCode()) continue;
                if (!effectLayers[i].isPlaying) continue;

                effectLayers[i].Stop();
                return;
            }
        }

        public static void StopAllEffects()
        {
            if (!enabled) Setup();

            for (int i = 0; i < MAX_EFFECT_LAYERS; ++i) effectLayers[i].Stop();
        }

        #endregion

        #region Volume

        static float pauseVolume = 0.3f;
        static float prePauseVolume;
        static float musicVolume = 0.75f;
        public static float MusicVolume
        {
            get { return musicVolume; }
            set
            {
                if (!enabled) Setup();

                musicVolume = value;
                musicLayer.volume = musicVolume;
            }
        }

        public static float EffectVolume = 0.75f;

        #endregion

        public static void Pause(bool pause = true)
        {
            if (pause) {
                musicLayer.volume = musicVolume * pauseVolume;
                for (int i = 0; i < MAX_EFFECT_LAYERS; ++i) effectLayers[i].Pause();
            }
            else {
                MusicVolume = musicVolume;
                for (int i = 0; i < MAX_EFFECT_LAYERS; ++i) effectLayers[i].UnPause();
            }
        }

        static AudioClip GetClip(string file)
        {
            AudioClip clip = Resources.Load<AudioClip>(file);

            #if BUNDLED
            if (clip == null) {
                return (AudioClip)Downloader.Bundle.LoadAsset(System.IO.Path.GetFileName(file));
            }
            #endif

            return clip;
        }
    }
}
