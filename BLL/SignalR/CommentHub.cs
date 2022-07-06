using BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BLL.SignalR
{
    [Authorize]
    public class CommentHub : Hub
    {
        private readonly CommentService _commentService;

        public CommentHub(CommentService commentService)
        {
            _commentService = commentService;
        }

        public async Task EnterInLogGroup(int logId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"LogGroupComment#{logId}");

            await _commentService.DisplayAllLogComment(logId, Context.UserIdentifier);
        }

        public async Task ExitFromLogGroup(int logId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"LogGroupComment#{logId}");
        }
    }
}
