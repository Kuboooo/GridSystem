using UnityEngine;

namespace Audio {
    public class AudioMixer : MonoBehaviour
    {
        public static AudioMixer Instance;

        public AudioSource musicSource;
        public UnityEngine.Audio.AudioMixer audioMixer;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            PlayAmbientMusic();
        }

        public void PlayAmbientMusic()
        {
            musicSource.Play();
        }

        public void SetMusicVolume(float volume)
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        }
    }
}