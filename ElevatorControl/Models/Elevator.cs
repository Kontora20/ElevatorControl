using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ElevatorControl.Models
{
    public class Elevator
    {
        public int Id { get; set; } = 0;

        public int CurrentFloor { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ElevatorStatus ElevatorStatus { get; set; }

        [JsonIgnore]
        public List<ElevatorLogEntry> LogEntries { get; set; }

        public bool IsCurrentlyTaken { get; set; }

        public Elevator(int id, int groundFloor)
        {
            CurrentFloor = groundFloor;
            Id = id;
            ElevatorStatus = ElevatorStatus.IDLE;
            LogEntries = new List<ElevatorLogEntry>();
            IsCurrentlyTaken = false;
        }
    }

    public enum ElevatorStatus
    {
        GOING_UP,
        GOING_DOWN,
        IDLE,
        DOOR_CLOSING,
        DOOR_OPENING
    }
}
