using budget.core.Entities;
using budget.core.Factories.Interfaces;
using budget.core.Models;
using budget.core.Models.Users;
using budget.core.Repositories.Interfaces;
using budget.core.Services;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;

namespace budget.test.core {
  public class DeletingServiceTest {
    private Mock<IPostingRepository<Item>> PostingRepositoryMock { get; } = new Mock<IPostingRepository<Item>>();
    private Mock<IEnumerableRepository<Item>> EnumerableRepositoryMock { get; } = new Mock<IEnumerableRepository<Item>>();
    private Mock<IFactory<Item, ItemView>> FactoryMock { get; } = new Mock<IFactory<Item, ItemView>>();

    [Fact]
    public void DeleteItem_ValidInput_ReturnTrue() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var item = new Item();
      var itemView = new ItemView(item);

      var deletingService = new DeletingService<Item, ItemView>(PostingRepositoryMock.Object, EnumerableRepositoryMock.Object, FactoryMock.Object);

      FactoryMock.Setup(x => x.CreateItem(It.IsAny<ItemView>())).Returns<ItemView>((itemView) => {
        return item;
      });

      EnumerableRepositoryMock.Setup(x => x.GetItem(It.IsAny<ObjectId>(), It.IsAny<string>())).Returns<ObjectId, string>((objectId, collectionName) => {
        return item;
      });

      PostingRepositoryMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<Item>(), It.IsAny<string>(), null)).Returns<AppUser, Item, string, IClientSessionHandle>((appUser, item, collectionName, session) => {
        return appUser != null && itemView != null && !string.IsNullOrEmpty(collectionName);
      });

      // Action
      var result = deletingService.DeleteItem(appUser, itemView, "collection");

      // Assert
      Assert.True(result);
    }

    [Fact]
    public void DeleteItem_InvalidInput_ReturnFalse() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var deletingService = new DeletingService<Item, ItemView>(PostingRepositoryMock.Object, EnumerableRepositoryMock.Object, FactoryMock.Object);
      Item? item = null;
      var itemView = new ItemView(new Item());

      FactoryMock.Setup(x => x.CreateItem(It.IsAny<ItemView>())).Returns<ItemView>((itemView) => {
        return new Item();
      });

      EnumerableRepositoryMock.Setup(x => x.GetItem(It.IsAny<ObjectId>(), It.IsAny<string>())).Returns<ObjectId, string>((objectId, collectionName) => {
        return item;
      });

      PostingRepositoryMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<Item>(), It.IsAny<string>(), null)).Returns<AppUser, Item, string, IClientSessionHandle>((appUser, item, collectionName, session) => {
        return appUser != null && itemView != null && !string.IsNullOrEmpty(collectionName);
      });

      // Action
      var result = deletingService.DeleteItem(appUser, itemView, "collection");

      // Assert
      Assert.False(result);
    }
  }
}