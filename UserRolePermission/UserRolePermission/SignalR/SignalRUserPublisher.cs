using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using UserRolePermission.Common.Models;
using UserRolePermission.HUB;
using UserRolePermission.Service.Interface;

namespace UserRolePermission.SignalR
{
    public class SignalRUserPublisher : IUserPublisherService
    {
        private readonly IHubContext<SignalRHub> _hubContext;
        private readonly ILogger<SignalRUserPublisher> _logger;
        private readonly IMapper _mapper;

        public SignalRUserPublisher(IHubContext<SignalRHub> hubContext, ILogger<SignalRUserPublisher> logger, IMapper mapper)
        {
            _hubContext = hubContext;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task PublishUserAdded(User user)
        {
           
            await _hubContext.Clients.All.SendAsync("UserAdded", user);
            _logger.LogInformation("User added event published: {UserId}", user.Id);
        }

        public async Task PublishUserUpdated(User user)
        {
            await _hubContext.Clients.All.SendAsync("UserUpdated", user);
            _logger.LogInformation("User updated event published: {UserId}", user.Id);
        }

        public async Task PublishUserDeleted(long userId)
        {
            await _hubContext.Clients.All.SendAsync("UserDeleted", userId);
            _logger.LogInformation("User deleted event published: {UserId}", userId);
        }
    }
}
