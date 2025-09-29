namespace GameToolkit.Runtime.Game.Systems.Sound
{
    public interface ISoundServices
    {
        SoundBuilder CreateSoundBuilder();
        SoundEmitter Get();
        bool CanPlaySound(SoundData data);
        void ReturnToPool(SoundEmitter soundEmitter);
        void StopAll();
    }
}
