using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RecruitmentApp.Areas.Admin.Chats.ViewModels;
using RecruitmentApp.Hubs;
using RecruitmentApp.Models;
using RecruitmentApp.Models.Chat;

namespace RecruitmentApp.Areas.Admin.Chats.Controllers
{
    [Area("Admin/Chats")]
    [Route("/admin/chat/[action]/{id?}")]
    public class ChatController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;
        public ChatController(AppDbContext context, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: Skill
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var rooms = await _context.ChatUserRooms
                .Where(x => x.UserId == userId)
                .Select(x => x.RoomId)
                .ToListAsync();

            // 1. Lấy tin nhắn cuối
            var lastMessages = await _context.ChatMessages
                .Where(m => rooms.Contains(m.RoomId))
                .GroupBy(m => m.RoomId)
                .Select(g => g.OrderByDescending(m => m.SentAt).FirstOrDefault())
                .ToListAsync();

            // 2. Lấy danh sách người còn lại trong các phòng
            var participants = await _context.ChatUserRooms
                .Where(p => rooms.Contains(p.RoomId) && p.UserId != userId)
                .Include(p => p.User) // <- rất quan trọng!
                .ToListAsync();

            var history = lastMessages.Select(msg =>
            {
                var participant = participants.FirstOrDefault(p => p.RoomId == msg.RoomId)?.User;

                return new ChatHistoryViewModel
                {
                    RoomId = msg.RoomId,
                    UserId = participant?.Id,
                    FullName = participant?.UserName ?? "Unknown",
                    Avatar = participant?.Avatar ?? "https://placehold.co/37x37",
                    LastMessage = msg.Message,
                    SentAt = msg.SentAt,
                    UnreadCount = 0
                };
            }).ToList();

            // ✅ Lấy room đầu tiên để load đoạn chat mặc định
            var firstRoom = history.FirstOrDefault();
            List<ChatMessage> messages = new();

            if (firstRoom != null)
            {
                var userTimeZoneId = Request.Cookies["timezone"];
                TimeZoneInfo userTimeZone;

                try
                {
                    userTimeZone = !string.IsNullOrEmpty(userTimeZoneId)
                        ? TimeZoneInfo.FindSystemTimeZoneById(userTimeZoneId)
                        : TimeZoneInfo.Utc;
                }
                catch
                {
                    userTimeZone = TimeZoneInfo.Utc;
                }

                messages = await _context.ChatMessages
                    .Where(m => m.RoomId == firstRoom.RoomId)
                    .OrderBy(m => m.SentAt)
                    .Include(m => m.Sender)
                    .ToListAsync();

                // Chuyển đổi SentAt theo timezone của user
                foreach (var m in messages)
                {
                    m.SentAt = TimeZoneInfo.ConvertTimeFromUtc(m.SentAt, userTimeZone);
                }
            }

            ViewBag.FirstRoomMessages = messages;
            ViewBag.FirstRoomId = firstRoom?.RoomId;
            return View(history);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(int RoomId, string Message)
        {
            if (string.IsNullOrWhiteSpace(Message)) return RedirectToAction("Index");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var newMessage = new ChatMessage
            {
                RoomId = RoomId,
                SenderId = userId,
                Message = Message,
                SentAt = DateTime.UtcNow
            };

            _context.ChatMessages.Add(newMessage);
            await _context.SaveChangesAsync();

            // Gửi signal realtime
            await _hubContext.Clients.Group(RoomId.ToString())
                .SendAsync("ReceiveMessage", RoomId, userId, Message, DateTime.UtcNow);

            return RedirectToAction("Index"); // hoặc RedirectToAction("Detail", new { id = RoomId });
        }


        [HttpGet]
        public async Task<IActionResult> Detail(int id) // id là RoomId
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var messages = await _context.ChatMessages
                .Where(m => m.RoomId == id)
                .OrderBy(m => m.SentAt)
                .Include(m => m.Sender)
                .ToListAsync();

            var roomInfo = await _context.ChatUserRooms
                .Include(p => p.User)
                .Where(p => p.RoomId == id && p.UserId != userId)
                .Select(p => p.User)
                .FirstOrDefaultAsync();

            ViewBag.RoomId = id;
            ViewBag.OtherUser = roomInfo;
            ViewBag.Messages = messages;
            return View("Index");
        }
    }
}
