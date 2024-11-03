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

        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<OrderEntity> _orderRepository;
        private readonly IRepository<OrderProductEntity> _orderProductRepository;
        private readonly IProductService _productService;

        public OrderManager(IUnitOfWork unitOfWork, IRepository<OrderEntity> repository, IRepository<OrderProductEntity> orderProductRepository, IProductService productService)
        {
            _orderRepository = repository;
            _unitOfWork = unitOfWork;
            _orderProductRepository = orderProductRepository;
            _productService = productService;
        }

        public async Task<ServiceMessage> AddOrder(AddOrderDto order)
        {
            var hasOrder = await _orderRepository.UserExistAsync(order.UserId);

            if (!hasOrder)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "User not found."
                };
            }
            await _unitOfWork.BeginTransaction();

            var newOrderEntity = new OrderEntity
            {
                UserId = order.UserId,
                OrderDate = DateTime.Now,
                TotalAmount = 0,
                OrderProducts = new List<OrderProductEntity>()

            };

            foreach (var productDto in order.Products)
            {
                var productPrice = await _productService.GetProduct(productDto.ProductId);

                if (productPrice is null)
                {
                    await _unitOfWork.RollBackTransaction();
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

                newOrderEntity.OrderProducts.Add(orderProduct);
                newOrderEntity.TotalAmount += productPrice.Price * productDto.Quantity;
            }


            _orderRepository.Add(newOrderEntity);

            try
            {
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackTransaction();
                throw new Exception("An error occurred while adding the order.", ex);
            }

            return new ServiceMessage
            {
                IsSucceed = true,
            };

        }

        public async Task<ServiceMessage> DeleteOrder(int id)
        {
            var order = _orderRepository.GetById(id);

            if (order is null)
            {
                new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Order not found."
                };
            }

            _orderRepository.Delete(id);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw new Exception("An error occurred while deleting the order.", ex);
            }

            return new ServiceMessage
            {
                IsSucceed = true
            };
        }

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

            }).FirstOrDefaultAsync();

            return order;
        }

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

            }).ToListAsync();

            return order;
        }

        public async Task<ServiceMessage> UpdateOrder(UpdateOrderDto order)
        {
            var orderEntity = _orderRepository.GetById(order.Id);

            if (orderEntity is null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Order not found."
                };
            }

            await _unitOfWork.BeginTransaction();

            orderEntity.UserId = order.UserId;
            //  orderEntity.OrderDate = order.OrderDate;

            _orderRepository.Update(orderEntity);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackTransaction();
                throw new Exception("An error occurred while updating the order.", ex);
            }

            var orderProducts = _orderProductRepository.GetAll(x => x.OrderId == x.Order.Id).ToList();

            foreach (var orderProduct in orderProducts)
            {
                _orderProductRepository.Delete(orderProduct, false);
            }

            foreach (var productId in order.Products)
            {
                var orderProduct = new OrderProductEntity
                {
                    ProductId = productId.ProductId,
                    Quantity= productId.Quantity,
                    OrderId = orderEntity.Id
                };

                _orderProductRepository.Add(orderProduct);
            }

            try
            {
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackTransaction();
                throw new Exception("An error occurred while updating the order products.", ex);
            }

            return new ServiceMessage
            {
                IsSucceed = true,
            };
        }
    }
}
