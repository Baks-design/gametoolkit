using UnityEngine;

namespace GameToolkit.Runtime.Game.Systems.Sound
{
    [CreateAssetMenu(menuName = "Data/Sound/Library")]
    public class SoundLibraryObject : ScriptableObject
    {
        [field: SerializeField]
        public SoundData FootstepClip { get; private set; }

        [field: SerializeField]
        public SoundData LandingClip { get; private set; }

        [field: SerializeField]
        public SoundData SwimmingClip { get; private set; }

        [field: SerializeField]
        public SoundData JumpingClip { get; private set; }

        [field: SerializeField]
        public SoundData ClimbingClip { get; private set; }

        [field: SerializeField]
        public SoundData DamagingClip { get; private set; }
    }
}
