using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BLL.SignalR
{
    [Authorize]
    public class RepairLogDetailsHub : Hub
    {
        public async Task EnterInLogGroup(int logId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"LogGroupDetails#{logId}");
        }

        public async Task ExitFromLogGroup(int logId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"LogGroupDetails#{logId}");
        }
    }
}
