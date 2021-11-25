using ElevatorControl.Models;
using ElevatorControl.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace ElevatorControl.Tests.Services
{
    [Collection("Sequential")]
    public class ElevatorManagerTests
    {
        private readonly IOptions<ElevatorConfig> _elevatorConfig;
        private readonly ElevatorManager _target;

        public ElevatorManagerTests()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();

            _elevatorConfig = Options.Create(configuration.GetSection("ElevatorConfig").Get<ElevatorConfig>());

            _target = new ElevatorManager(_elevatorConfig);
            _target.Initialize();
        }

        [Fact]
        public void MoveElevatorToFloor_MovesToExpectedFloor_OnCorrectFloorGiven()
        {
            var expectedFloor = 3;
            var elevatorId = 0;

            var elevator = GetFinishedMovingElevator(elevatorId, expectedFloor);

            Assert.Equal(expectedFloor, elevator.CurrentFloor);
        }

        private Elevator GetFinishedMovingElevator(int elevatorId, int expectedFloor)
        {
            _target.MoveElevatorToFloor(elevatorId, expectedFloor);

            var movedElevator = _target.GetElevatorById(elevatorId);

            // I have no idea if unit tests should be written this way
            while (movedElevator.CurrentFloor != expectedFloor)
            {
                movedElevator = _target.GetElevatorById(elevatorId);
                Task.Delay(1000);
            }

            return movedElevator;
        }

        [Fact]
        public void MoveElevatorToFloor_ThrowsException_OnIncorrectFloorGiven()
        {
            var incorrectFloor = _elevatorConfig.Value.GroundFloorIndex - 1;
            var elevatorId = 0;

            Assert.Throws<ArgumentException>(() => _target.MoveElevatorToFloor(elevatorId, incorrectFloor));
        }

        [Fact]
        public void GetElevatorById_ReturnElevator_WithProperElevatorId()
        {
            var elevatorId = 0;
            var elevator = new Elevator(elevatorId, _elevatorConfig.Value.GroundFloorIndex);

            var result = _target.GetElevatorById(elevatorId);

            Assert.NotNull(result);
            Assert.Equal(elevator.Id, result.Id);
            Assert.Equal(elevator.ElevatorStatus, result.ElevatorStatus);
            Assert.Equal(elevator.CurrentFloor, result.CurrentFloor);
        }

        [Fact]
        public void GetElevatorById_ThrowsException_WithIncorrectElevatorId()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _target.GetElevatorById(-1));
        }

        [Fact]
        public void GetElevatorLogs_ReturnsLogs_OnElevatorAction()
        {
            var expectedFloor = 1;
            var elevatorId = 0;

            var elevator = GetFinishedMovingElevator(elevatorId, expectedFloor);
            var elevatorLogs = _target.GetElevatorLogs(elevator.Id, 1);

            Assert.NotEmpty(elevatorLogs);
        }
    }
}
