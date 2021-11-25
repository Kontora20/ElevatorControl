using ElevatorControl.Models;
using System.Collections.Generic;

namespace ElevatorControl.Services
{
    public class ElevatorService : IElevatorService
    {
        private readonly IElevatorManager _elevatorManager;

        public ElevatorService(IElevatorManager elevatorManager)
        {
            _elevatorManager = elevatorManager;
        }

        public void MoveElevator(int elevatorId, int destinationFloor)
        {
            _elevatorManager.MoveElevatorToFloor(elevatorId, destinationFloor);
        }

        public Elevator GetElevatorInfo(int elevatorId)
        {
            return _elevatorManager.GetElevatorById(elevatorId);
        }

        public List<ElevatorLogEntry> GetElevatorLogs(int elevatorId, int  beforeTimeMinutes)
        {
            return _elevatorManager.GetElevatorLogs(elevatorId, beforeTimeMinutes);
        }
    }
}
