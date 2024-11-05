using MongoDB.Bson;

namespace budget.core.Factories.Interfaces 
{
    public interface ITransformable<T>
      where T : class
    {
        public T Transform();
        public ObjectId Id { get; set; }
  }
}