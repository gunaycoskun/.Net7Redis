using NET7Redis.Cache;
using NET7RedisAPI.Model;
using StackExchange.Redis;
using System.Text.Json;

namespace NET7RedisAPI.Repository
{
    public class ProductRepositoryWithCache : IProductRepository
    {
        private const string productKey = "productsCache";
        private readonly IProductRepository _productRepository;
        private readonly IDatabase _cacheRepository;
        private readonly RedisService _redisService;
        public ProductRepositoryWithCache(IProductRepository repository, RedisService redisService,int dbIndex)
        {
            _productRepository = repository;          
            _redisService = redisService;
            _cacheRepository = _redisService.GetDatabase(dbIndex);
        }
        public async Task<Product> CreateAsync(Product product)
        {
            var newProduct = await _productRepository.CreateAsync(product);
            if(await _cacheRepository.KeyExistsAsync(productKey))
            {
                await _cacheRepository.HashSetAsync(productKey, product.Id, JsonSerializer.Serialize(product));
            }          
            return newProduct;
        }
        public async Task<List<Product>> GetAllAsync()
        {
            if (!await _cacheRepository.KeyExistsAsync(productKey))
            {
                return await LoadCacheFromDb();
            }
            else
            {
                var products=new List<Product>();
                var cacheProductList = await _cacheRepository.HashGetAllAsync(productKey);
                foreach (var item in cacheProductList.ToList())
                {
                    products.Add(JsonSerializer.Deserialize<Product>(item.Value));
                }
                return products;
            }
        }
        public async Task<Product> GetByIdAsync(int id)
        {
            if (await _cacheRepository.KeyExistsAsync(productKey))
            {
                var product= await _cacheRepository.HashGetAsync(productKey,id);
                return product.HasValue? JsonSerializer.Deserialize<Product>(product):null;
            }
            var products=await LoadCacheFromDb();
            return products.FirstOrDefault(x => x.Id == id);
        }
        private async Task<List<Product>> LoadCacheFromDb()
        {
            var products = await _productRepository.GetAllAsync();
            products.ForEach(p =>
            {
                _cacheRepository.HashSetAsync(productKey, p.Id, JsonSerializer.Serialize(p));
            });
            return products;
        }
    }
}
