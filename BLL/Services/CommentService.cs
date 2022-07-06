using AutoMapper;
using BLL.Models;
using BLL.Services.Generic;
using BLL.SignalR;
using Core.Models;
using DAL.Repository;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class CommentService : GenericService<Comment, CommentDto>
    {
        private readonly NotificationService _notificationService;
        private readonly IHubContext<NotificationHub> _notifHubContext;
        private readonly IHubContext<CommentHub> _commentHubContext;

        public CommentService(IMapper mapper, IRepository<Comment> repository,
                           IValidator<Comment> validator, IHubContext<NotificationHub> hubContext, IHubContext<CommentHub> commentHubContext, NotificationService notificationService)
           : base(mapper, repository, validator)
        {
            _notificationService = notificationService;
            _commentHubContext = commentHubContext;
            _notifHubContext = hubContext;
        }

        public async Task AddNotificationsToComment_ByRepairGroups(int userId, int commId)
        {
            var comment = await _repository
                .GetQuery()
                .Include(log => log.Notifications)
                .FirstOrDefaultAsync(comment => comment.Id == commId);

            if (comment != null)
            {
                var notification = new Notification()
                {
                    IsRead = false,
                    CommentId = commId,
                    RecipientId = userId,
                    Text = comment.Text
                };
                comment.Notifications.Add(notification);

                await _repository.Update(comment);

                await _notifHubContext.Clients.User(userId.ToString()).SendAsync("NewMessage",
                    $"{comment.Commentator.SurName} {comment.Commentator.Name[0]}.{comment.Commentator.MiddleName[0]}.",
                    $"{comment.Text}",
                    $"{comment.Date:f}",
                    $"{comment.Id}",
                    $"{comment.RepairLogId}");

                var nots = await _notificationService.GetUserNotifications(userId);
                await _notifHubContext.Clients.User(userId.ToString()).SendAsync("NotifCount", nots.Count());
            }
        }

        public async Task AddNotificationsToComment_AtExecutorUser(int userId, int commId)
        {
            var comment = await _repository
                .GetQuery()
                .Include(comment => comment.Notifications)
                .Include(comment => comment.RepairLog)
                .FirstOrDefaultAsync(comment => comment.Id == commId);

            if (comment != null)
            {
                var notification = new Notification()
                {
                    IsRead = false,
                    CommentId = commId,
                    RecipientId = userId,
                    Text = $"Вас назначили исполнителем к заявке #{comment.RepairLog.Id}"
                };
                comment.Notifications.Add(notification);

                await _repository.Update(comment);

                await _notifHubContext.Clients.User(userId.ToString()).SendAsync("NewMessage",
                    $"{comment.Commentator.SurName} {comment.Commentator.Name[0]}.{comment.Commentator.MiddleName[0]}.",
                    notification.Text,
                    $"{comment.Date:f}",
                    $"{comment.Id}",
                    $"{comment.RepairLogId}");

                var nots = await _notificationService.GetUserNotifications(userId);
                await _notifHubContext.Clients.User(userId.ToString()).SendAsync("NotifCount", nots.Count());
            }
        }

        public async Task SendNewCommentToActiveUsers(int commId)
        {
            var comment = await _repository
                .GetQuery()
                .Include(log => log.Notifications)
                .Include(log => log.Commentator)
                .FirstOrDefaultAsync(comment => comment.Id == commId);

            if (comment != null)
            {
                await _commentHubContext.Clients.Group($"LogGroupComment#{comment.RepairLogId}").SendAsync("NewComment",
                    $"{comment.Commentator.SurName} {comment.Commentator.Name[0]}.{comment.Commentator.MiddleName[0]}.",
                    $"{comment.Text}",
                    $"{comment.Date:f}",
                    $"{comment.Id}");
            }
        }

        public async Task DisplayAllLogComment(int id, string userId)
        {
            var comments = await _repository
                            .GetQuery()
                            .Where(c => c.RepairLogId == id)
                            .Include(c => c.Commentator)
                            .ToListAsync();

            if (comments != null)
            {
                foreach (var comment in comments.OrderBy(comm => comm.Date))
                {
                    await _commentHubContext.Clients.User(userId).SendAsync("NewComment",
                        $"{comment.Commentator.SurName} {comment.Commentator.Name[0]}.{comment.Commentator.MiddleName[0]}.",
                        $"{comment.Text}",
                        $"{comment.Date:f}",
                        $"{comment.Id}");
                }
            }
        }
    }
}
