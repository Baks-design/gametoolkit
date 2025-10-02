using System;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public interface IPlayerStatus
    {
        PlayerStatusData CurrentStatus { get; }

        void UpdateStatus(Func<PlayerStatusData, PlayerStatusData> statusUpdate);
        void HandleStatusChange(PlayerStatusData newStatus, PlayerStatusData previousStatus);
    }
}
