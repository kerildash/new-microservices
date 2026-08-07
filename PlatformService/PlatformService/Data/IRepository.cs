namespace PlatformService.Data;

public interface IRepository<T>
{
    bool SaveChanges();
    IEnumerable<T> GetAll();
    T? GetById(int id);
    void Add(T item);
}