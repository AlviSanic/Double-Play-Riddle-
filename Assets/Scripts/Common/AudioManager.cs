using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common {
    public class AudioManager : MonoBehaviour {

        public static readonly float Volume = 1.0f;

        public static AudioManager Instance { private set; get; } = null;
        public static void PlaySfx(string clipName, float volume = 1f) => Instance?.PlaySfxInternal(clipName, volume);
        public static void PlayBgm(string clipName, float volume = 1f) => Instance?.PlayBgmInternal(clipName, volume);

        [SerializeField]
        private List<AudioClip> clips = new();

        private Dictionary<string, AudioClip> _clipLookup = new();
        private Dictionary<string, AudioSource> _bgmsPlaying = new();

        private void PlaySfxInternal(string clipName, float volume = 1f) {
            var source = CreateSource(clipName);
            if (source == null) {
                Debug.LogError($"No Audio Clip ({clipName}).");
                return;
            }
            source.volume = volume;
            source.loop = false;
            source.Play();
            DestroySourceWhenFinished(source, false);
        }

        private void PlayBgmInternal(string clipName, float volume = 1f) {
            var source = CreateSource(clipName);
            if (source == null) {
                Debug.LogError($"No Audio Clip ({clipName}).");
                return;
            }
            StopBgm(clipName);
            source.volume = volume;
            source.loop = true;
            source.Play();
            _bgmsPlaying[clipName] = source;
        }

        public void StopBgm(string clipName) {
            if (_bgmsPlaying.ContainsKey(clipName) && _bgmsPlaying[clipName] != null) {
                _bgmsPlaying[clipName].Stop();
                DestroySourceWhenFinished(_bgmsPlaying[clipName], true);
                _bgmsPlaying.Remove(clipName);
            }
        }

        public void StopAll() {
            foreach (var bgm in _bgmsPlaying.Keys) {
                StopBgm(bgm);
            }
        }

        private AudioSource CreateSource(string clipName) {
            if (!_clipLookup.ContainsKey(clipName)) {
                return null;
            }

            var g = new GameObject(clipName);
            var source = g.AddComponent<AudioSource>();

            g.transform.SetParent(transform);
            source.clip = _clipLookup[clipName];
            return source;
        }

        private void Awake() {
            if (Instance != null) {
                DestroyImmediate(this);
                return;
            }
            Instance = this;
            foreach (var c in clips) {
                _clipLookup[c.name] = c;
            }
        }

        private void DestroySourceWhenFinished(AudioSource source, bool forced) {
            StartCoroutine(DestroySourceWhenFinishedRoutine(source, forced));
        }

        private IEnumerator DestroySourceWhenFinishedRoutine(AudioSource source, bool forced) {
            if ((!source.isPlaying || source.clip == null || source.loop) && !forced) {
                yield break;
            }
            yield return new WaitForSeconds(source.clip.length);
            Destroy(source.gameObject);
        }
    }
}
