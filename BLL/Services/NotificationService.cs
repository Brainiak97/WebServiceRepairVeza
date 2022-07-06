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
    public class NotificationService : GenericService<Notification, NotificationDto>
    {
        private IHubContext<NotificationHub> _hubContext;

        public NotificationService(IMapper mapper, IRepository<Notification> repository,
                           IValidator<Notification> validator, IHubContext<NotificationHub> hubContext)
           : base(mapper, repository, validator)
        {
            _hubContext = hubContext;
        }

        public async Task<IEnumerable<NotificationDto>> GetUserNotifications(int userId)
        {
            return _mapper.Map<IEnumerable<NotificationDto>>(await _repository
                .GetQuery()
                .Where(u => u.RecipientId.Equals(userId) && !u.IsRead)
                                            .Include(n => n.Recipient)
                                            .Include(n => n.Comment)
                                            .Include(n => n.Comment.Commentator)
                                            .ToListAsync());
        }

        public async Task ReadNotification(int commentId, int userId)
        {
            var notification = await _repository
                .GetQuery()
                .FirstOrDefaultAsync(n => n.RecipientId.Equals(userId) && n.CommentId == commentId);

            if (notification != null)
            {
                notification.IsRead = true;

                await _repository.Update(notification);
            }
        }

        public async Task ReadAllNotifications(int userId)
        {
            var notifications = await _repository
                .GetQuery()
                .Where(n => n.RecipientId.Equals(userId))
                .ToListAsync();

            if (notifications != null)
            {
                foreach (var notification in notifications)
                {
                    notification.IsRead = true;

                    await _repository.Update(notification);
                }
            }

            await _hubContext.Clients.User(userId.ToString()).SendAsync("ClearList");
        }
    }
}
