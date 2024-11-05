using budget.core.Entities;
using budget.core.Factories.Interfaces;
using budget.core.Models;
using budget.core.Models.Users;
using budget.core.Repositories.Interfaces;
using budget.core.Services;
using budget.core.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;

namespace budget.test.core {
  public class EnumerableServiceTest {
    private Mock<IEnumerableRepository<Item>> EnumerableRepositoryMock { get; } = new Mock<IEnumerableRepository<Item>>();
    private Mock<IFactory<Item, ItemView>> FactoryMock { get; } = new Mock<IFactory<Item, ItemView>>();
    private Mock<IFilterBuilder<Item>> FilterBuilderMock { get; } = new Mock<IFilterBuilder<Item>>();

    [Fact]
    public void GetItemViews_ValidInput_ReturnItemViews() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var item = new Item();
      var itemView = new ItemView(item);

      var mockItems = new ListAsyncCursor<Item>(new List<Item>() {
          new Item(),
          new Item(),
          new Item(),
          new Item(),
          new Item()
        });

      var mockTotalCount = 5;

      var tableLazyLoadEvent = new TableLazyLoadEvent();
      tableLazyLoadEvent.First = 2;
      tableLazyLoadEvent.Rows = 10;
      var enumerableService = new EnumerableService<Item, ItemView>(EnumerableRepositoryMock.Object, FactoryMock.Object, FilterBuilderMock.Object);

      FilterBuilderMock.Setup(x => x.BuildFilerDefinition(It.IsAny<TableLazyLoadEvent>())).Returns<TableLazyLoadEvent>((tableLazyLoadEvent) => {
        return null;
      });

      EnumerableRepositoryMock.Setup(x => x.GetItems(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<FilterDefinition<Item>>())).Returns<AppUser, string, FilterDefinition<Item>>((appUser, collectionName, filterDefinition) => {
        return new(mockItems, mockTotalCount);
      });

      // Action
      var result = enumerableService.GetItemViews(appUser, "collectionName", tableLazyLoadEvent);

      // Assert
      Assert.NotNull(result);
      Assert.Equal(5, result.TotalCount);
      Assert.Equal(mockTotalCount - tableLazyLoadEvent.First, result.ItemViews.Count());
    }

    [Fact]
    public void GetItemView_ValidInput_ReturnItemView() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var item = new Item() { Id = new ObjectId() };
      var itemView = new ItemView(item);

      var enumerableService = new EnumerableService<Item, ItemView>(EnumerableRepositoryMock.Object, FactoryMock.Object, FilterBuilderMock.Object);

      EnumerableRepositoryMock.Setup(x => x.GetItem(It.IsAny<ObjectId>(), It.IsAny<string>())).Returns<ObjectId, string>((id, collectionName) => {
        return item;
      });

      FactoryMock.Setup(x => x.CreateView(It.IsAny<Item>())).Returns<Item>((item) => {
        return itemView;
      });

      var id = new ObjectId();

      // Action
      var result = enumerableService.GetItemView(id, "collectionName");

      // Assert
      Assert.NotNull(result);
      Assert.Equal(item.Id, result.Id);
    }

    [Fact]
    public void GetItemView_InvalidInput_ReturnNull() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var item = new Item() { Id = new ObjectId() };
      var itemView = new ItemView(item);

      var enumerableService = new EnumerableService<Item, ItemView>(EnumerableRepositoryMock.Object, FactoryMock.Object, FilterBuilderMock.Object);

      EnumerableRepositoryMock.Setup(x => x.GetItem(It.IsAny<ObjectId>(), It.IsAny<string>())).Returns<ObjectId, string>((id, collectionName) => {
        return null;
      });

      FactoryMock.Setup(x => x.CreateView(It.IsAny<Item>())).Returns<Item>((item) => {
        return itemView;
      });

      var id = new ObjectId();

      // Action
      var result = enumerableService.GetItemView(id, "collectionName");

      // Assert
      Assert.Null(result);
    }
  }

  public class ListAsyncCursor<T> : IAsyncCursor<T> {
    private readonly IEnumerator<T> _enumerator;
    private readonly IEnumerable<T> _items;

    public ListAsyncCursor(IEnumerable<T> items) {
      _enumerator = items.GetEnumerator();
      _items = items;
    }

    public IEnumerable<T> Current => _enumerator.Current != null ? new[] { _enumerator.Current } : Enumerable.Empty<T>();

    public bool MoveNext() => _enumerator.MoveNext();

    public void Dispose() => _enumerator.Dispose();

    public bool MoveNextAsync() => _enumerator.MoveNext();

    public IEnumerable<T> ToEnumerable() => _items;

    public async Task<bool> MoveNextAsync(CancellationToken cancellationToken = default) {
      await Task.Yield(); // Simulate an asynchronous operation
      return _enumerator.MoveNext();
    }

    public bool MoveNext(CancellationToken cancellationToken = default) => _enumerator.MoveNext();
  }
}