using Microsoft.EntityFrameworkCore;
using OnlineShoppingApp.Business.Operations.Order.Dtos;
using OnlineShoppingApp.Business.Operations.Product;
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

namespace OnlineShoppingApp.Business.Operations.Order
{
    public class OrderManager : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork; // Unit of Work for managing transactions.
        private readonly IRepository<OrderEntity> _orderRepository; // Repository for OrderEntity.
        private readonly IRepository<OrderProductEntity> _orderProductRepository; // Repository for OrderProductEntity.
        private readonly IProductService _productService; // Service for product operations.

        public OrderManager(IUnitOfWork unitOfWork, IRepository<OrderEntity> repository, IRepository<OrderProductEntity> orderProductRepository, IProductService productService)
        {
            _orderRepository = repository;
            _unitOfWork = unitOfWork;
            _orderProductRepository = orderProductRepository;
            _productService = productService;
        }

        // Adds a new order.
        public async Task<ServiceMessage> AddOrder(AddOrderDto order)
        {
            var hasOrder = await _orderRepository.UserExistAsync(order.UserId); // Check if the user exists.

            if (!hasOrder)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "User not found."
                };
            }

            await _unitOfWork.BeginTransaction(); // Begin transaction.

            var newOrderEntity = new OrderEntity
            {
                UserId = order.UserId,
                OrderDate = DateTime.Now,
                TotalAmount = 0,
                OrderProducts = new List<OrderProductEntity>()
            };

            // Loop through the products in the order.
            foreach (var productDto in order.Products)
            {
                var productPrice = await _productService.GetProduct(productDto.ProductId); // Get the product price.

                if (productPrice is null)
                {
                    await _unitOfWork.RollBackTransaction(); // Roll back transaction if product not found.
                    return new ServiceMessage
                    {
                        IsSucceed = false,
                        Message = "Product not found."
                    };
                }

                var orderProduct = new OrderProductEntity
                {
                    ProductId = productDto.ProductId,
                    Quantity = productDto.Quantity,
                };

                newOrderEntity.OrderProducts.Add(orderProduct); // Add product to the order.
                newOrderEntity.TotalAmount += productPrice.Price * productDto.Quantity; // Update total amount.
            }

            _orderRepository.Add(newOrderEntity); // Add the new order entity.

            try
            {
                await _unitOfWork.SaveChangesAsync(); // Save changes to the database.
                await _unitOfWork.CommitTransaction(); // Commit transaction.
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackTransaction(); // Roll back transaction on error.
                throw new Exception("An error occurred while adding the order.", ex);
            }

            return new ServiceMessage
            {
                IsSucceed = true,
            };
        }

        // Deletes an order by ID.
        public async Task<ServiceMessage> DeleteOrder(int id)
        {
            var order = _orderRepository.GetById(id); // Get the order by ID.

            if (order is null)
            {
                return new ServiceMessage // Return message if order not found.
                {
                    IsSucceed = false,
                    Message = "Order not found."
                };
            }

            _orderRepository.Delete(id); // Delete the order.

            try
            {
                await _unitOfWork.SaveChangesAsync(); // Save changes to the database.
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the order.", ex); // Handle exception.
            }

            return new ServiceMessage
            {
                IsSucceed = true
            };
        }

        // Gets an order by ID.
        public async Task<OrderDto> GetOrder(int id)
        {
            var order = await _orderRepository.GetAll(x => x.Id == id).Select(x => new OrderDto
            {
                Id = x.Id,
                OrderDate = x.OrderDate,
                TotalAmount = x.TotalAmount,
                UserId = x.UserId,
                User = new OrderUserDto
                {
                    Id = x.User.Id,
                    Email = x.User.Email,
                    FirstName = x.User.FirstName,
                    LastName = x.User.LastName,
                    PhoneNumber = x.User.PhoneNumber,
                    UserType = x.User.UserType,
                }
            }).FirstOrDefaultAsync(); // Select and map order to DTO.

            return order; // Return the mapped order.
        }

        // Gets all orders.
        public async Task<List<OrderDto>> GetOrders()
        {
            var order = await _orderRepository.GetAll().Select(x => new OrderDto
            {
                Id = x.Id,
                OrderDate = x.OrderDate,
                TotalAmount = x.TotalAmount,
                UserId = x.UserId,
                User = new OrderUserDto
                {
                    Id = x.User.Id,
                    Email = x.User.Email,
                    FirstName = x.User.FirstName,
                    LastName = x.User.LastName,
                    PhoneNumber = x.User.PhoneNumber,
                    UserType = x.User.UserType,
                }
            }).ToListAsync(); // Select and map all orders to DTOs.

            return order; // Return the list of mapped orders.
        }

        // Updates an existing order.
        public async Task<ServiceMessage> UpdateOrder(UpdateOrderDto order)
        {
            var orderEntity = _orderRepository.GetById(order.Id); // Get the existing order.

            if (orderEntity is null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Order not found."
                };
            }

            await _unitOfWork.BeginTransaction(); // Begin transaction.

            orderEntity.UserId = order.UserId; // Update user ID.
            // orderEntity.OrderDate = order.OrderDate; // Optional: Update order date (commented out).

            _orderRepository.Update(orderEntity); // Update the order entity.

            try
            {
                await _unitOfWork.SaveChangesAsync(); // Save changes to the database.
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackTransaction(); // Roll back transaction on error.
                throw new Exception("An error occurred while updating the order.", ex);
            }

            var orderProducts = _orderProductRepository.GetAll(x => x.OrderId == orderEntity.Id).ToList(); // Get all products for the order.

            // Delete existing order products.
            foreach (var orderProduct in orderProducts)
            {
                _orderProductRepository.Delete(orderProduct, false);
            }

            // Add new order products.
            foreach (var productId in order.Products)
            {
                var orderProduct = new OrderProductEntity
                {
                    ProductId = productId.ProductId,
                    Quantity = productId.Quantity,
                    OrderId = orderEntity.Id
                };

                _orderProductRepository.Add(orderProduct); // Add new product to the order.
            }

            try
            {
                await _unitOfWork.SaveChangesAsync(); // Save changes to the database.
                await _unitOfWork.CommitTransaction(); // Commit transaction.
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackTransaction(); // Roll back transaction on error.
                throw new Exception("An error occurred while updating the order products.", ex);
            }

            return new ServiceMessage
            {
                IsSucceed = true,
            };
        }
    }
}
