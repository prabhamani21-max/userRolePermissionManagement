using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserRolePermission.Common.Models;
using UserRolePermission.Repository.Interface;
using UserRolePermission.Service.Interface;

namespace UserRolePermission.Service.Implementation
{
    public class StatusService : IStatusService
    {
        private readonly IStatusRepository _repository;
        private readonly IMapper _mapper;

        public StatusService(IStatusRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<int> CreateStatusAsync(Status status)
        {
            return await _repository.CreateStatusAsync(status);
        }
        public async Task<List<Status>> GetAllStatusesAsync()
        {
            return await _repository.GetAllStatusesAsync();
        }

        public async Task<Status> GetStatusByIdAsync(int id)
        {
            return await _repository.GetStatusByIdAsync(id);
        }
        public async Task<Status> UpdateStatusAsync(Status status)
        {
            return await _repository.UpdateStatusAsync(status);
        }

        public async Task<(bool Success, string Message)> DeleteStatusAsync(int id)
        {
            return await _repository.DeleteStatusAsync(id);
        }
    }
}
