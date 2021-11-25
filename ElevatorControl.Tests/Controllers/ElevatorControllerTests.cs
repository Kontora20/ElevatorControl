using ElevatorControl.Controllers;
using ElevatorControl.Models;
using ElevatorControl.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ElevatorControl.Tests.Controllers
{
    public class ElevatorControllerTests
    {
        private readonly Mock<IElevatorService> _elevatorServiceMock;
        private readonly ElevatorController _target;

        public ElevatorControllerTests()
        {
            _elevatorServiceMock = new Mock<IElevatorService>();
            _target = new ElevatorController(_elevatorServiceMock.Object);    
        }

        [Fact]
        public void MoveElevator_ReturnsOk_OnMoveElevatorWithParams()
        {
            var result = _target.MoveElevator(0, 5) as OkResult;

            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public void GetElevator_ReturnsElevator_OnCorrectElevatorId()
        {
            var elevator = new Elevator(0, 1);

            _elevatorServiceMock.Setup(x => x.GetElevatorInfo(0)).Returns(elevator);

            var actionResult = _target.GetElevator(0);
            var result = actionResult.Result as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(elevator, result.Value);
        }
    }
}
