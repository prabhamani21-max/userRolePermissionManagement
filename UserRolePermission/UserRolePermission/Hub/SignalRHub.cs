using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace UserRolePermission.HUB
{
    [Authorize] // Only authenticated users can connect to this hub
    public class SignalRHub : Hub
    {
        public async Task JoinChatThread(string threadId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, threadId);
           
        }

        public async Task LeaveChatThread(string threadId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, threadId);
        }
    }
}