using System;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class PlayerStatus : IPlayerStatus
    {
        PlayerStatusData currentStatus;

        public PlayerStatusData CurrentStatus => currentStatus;

        public event Action<PlayerStatusData, PlayerStatusData> OnStatusChanged;

        public PlayerStatus(PlayerStatusData initialStatus) => currentStatus = initialStatus;

        public void UpdateStatus(Func<PlayerStatusData, PlayerStatusData> statusUpdate)
        {
            var previousStatus = currentStatus;
            currentStatus = statusUpdate(currentStatus);

            if (!currentStatus.Equals(previousStatus))
            {
                OnStatusChanged?.Invoke(currentStatus, previousStatus);
                HandleStatusChange(currentStatus, previousStatus);
            }
        }

        public void HandleStatusChange(PlayerStatusData newStatus, PlayerStatusData previousStatus)
        {
            if (newStatus.CurrentHealth == previousStatus.CurrentHealth)
                return;
        }
    }
}
