using System;

namespace ElevatorControl.Models
{
    public class ElevatorLogEntry
    {
        public DateTime Time { get; set; }
        public string Information { get; set; }

        public ElevatorLogEntry(string information)
        {
            Time = DateTime.UtcNow;
            Information = information;
        }
    }
}
