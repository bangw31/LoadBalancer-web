//using ECommerceSystem.Api.Data;
//using ECommerceSystem.Shared.DTOs;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace ECommerceSystem.Api.Services
//{
//    public class NotificationService
//    {
//        private readonly WebDBContext _dbContext;

//        public NotificationService(WebDBContext dbContext)
//        {
//            _dbContext = dbContext;
//        }

//        public async Task<List<NotificationDTO>> GetNotifications(string username)
//        {
//            var notifications = await _dbContext.Notifications
//                .Where(n => n.UserName == username)
//                .Select(n => new NotificationDTO { Message = n.Message, Time = n.CreatedAt })
//                .ToListAsync();
//            return notifications ?? new List<NotificationDTO>();
//        }
//    }
//}