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
    public class ScreenService : IScreenService
    {
        private readonly IScreenRepository _repository;
        private readonly IMapper _mapper;

        public ScreenService(IScreenRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<int> CreateScreenAsync(Screen screen)
        {
            return await _repository.CreateScreenAsync(screen);
        }

        public async Task<List<Screen>> GetAllScreensAsync(int? statusId, string? screenName)
        {
            return await _repository.GetAllScreensAsync(statusId, screenName);
        }

        public async Task<Screen> GetScreenByIdAsync(int id)
        {
            return await _repository.GetScreenByIdAsync(id);
        }

        public async Task<Screen> UpdateScreenAsync(Screen screen)
        {
            return await _repository.UpdateScreenAsync(screen);
        }

        public async Task<(bool Success, string Message)> DeleteScreenAsync(int id)
        {
            return await _repository.DeleteScreenAsync(id);
        }
    }
}
