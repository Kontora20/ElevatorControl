using ElevatorControl.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevatorControl.Services
{
    public class ElevatorManager : IElevatorManager
    {
        private readonly IOptions<ElevatorConfig> _elevatorConfig;

        public int NumberOfFloors { get; set; }
        public int NumberOfElevators { get; set; }
        public int GroundFloorIndex { get; set; }
        public int WaitTimeForDoorsMs { get; set; }
        public int WaitTimeForPassengersMs { get; set; }
        public int WaitTimeToReachFloorMs { get; set; }
        public Dictionary<int, Elevator> Elevators { get; set; }

        public ElevatorManager(IOptions<ElevatorConfig> elevatorConfig)
        {
            _elevatorConfig = elevatorConfig;
        }

        public void Initialize()
        {
            NumberOfElevators = _elevatorConfig.Value.NumberOfElevators;
            NumberOfFloors = _elevatorConfig.Value.NumberOfFloors;
            GroundFloorIndex = _elevatorConfig.Value.GroundFloorIndex;
            WaitTimeForDoorsMs = _elevatorConfig.Value.WaitTimeForDoorsMs;
            WaitTimeForPassengersMs = _elevatorConfig.Value.WaitTimeForPassengersMs;
            WaitTimeToReachFloorMs = _elevatorConfig.Value.WaitTimeToReachFloorMs;

            Elevators = new Dictionary<int, Elevator>();

            for (int i = 0; i < NumberOfElevators; i++)
            {
                Elevators.Add(i, new Elevator(i, GroundFloorIndex));
            }
        }

        public void MoveElevatorToFloor(int elevatorId, int destinationFloor)
        {
            if (destinationFloor < GroundFloorIndex || destinationFloor > NumberOfFloors)
            {
                throw new ArgumentException("The chosen destination floor is invalid");
            }

            var elevatorToMove = GetElevatorById(elevatorId);

            // Would be nice to implement a queue of some sort to allow for continous elevator calling...
            if (elevatorToMove.IsCurrentlyTaken)
            {
                throw new Exception("Elevator is currently taken and no queue is implemented");
            }

            Task.Run(() => InitiateMove(destinationFloor, elevatorToMove));
        }

        private async Task InitiateMove(int destinationFloor, Elevator elevator)
        {
            elevator.IsCurrentlyTaken = true;

            if (destinationFloor > elevator.CurrentFloor)
            {
                await MoveUp(destinationFloor, elevator);
            }
            else if (destinationFloor < elevator.CurrentFloor)
            {
                await MoveDown(destinationFloor, elevator);
            }
            else
            {
                Console.WriteLine("Seems like elevator has nowhere to move");
                await OperateDoors(elevator);                
            }

            elevator.IsCurrentlyTaken = false;
        }

        private async Task MoveDown(int destinationFloor, Elevator elevator)
        {
            await OperateDoors(elevator);

            AddLogEntry(elevator, $"Elevator {elevator.Id} started to move down from floor {elevator.CurrentFloor} to floor {destinationFloor}");
            elevator.ElevatorStatus = ElevatorStatus.GOING_DOWN;
            for (int i = elevator.CurrentFloor; i > destinationFloor; i--)
            {
                await Task.Delay(WaitTimeToReachFloorMs);
                elevator.CurrentFloor--;
                AddLogEntry(elevator, $"Elevator {elevator.Id} is currently descending and is on floor {elevator.CurrentFloor}");
            }

            await OperateDoors(elevator);
        }

        private async Task MoveUp(int destinationFloor, Elevator elevator)
        {
            await OperateDoors(elevator);

            AddLogEntry(elevator, $"Elevator {elevator.Id} started to move up from floor {elevator.CurrentFloor} to floor {destinationFloor}");
            elevator.ElevatorStatus = ElevatorStatus.GOING_UP;
            for (int i = elevator.CurrentFloor; i < destinationFloor; i++)
            {
                await Task.Delay(WaitTimeToReachFloorMs);
                elevator.CurrentFloor++;
                AddLogEntry(elevator, $"Elevator {elevator.Id} is currently ascending and is on floor {elevator.CurrentFloor}");
            }

            await OperateDoors(elevator);
        }

        private async Task OperateDoors(Elevator elevator)
        {
            AddLogEntry(elevator, $"Elevator {elevator.Id} door is opening.");
            elevator.ElevatorStatus = ElevatorStatus.DOOR_OPENING;
            await Task.Delay(WaitTimeForDoorsMs);

            AddLogEntry(elevator, $"Elevator {elevator.Id} is waiting for passengers to get in / get out");
            elevator.ElevatorStatus = ElevatorStatus.IDLE;
            await Task.Delay(WaitTimeForPassengersMs);

            AddLogEntry(elevator, $"Elevator {elevator.Id} door is closing.");
            elevator.ElevatorStatus = ElevatorStatus.DOOR_CLOSING;
            await Task.Delay(WaitTimeForDoorsMs);
        }

        public Elevator GetElevatorById(int elevatorId)
        {
            if (!Elevators.ContainsKey(elevatorId))
            {
                throw new ArgumentOutOfRangeException("Elevator not found by given id");
            }

            Elevators.TryGetValue(elevatorId, out Elevator elevator);
            return elevator;
        }

        // Initial idea was to add a logging provider, log everything into a file, afterwards filter the file by timestamps and return
        // log entries. That felt kind of tedious, so for this case, I've decided to store everything in the elevator.
        private void AddLogEntry(Elevator elevator, string logEntry)
        {
            elevator.LogEntries.Add(new ElevatorLogEntry(logEntry));
            Console.WriteLine(logEntry);
        }

        public List<ElevatorLogEntry> GetElevatorLogs(int elevatorId, int dateTimeOffsetMinutes)
        {
            var elevator = GetElevatorById(elevatorId);
            var oldestDate = DateTime.UtcNow.AddMinutes(-dateTimeOffsetMinutes);
            return elevator.LogEntries.Where(t => t.Time >= oldestDate).ToList();
        }
    }
}
