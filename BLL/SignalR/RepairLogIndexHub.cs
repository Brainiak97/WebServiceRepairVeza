using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BLL.SignalR
{
    [Authorize]
    public class RepairLogIndexHub : Hub
    {
        public async Task EnterInLogGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "RepairLogIndex");
        }

        public async Task ExitFromLogGroup()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "RepairLogIndex");
        }
    }
}
