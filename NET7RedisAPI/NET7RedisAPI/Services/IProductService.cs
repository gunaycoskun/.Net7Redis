using NET7RedisAPI.Model;

namespace NET7RedisAPI.Services
{
    public interface IProductService
    {

        Task<List<Product>> GetAllAsync();

        Task<Product> GetByIdAsync(int id);

        Task<Product> CreateAsync(Product product);
    }
}
