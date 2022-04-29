using UnityEngine;

namespace DigitalRuby.SoundManagerNamespace
{
    public class SoundPlayer : MonoBehaviour
    {
        public AudioSource[] SoundAudioSources;
        public AudioSource[] MusicAudioSources;
        private GameObject[] objectsWithSoundSource;
        private GameObject[] objectsWithMusicSource;

        private int soundsToProcess;

        private void Awake()
        {
            soundsToProcess = 0;

            FindMusicSources();
            FindSoundSources();
        }

        public void PlaySound(AudioClip sound, int source)
        {
            SoundAudioSources[source].PlayOneShotSoundManaged(sound);
        }

        public void PlayMusic(int index)
        {
            MusicAudioSources[index].PlayLoopingMusicManaged(1.0f, 1.0f, true);
        }

        public void FindMusicSources()
        {
            objectsWithMusicSource = GameObject.FindGameObjectsWithTag("MusicSource");
            for (int i = 0; i < objectsWithMusicSource.Length; i++)
            {
                MusicAudioSources[i] = objectsWithMusicSource[i].GetComponent<AudioSource>();
            }
        }

        public void FindSoundSources()
        {
            objectsWithSoundSource = GameObject.FindGameObjectsWithTag("MusicSource");
            for (int i = 0; i < objectsWithMusicSource.Length; i++)
            {
                SoundAudioSources[i] = objectsWithSoundSource[i].GetComponent<AudioSource>();
            }
        }
    }
}
