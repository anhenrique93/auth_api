namespace TaskCircle.AuthentcationApi.Infrastructure.Repositories.Iterfaces;

public interface IRepository<T>
{
    Task<T> Register(T Entity);
    Task<T> Update(T Entity);
    Task<T> Delete(int id);
    Task<T> GetUserById(int id);
}
