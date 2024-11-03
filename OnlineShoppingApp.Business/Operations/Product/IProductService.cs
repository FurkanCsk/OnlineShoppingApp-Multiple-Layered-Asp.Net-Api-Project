using OnlineShoppingApp.Business.Operations.Product.Dtos;
using OnlineShoppingApp.Business.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingApp.Business.Operations.Product
{
    public interface IProductService
    {
        Task<ServiceMessage> AddProduct(AddProductDto product);
        Task<ProductDto> GetProduct(int id);
        Task<List<ProductDto>> GetProducts();
        Task<ServiceMessage> AdjustProductPrice(int id, decimal changeTo);
        Task<ServiceMessage> DeleteProduct(int id);
        Task<ServiceMessage> UpdateProduct(UpdateProductDto product);
    }
}
