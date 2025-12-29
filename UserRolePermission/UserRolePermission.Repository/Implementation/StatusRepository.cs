using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UserRolePermission.Common.Models;
using UserRolePermission.Repository.Data;
using UserRolePermission.Repository.Interface;
using UserRolePermission.Repository.Models;

namespace UserRolePermission.Repository.Implementation
{
    public class StatusRepository : IStatusRepository
    {
        private readonly AppDBContext _context;
        private readonly IMapper _mapper;
        public StatusRepository(AppDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<int> CreateStatusAsync(Status status)
        {
            var entity = _mapper.Map<StatusDb>(status);
            await _context.UserStatus.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }
        public async Task<List<Status>> GetAllStatusesAsync()
        {
            var statuses = await _context.UserStatus
                .AsNoTracking()
                .OrderByDescending(s => s.Id)
                .ToListAsync();

            return _mapper.Map<List<Status>>(statuses);
        }
        public async Task<Status> GetStatusByIdAsync(int id)
        {
            var status = await _context.UserStatus.FindAsync(id);
            return _mapper.Map<Status>(status);
        }
        public async Task<Status> UpdateStatusAsync(Status status)
        {
            var existingStatus = await _context.UserStatus.FindAsync(status.Id);
            if (existingStatus == null) return null;
            existingStatus.Name = status.Name;
            existingStatus.UpdatedDate = DateTime.UtcNow;
            existingStatus.UpdatedBy = status.UpdatedBy;

            _context.UserStatus.Update(existingStatus);
            await _context.SaveChangesAsync();
            return new Status
            {
                Id = status.Id,
                Name = status.Name,
                UpdatedDate = status.UpdatedDate,
                UpdatedBy = status.UpdatedBy,
            };
        }
        public async Task<(bool Success, string Message)> DeleteStatusAsync(int id)
        {
            var status = await _context.UserStatus.FindAsync(id);
            if (status == null) return (false, "Status not found");

            _context.UserStatus.Remove(status);
            await _context.SaveChangesAsync();
            return (true, "Status deleted successfully");
        }
    }
}
