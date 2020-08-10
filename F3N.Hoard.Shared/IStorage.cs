using System.Collections.Generic;
using System.Threading.Tasks;

namespace F3N.Hoard.Shared
{
    public interface IStorage
    {
        Task<T> Get<T>(string id);
        Task<T> GetByKey<T>(string key);
        Task<List<T>> GetAll<T>();

        Task Save<T>(string id, T data);
        Task SaveByKey<T>(string key, T data);
        Task SaveAllByKey<T>(Dictionary<string, T> data);

        Task Delete<T>(string id);
        Task DeleteByKey<T>(string key);
        Task DeleteAll<T>();

        Task Clear();

        Task Flush();
    }
}
