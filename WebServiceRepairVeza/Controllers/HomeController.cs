using BLL.Services;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace WebService.Controllers
{
    public class HomeController : Controller
    {
        private readonly NotificationService _notificationService;

        public HomeController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(ErrorViewModel errorViewModel)
        {
            return View(errorViewModel);
        }

        [HttpPost]
        public async Task ReadAllNotifications()
        {
            await _notificationService.ReadAllNotifications(Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
        }

        [HttpPost]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var notifications = await _notificationService.GetUserNotifications(userId);
            return Ok(new { Notification = notifications, Count = notifications.Count() });
        }

        [HttpPost]
        public async Task ReadNotification(int commentId)
        {
            await _notificationService.ReadNotification(commentId, Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
        }
    }
}
