using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentApp.Models.Chat;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using RecruitmentApp.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace RecruitmentApp.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ChatController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("messages/{roomId}")]
        public async Task<IActionResult> GetMessages(int roomId)
        {
            var messages = await _context.ChatMessages
                .Where(m => m.RoomId == roomId)
                .Include(m => m.Sender)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            return Ok(messages);
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessage model)
        {
            if (!ModelState.IsValid) return BadRequest();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.SenderId = userId;
            model.SentAt = DateTime.UtcNow;

            _context.ChatMessages.Add(model);
            await _context.SaveChangesAsync();

            return Ok(model);
        }

        [HttpPost("create-room")]
        public async Task<IActionResult> CreateRoom([FromBody] string[] userIds)
        {
            var room = new ChatRoom();
            _context.ChatRooms.Add(room);
            await _context.SaveChangesAsync();

            foreach (var userId in userIds)
            {
                _context.ChatUserRooms.Add(new ChatUserRoom
                {
                    RoomId = room.Id,
                    UserId = userId
                });
            }

            await _context.SaveChangesAsync();
            return Ok(room);
        }
    }
}
