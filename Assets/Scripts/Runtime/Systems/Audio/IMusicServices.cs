using UnityEngine;

namespace GameToolkit.Runtime.Systems.Audio
{
    public interface IMusicServices
    {
        void AddToPlaylist(AudioClip clip);
        void Play(AudioClip clip);
        void PlayNextTrack();
        void Clear();
    }
}
