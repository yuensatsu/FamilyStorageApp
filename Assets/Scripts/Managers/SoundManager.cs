using UnityEngine;

namespace FamilyStorageApp.Managers
{
    /// <summary>
    /// BGMとSEの再生を担当します。
    /// まずは最低限の音量ON/OFFと再生だけを用意しています。
    /// </summary>
    public class SoundManager : MonoBehaviour
    {
        [SerializeField]
        private AudioSource bgmSource;

        [SerializeField]
        private AudioSource seSource;

        [SerializeField]
        private bool isSoundEnabled = true;

        public void PlayBgm(AudioClip clip)
        {
            if (!isSoundEnabled || bgmSource == null || clip == null)
            {
                return;
            }

            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.Play();
            Debug.Log("SoundManager: BGMを再生しました。");
        }

        public void StopBgm()
        {
            if (bgmSource == null)
            {
                return;
            }

            bgmSource.Stop();
            Debug.Log("SoundManager: BGMを停止しました。");
        }

        public void PlaySe(AudioClip clip)
        {
            if (!isSoundEnabled || seSource == null || clip == null)
            {
                return;
            }

            seSource.PlayOneShot(clip);
            Debug.Log("SoundManager: SEを再生しました。");
        }

        public void SetSoundEnabled(bool enabled)
        {
            isSoundEnabled = enabled;

            if (bgmSource != null)
            {
                bgmSource.mute = !isSoundEnabled;
            }

            if (seSource != null)
            {
                seSource.mute = !isSoundEnabled;
            }

            Debug.Log($"SoundManager: 音量ON/OFFを切り替えました。Enabled: {isSoundEnabled}");
        }
    }
}
