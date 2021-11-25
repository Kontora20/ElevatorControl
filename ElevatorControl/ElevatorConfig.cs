namespace ElevatorControl
{
    public class ElevatorConfig
    {
        public int NumberOfFloors { get; set; }
        public int NumberOfElevators { get; set; }
        public int GroundFloorIndex { get; set; }

        public int WaitTimeForDoorsMs { get; set; }

        public int WaitTimeForPassengersMs { get; set; }

        public int WaitTimeToReachFloorMs { get; set; }
    }
}
