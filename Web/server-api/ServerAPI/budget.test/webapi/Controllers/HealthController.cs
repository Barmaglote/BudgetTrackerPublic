using budget.core.Factories.Interfaces;
using Moq;
using utils.Exceptions;
using webapi.Controllers;

namespace budget.test.webapi {
  public class HealthControllerTest {
    private Mock<IUserFactory> IUserFactoryMock { get; } = new Mock<IUserFactory>();

    [Fact]
    public void GetState_ReturnsTrue() {
      // Arrange
      var healthController = new HealthController(IUserFactoryMock.Object);

      // Act
      var result = healthController.GetState();

      // Assert
      Assert.True(result);
    }

    [Fact]
    public void GetError_ReturnsApiException() {
      // Arrange
      var healthController = new HealthController(IUserFactoryMock.Object);

      // Act
      var result = () => healthController.GetError();

      // Assert
      Assert.Throws<ApiException>(() => result());
    }

    
  }
}