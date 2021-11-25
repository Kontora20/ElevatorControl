using ElevatorControl.Models;
using System.Collections.Generic;

namespace ElevatorControl.Services
{
    public interface IElevatorService
    {
        public void MoveElevator(int elevatorId, int destinationFloor);

        public Elevator GetElevatorInfo(int elevatorId);

        public List<ElevatorLogEntry> GetElevatorLogs(int elevatorId, int beforeTimeMinutes);
    }
}
