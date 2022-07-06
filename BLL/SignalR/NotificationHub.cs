using BLL.Models;
using BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BLL.SignalR
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly UserService _userService;
        private readonly RepairGroupService _repairGroupService;
        private readonly NotificationService _notificationService;

        public NotificationHub(UserService userService, RepairGroupService repairGroupService, NotificationService notificationService)
        {
            _userService = userService;
            _repairGroupService = repairGroupService;
            _notificationService = notificationService;
        }

        public async Task EnterInRepairGroup()
        {
            var groups = await _repairGroupService.GetUserGroups(Convert.ToInt32(Context.UserIdentifier));
            if (groups != null)
                foreach (var gr in groups)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, gr.Name);
                }
        }

        public async Task ExitFromRepairGroup()
        {
            var groups = await _repairGroupService.GetUserGroups(Convert.ToInt32(Context.UserIdentifier));
            if (groups != null)
                foreach (var gr in groups)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, gr.Name);
                }
        }

        public async Task NotifCountUpdate()
        {
            var user = await _userService.GetUserWithRepairGroups(Convert.ToInt32(Context.UserIdentifier));
            if (user != null)
            {
                var notifs = await _notificationService.GetUserNotifications(user.Id);

                await Clients.User(user.Id.ToString()).SendAsync("NotifCount", notifs.Count());

                if (notifs != null)
                    foreach (var comment in notifs.Select(not => not.Comment))
                    {
                        await Clients.User(user.Id.ToString()).SendAsync("NewMessage",
                            $"{comment.Commentator.SurName} {comment.Commentator.Name[0]}.{comment.Commentator.MiddleName[0]}.",
                            comment.Text,
                            comment.Date.ToString("f"),
                            comment.Id,
                            comment.RepairLogId);
                    }
            }
        }

        public override async Task OnConnectedAsync()
        {
            await EnterInRepairGroup();
            await NotifCountUpdate();

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await ExitFromRepairGroup();

            await base.OnDisconnectedAsync(exception);
        }
    }
}
