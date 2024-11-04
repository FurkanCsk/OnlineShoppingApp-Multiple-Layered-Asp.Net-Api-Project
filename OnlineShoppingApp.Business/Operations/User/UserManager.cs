using Microsoft.EntityFrameworkCore;
using OnlineShoppingApp.Business.DataProtection;
using OnlineShoppingApp.Business.Operations.Product.Dtos;
using OnlineShoppingApp.Business.Operations.User.Dtos;
using OnlineShoppingApp.Business.Types;
using OnlineShoppingApp.Data.Entities;
using OnlineShoppingApp.Data.Repositories;
using OnlineShoppingApp.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingApp.Business.Operations.User
{
    public class UserManager : IUserService
    {
        private readonly IUnitOfWork _unitOfWork; // Manages database transactions
        private readonly IRepository<UserEntity> _userRepository; // Repository for user data
        private readonly IDataProtection _protector; // Handles data protection

        public UserManager(IUnitOfWork unitOfWork, IRepository<UserEntity> userRepository,IDataProtection protector)
        {
            _unitOfWork = unitOfWork; // Initialize unit of work
            _userRepository = userRepository; // Initialize user repository
            _protector = protector; // Initialize data protector
        }


        public async Task<ServiceMessage> AddUser(AddUserDto user)
        {
            // Check if the email already exists in the database
            var hasMail = _userRepository.GetAll(x => x.Email.ToLower() == user.Email.ToLower());

            if(hasMail.Any())
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "The email already exists."
                };
            }
            // Create a new user entity with protected password
            var userEntity = new UserEntity()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Password = _protector.Protect(user.Password), // Protect the password
                PhoneNumber = user.PhoneNumber,
                UserType = UserEntity.Role.User

            };

            _userRepository.Add(userEntity); // Add the user entity to the repository

            try
            {
                await _unitOfWork.SaveChangesAsync(); // Save changes to the database
            }
            catch (Exception)
            {

                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "An error occurred during user registration."
                };
            }

            return new ServiceMessage
            {
                IsSucceed = true
            };
        }

        public async Task<List<UserInfoDto>> GetUsers()
        {
            var users = await _userRepository.GetAll().Select(x => new UserInfoDto
            {
                Id = x.Id,
                Email =  x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                UserType = x.UserType
            }).ToListAsync();

            return users;
        }

        public ServiceMessage<UserInfoDto> LoginUser(LoginUserDto user)
        {
            // Retrieve user entity based on email
            var userEntity = _userRepository.Get(x => x.Email.ToLower() == user.Email.ToLower());

            if(userEntity is  null)
            {
                return new ServiceMessage<UserInfoDto>
                {
                    IsSucceed = false,
                    Message = "Wrong email or password."
                };
            }
            // Unprotect the password for comparison
            var unprotectedPassword = _protector.UnProtect(userEntity.Password);

            if(unprotectedPassword == user.Password)
            {
                return new ServiceMessage<UserInfoDto>
                {
                    IsSucceed = true,
                    Data = new UserInfoDto
                    {
                        Email = userEntity.Email,
                        FirstName = userEntity.FirstName,
                        LastName = userEntity.LastName,
                        UserType = userEntity.UserType

                    }
                };
            }
            else
            {
                return new ServiceMessage<UserInfoDto>
                {
                    IsSucceed = false,
                    Message = "Wrong email or password."
                };
            }
        }
    }
}
