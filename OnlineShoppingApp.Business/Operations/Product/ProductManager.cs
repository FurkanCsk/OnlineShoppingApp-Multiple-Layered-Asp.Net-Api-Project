using Microsoft.EntityFrameworkCore;
using OnlineShoppingApp.Business.Operations.Product.Dtos;
using OnlineShoppingApp.Business.Types;
using OnlineShoppingApp.Data.Entities;
using OnlineShoppingApp.Data.Repositories;
using OnlineShoppingApp.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingApp.Business.Operations.Product
{
    public class ProductManager : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ProductEntity> _productRepository;

        public ProductManager(IUnitOfWork unitOfWork, IRepository<ProductEntity> repository)
        {
            _productRepository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceMessage> AddProduct(AddProductDto product)
        {
            var hasProduct = _productRepository.GetAll(x => x.ProductName.ToLower() == product.ProductName.ToLower()).Any();

            if (hasProduct)
            {
                new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "The product already exists."
                };
            }

            var productEntity = new ProductEntity
            {
                ProductName = product.ProductName,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
            };

            _productRepository.Add(productEntity);

            try
            {
                await _unitOfWork.SaveChangesAsync();

            }
            catch (Exception)
            {

                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "An error occurred while adding the product."
                };
            }

            return new ServiceMessage
            {
                IsSucceed = true
            };
        }

        public async Task<ServiceMessage> AdjustProductPrice(int id, decimal changeTo)
        {
            var product = _productRepository.GetById(id);

            if (product is null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "This product matching the ID was not found."
                };
            }

            product.Price = changeTo;

            _productRepository.Update(product);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {

                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "An error occurred while changing the product price."
                };
            }

            return new ServiceMessage
            {
                IsSucceed = true
            };

        }

        public async Task<ServiceMessage> DeleteProduct(int id)
        {
            var product = _productRepository.GetById(id);

            if (product is null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "The product to be deleted was not found."
                };
            }

            _productRepository.Delete(id);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {

                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "An error occurred during the deletion process."
                };
            }

            return new ServiceMessage
            {
                IsSucceed = true
            };
        }

        public async Task<ProductDto> GetProduct(int id)
        {
            var product = await _productRepository.GetAll(x => x.Id == id).Select(x => new ProductDto
            {
                ProductName = x.ProductName,
                Price = x.Price,
                StockQuantity = x.StockQuantity,
            }).FirstOrDefaultAsync();

            return product;
        }

        public async Task<List<ProductDto>> GetProducts()
        {
            var products = await _productRepository.GetAll().Select(x => new ProductDto
            {
                ProductName = x.ProductName,
                Price = x.Price,
                StockQuantity = x.StockQuantity,
            }).ToListAsync();

            return products;
        }

        public async Task<ServiceMessage> UpdateProduct(UpdateProductDto product)
        {
            var productEntity = _productRepository.GetById(product.Id);

            if (productEntity is null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Product not found."
                };
            }

            productEntity.ProductName = product.ProductName;
            productEntity.Price = product.Price;
            productEntity.StockQuantity = product.StockQuantity;

            _productRepository.Update(productEntity);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {

                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "An error occurred while updating product information."
                };
            }

            return new ServiceMessage
            {
                IsSucceed = true,
            };
          
        }
    }
}
