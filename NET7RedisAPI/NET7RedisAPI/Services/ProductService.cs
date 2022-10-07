using NET7RedisAPI.Model;
using NET7RedisAPI.Repository;

namespace NET7RedisAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            return await _productRepository.CreateAsync(product);
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            // mapper DTO

            return product;
        }
    }
}
