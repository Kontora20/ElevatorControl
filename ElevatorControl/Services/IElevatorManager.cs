using ElevatorControl.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElevatorControl.Services
{
    public interface IElevatorManager
    {
        public void Initialize();
        public void MoveElevatorToFloor(int elevatorId, int destinationFloor);
        public Elevator GetElevatorById(int elevatorId);

        public List<ElevatorLogEntry> GetElevatorLogs(int elevatorId, int dateTimeOffsetMinutes);
    }
}
