using OnlineShoppingApp.Business.DataProtection;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IDataProtection _protector;

        public UserManager(IUnitOfWork unitOfWork, IRepository<UserEntity> userRepository,IDataProtection protector)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _protector = protector;
        }


        public async Task<ServiceMessage> AddUser(AddUserDto user)
        {
            var hasMail = _userRepository.GetAll(x => x.Email.ToLower() == user.Email.ToLower());

            if(hasMail.Any())
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "The email already exists."
                };
            }

            var userEntity = new UserEntity()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Password = _protector.Protect(user.Password),
                PhoneNumber = user.PhoneNumber,
                UserType = UserEntity.Role.User

            };

            _userRepository.Add(userEntity);

            try
            {
                await _unitOfWork.SaveChangesAsync();
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

        public ServiceMessage<UserInfoDto> LoginUser(LoginUserDto user)
        {
            var userEntity = _userRepository.Get(x => x.Email.ToLower() == user.Email.ToLower());

            if(userEntity is  null)
            {
                return new ServiceMessage<UserInfoDto>
                {
                    IsSucceed = false,
                    Message = "Wrong email or password."
                };
            }

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
