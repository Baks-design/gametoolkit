using UnityEngine;

namespace GameToolkit.Runtime.Game.Systems.Sound
{
    public interface IMusicServices
    {
        void AddToPlaylist(AudioClip clip);
        void Play(AudioClip clip);
        void PlayNextTrack();
        void Clear();
    }
}
