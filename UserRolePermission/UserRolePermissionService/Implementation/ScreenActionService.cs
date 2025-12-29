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
    public class ScreenActionService : IScreenActionService
    {
        private readonly IScreenActionRepository _repository;
        private readonly IMapper _mapper;

        public ScreenActionService(IScreenActionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<int> CreateScreenActionAsync(ScreenAction screenAction)
        {
            return await _repository.CreateScreenActionAsync(screenAction);
        }
        public async Task<List<ScreenAction>> GetAllScreenActionsAsync(int? statusId, int? screenId, string? actionName)
        {
            return await _repository.GetAllScreenActionsAsync(statusId, screenId, actionName);
        }

        public async Task<ScreenAction> GetScreenActionByIdAsync(int id)
        {
            return await _repository.GetScreenActionByIdAsync(id);
        }
        public async Task<ScreenAction> UpdateScreenActionAsync(ScreenAction screenAction)
        {
            return await _repository.UpdateScreenActionAsync(screenAction);
        }

        public async Task<(bool Success, string Message)> DeleteScreenActionAsync(int id)
        {
            return await _repository.DeleteScreenActionAsync(id);
        }
    }
}