using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserRolePermission.Common.Models;
using UserRolePermission.Common.Pagination;
using UserRolePermission.Repository.Interface;
using UserRolePermission.Service.Interface;

namespace UserRolePermission.Service.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }


        public async Task<long> RegisterUserAsync(User user)
        {
            return await _repository.RegisterUserAsync(user);
        }
        public async Task<Pagination<User>> GetAllUsersAsync( int? statusId, string? name, int pageNumber = 1,
    int pageSize = 10)
        {
            return await _repository.GetAllUsersAsync(statusId, name, pageNumber, pageSize);
        }

        public async Task<User> GetUserByIdAsync(long id)
        {
            return await _repository.GetUserByIdAsync(id);
        }
        public async Task<User> UpdateUserAsync(User user)
        {
            return await _repository.UpdateUserAsync(user);
        }

        public async Task<(bool Success, string Message)> DeleteUserAsync(long id)
        {
            return await _repository.DeleteUserAsync(id);
        }
        public async Task<User> GetUserByEmailAsync(string email)
        {
            var userDb = await _repository.GetUserByEmailAsync(email);
            return _mapper.Map<User>(userDb);
        }
        public async Task<List<int>> GetUserRoleIdsAsync(long userId)
        {
            return await _repository.GetUserRoleIdsAsync(userId);
        }
    }
}
