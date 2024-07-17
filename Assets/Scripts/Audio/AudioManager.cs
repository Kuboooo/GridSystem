using SOs;
using StructureBuilding;
using UnityEngine;

namespace Audio {
    public class AudioManager : MonoBehaviour {
        public static AudioManager instance;
        public AudioSource ambientSource;
        public AudioSource effectsSource;
        public float transitionSpeed = 1.0f;

        [SerializeField]
        private AudioClip newClip;
        private float targetVolume = 1.0f;
        private bool isFadingOut = false;
        private bool isFadingIn = false;

        private void Awake() {
            if (instance == null) {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else {
                Destroy(gameObject);
            }
        }


        private void OnEnable() {
            BuildingPlacer.OnBuildingBuilt += OnBuildingBuilt;
        }

        private void OnBuildingBuilt(object arg1, PreviewBuildingSO arg2) {
            PlaySound();
        }

        private void Update() {
            if (isFadingOut) {
                ambientSource.volume -= transitionSpeed * Time.deltaTime;
                if (ambientSource.volume <= 0) {
                    ambientSource.Stop();
                    ambientSource.volume = 0;
                    isFadingOut = false;
                    PlayNewClip();
                }
            }

            if (isFadingIn) {
                ambientSource.volume += transitionSpeed * Time.deltaTime;
                if (ambientSource.volume >= targetVolume) {
                    ambientSource.volume = targetVolume;
                    isFadingIn = false;
                }
            }
        }

        public void PlayAmbientMusic(AudioClip clip) {
            if (ambientSource.clip == null || !ambientSource.isPlaying) {
                ambientSource.clip = clip;
                ambientSource.volume = 0;
                ambientSource.loop = true;
                ambientSource.Play();
                isFadingIn = true;
            }
            else {
                newClip = clip;
                isFadingOut = true;
            }
        }

        public void StopAmbientMusic() {
            isFadingOut = true;
        }

        private void PlayNewClip() {
            ambientSource.clip = newClip;
            ambientSource.loop = true;
            ambientSource.Play();
            isFadingIn = true;
        }

        public void PlaySound() {
            effectsSource.PlayOneShot(newClip);
        }
    }
}