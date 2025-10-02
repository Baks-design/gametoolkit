using System;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public interface IHealable
    {
        event Action<int> OnHealthIncreased;

        void TakeHeal(int cure);
    }
}
