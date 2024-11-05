using budget.core.Entities;
using budget.core.Factories.Interfaces;
using budget.core.Models;
using budget.core.Models.Users;
using budget.core.Repositories.Interfaces;
using budget.core.Services;
using MongoDB.Bson;
using Moq;

namespace budget.test.core {
  public class PostingServiceTest {
    private Mock<IFactory<Item, ItemView>> IFactoryMock { get; } = new Mock<IFactory<Item, ItemView>>();
    private Mock<IPostingRepository<Item>> IPostingRepositoryMock { get; } = new Mock<IPostingRepository<Item>>();
    private Mock<IEnumerableRepository<Item>> IEnumerableRepositoryMock { get; } = new Mock<IEnumerableRepository<Item>>();

    [Fact]
    public void UpsertItem_NewItem_ReturnTrue() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var itemView = new ItemView(new Item());
      var postingService = new PostingService<Item, ItemView>(IPostingRepositoryMock.Object, IFactoryMock.Object, IEnumerableRepositoryMock.Object);
      IPostingRepositoryMock.Setup(x => x.UpdateItem(It.IsAny<AppUser>(), It.IsAny<Item>(), It.IsAny<string>(), null)).Returns(true);
      IPostingRepositoryMock.Setup(x => x.AddItem(It.IsAny<AppUser>(), It.IsAny<Item>(), It.IsAny<string>(), null)).Returns(true);
      IFactoryMock.Setup(x => x.CreateItem(It.IsAny<ItemView>())).Returns(new Item());

      // Act
      var result = postingService.UpsertItem(appUser, itemView, "collectionName", out bool isAdded);

      // Assert
      Assert.True(result);
      Assert.True(isAdded);
    }

    [Fact]
    public void UpsertItem_ExistingItem_ReturnTrue() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var itemView = new ItemView(new Item() { Id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 }) });
      var postingService = new PostingService<Item, ItemView>(IPostingRepositoryMock.Object, IFactoryMock.Object, IEnumerableRepositoryMock.Object);
      IPostingRepositoryMock.Setup(x => x.UpdateItem(It.IsAny<AppUser>(), It.IsAny<Item>(), It.IsAny<string>(), null)).Returns(true);
      IPostingRepositoryMock.Setup(x => x.AddItem(It.IsAny<AppUser>(), It.IsAny<Item>(), It.IsAny<string>(), null)).Returns(true);
      IEnumerableRepositoryMock.Setup(x => x.GetItem(It.IsAny<ObjectId>(), It.IsAny<string>())).Returns(new Item());
      IFactoryMock.Setup(x => x.CreateItem(It.IsAny<ItemView>())).Returns(new Item());

      // Act
      var result = postingService.UpsertItem(appUser, itemView, "collectionName", out bool isAdded);

      // Assert
      Assert.True(result);
      Assert.False(isAdded);
    }
  }
}