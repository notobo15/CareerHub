using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Cms;
using RecruitmentApp.Areas.Admin.Companies.Controllers;
using RecruitmentApp.Areas.Admin.Email.ViewModels;
using RecruitmentApp.Data;
using RecruitmentApp.Hubs;
using RecruitmentApp.Models;
using RecruitmentApp.Models.Email;

namespace RecruitmentApp.Areas.Admin.Email.Controllers
{
    [Route("/admin/[controller]/[action]")]
    [Area("Admin/Email")]
    //[Authorize(Roles = RoleName.Administrator)]
    public class EmailController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CompanyController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHubContext<MailHub> _hubContext;
        private readonly IWebHostEnvironment _env;
        private int MAX_ITEMS_PAGE { get; } = 10;


        public EmailController(AppDbContext context, ILogger<CompanyController> logger, UserManager<AppUser> userManager, IHubContext<MailHub> hubContext, IWebHostEnvironment env)
        {
            _context = context;
            _logger = logger;
            _hubContext = hubContext;
            _userManager = userManager;
            _env = env;
        }

        public async Task<IActionResult> Detail(int id)
        {
            var userId = _userManager.GetUserId(User);

            // Lấy mail được click
            var clickedMail = await _context.Mails
                .Include(m => m.Sender)
                .Include(m => m.Recipients).ThenInclude(r => r.User)
                .Include(m => m.ReplyToMail)
                .Include(m => m.Attachments)
                .FirstOrDefaultAsync(m =>
                    m.Id == id &&
                    (m.SenderId == userId || m.Recipients.Any(r => r.UserId == userId)));

            if (clickedMail == null)
            {
                return NotFound();
            }

            // Tìm email gốc trong luồng
            var rootMail = clickedMail;
            while (rootMail.ReplyToMail != null)
            {
                rootMail = rootMail.ReplyToMail;
                rootMail = await _context.Mails
                    .Include(m => m.Sender)
                    .Include(m => m.Recipients).ThenInclude(r => r.User)
                    .Include(m => m.Attachments)
                    .Include(m => m.ReplyToMail)
                    .FirstOrDefaultAsync(m => m.Id == rootMail.Id);
            }

            // Đánh dấu đã đọc nếu người dùng là người nhận
            var recipient = clickedMail.Recipients.FirstOrDefault(r => r.UserId == userId);
            if (recipient != null && !clickedMail.IsRead)
            {
                clickedMail.IsRead = true;
                await _context.SaveChangesAsync();
            }

            // Lấy toàn bộ reply của root mail
            var replies = await _context.Mails
                .Include(m => m.Sender)
                .Include(m => m.Recipients).ThenInclude(r => r.User)
                .Where(m => m.ReplyToMailId == rootMail.Id)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            ViewData["mail"] = rootMail;
            ViewData["replies"] = replies;

            return View(rootMail);
        }

        public async Task<IActionResult> Inbox(string sortBy = "Date", int page = 1, int pageSize = 10, string search = "")
        {
            var userId = _userManager.GetUserId(User);

            var query = _context.MailRecipients
                .Include(r => r.Mail).ThenInclude(m => m.Sender)
                .Include(r => r.Mail).ThenInclude(m => m.Attachments)
                .Where(r => r.UserId == userId && !r.Mail.IsTrash)
                .AsQueryable();

            // ✅ Filter by search term
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r =>
                    r.Mail.Subject.Contains(search) ||
                    r.Mail.Body.Contains(search) ||
                    r.Mail.Sender.Email.Contains(search));
            }

            // ✅ Sorting (giữ nguyên)
            query = sortBy switch
            {
                "From" => query.OrderBy(r => r.Mail.Sender.Email),
                "Subject" => query.OrderBy(r => r.Mail.Subject),
                "Size" => query.OrderByDescending(r => r.Mail.Attachments.Sum(a => a.FileSize)),
                _ => query.OrderByDescending(r => r.Mail.SentAt)
            };

            var totalItems = await query.CountAsync();

            var mails = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => r.Mail)
                .ToListAsync();

            ViewData["CurrentSort"] = sortBy;
            ViewData["CurrentPage"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalItems"] = totalItems;
            ViewData["Search"] = search;

            return View(mails);
        }


        public IActionResult Compose()
        {

            var model = new ComposeMailViewModel
            {
                AllUsers = _context.Users.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.Email
                }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Send(ComposeMailViewModel model)
        {
            if (!ModelState.IsValid) return View("Compose", model);

            var userId = _userManager.GetUserId(User);

            var mail = new Mail
            {
                Subject = model.Subject,
                Body = model.Body,
                SentAt = DateTime.Now,
                SenderId = userId, // bạn cần lấy ID người dùng đăng nhập hiện tại
                Recipients = new List<MailRecipient>(),
                Attachments = new List<Attachment>()
            };

            // Thêm người nhận chính (To:)
            if (model.ToUserIds != null)
            {
                foreach (var recipientId in model.ToUserIds)
                {
                    mail.Recipients.Add(new MailRecipient
                    {
                        UserId = recipientId
                    });
                    await _hubContext.Clients.User(recipientId.ToString())
                        .SendAsync("ReceiveMailNotification");
                }
            }

            // Xử lý file đính kèm
            if (Request.Form.Files.Any())
            {
                var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                foreach (var file in Request.Form.Files)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(uploadsPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    mail.Attachments.Add(new Attachment
                    {
                        FileName = fileName,
                        FilePath = "/uploads/" + fileName,
                        FileSize = file.Length
                    });
                }
            }

            _context.Mails.Add(mail);
            await _context.SaveChangesAsync();

            return RedirectToAction("Inbox");
        }

        public async Task<IActionResult> Sent(string sortBy = "Date", int page = 1, int pageSize = 10, string search = "")
        {
            var userId = _userManager.GetUserId(User);

            var query = _context.Mails
                .Where(m => m.SenderId == userId && !m.IsDraft && !m.IsTrash)
                .Include(m => m.Recipients).ThenInclude(r => r.User)
                .Include(m => m.Attachments)
                .Include(m => m.Sender)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(m =>
                    m.Subject.Contains(search) ||
                    m.Body.Contains(search) ||
                    m.Recipients.Any(r => r.User.Email.Contains(search))
                );
            }

            query = sortBy switch
            {
                "Subject" => query.OrderBy(m => m.Subject),
                "Size" => query.OrderByDescending(m => m.Attachments.Sum(a => a.FileSize)),
                _ => query.OrderByDescending(m => m.SentAt)
            };

            var totalItems = await query.CountAsync();
            var mails = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewData["CurrentSort"] = sortBy;
            ViewData["CurrentPage"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalItems"] = totalItems;
            ViewData["Search"] = search;
            ViewData["Title"] = "Sent Mail";

            return View("Inbox", mails);
        }



        public async Task<IActionResult> Draft(string sortBy = "Date", int page = 1, int pageSize = 10, string search = "")
        {
            var userId = _userManager.GetUserId(User);

            var query = _context.Mails
                .Where(m => m.SenderId == userId && m.IsDraft)
                .Include(m => m.Attachments)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(m => m.Subject.Contains(search) || m.Body.Contains(search));
            }

            query = sortBy switch
            {
                "Subject" => query.OrderBy(m => m.Subject),
                "Size" => query.OrderByDescending(m => m.Attachments.Sum(a => a.FileSize)),
                _ => query.OrderByDescending(m => m.SentAt)
            };

            var totalItems = await query.CountAsync();
            var mails = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewData["CurrentSort"] = sortBy;
            ViewData["CurrentPage"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalItems"] = totalItems;
            ViewData["Search"] = search;
            ViewData["Title"] = "Drafts";

            return View("Inbox", mails);
        }


        public async Task<IActionResult> Trash(string sortBy = "Date", int page = 1, int pageSize = 10, string search = "")
        {
            var userId = _userManager.GetUserId(User);

            var query = _context.Mails
                .Where(m => m.IsTrash &&
                    (m.SenderId == userId || m.Recipients.Any(r => r.UserId == userId)))
                    //(m.SenderId == userId ))
                .Include(m => m.Sender)
                .Include(m => m.Attachments)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(m =>
                    m.Subject.Contains(search) ||
                    m.Body.Contains(search) ||
                    m.Sender.Email.Contains(search)
                );
            }

            query = sortBy switch
            {
                "Subject" => query.OrderBy(m => m.Subject),
                "Size" => query.OrderByDescending(m => m.Attachments.Sum(a => a.FileSize)),
                _ => query.OrderByDescending(m => m.SentAt)
            };

            var totalItems = await query.CountAsync();
            var mails = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewData["CurrentSort"] = sortBy;
            ViewData["CurrentPage"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalItems"] = totalItems;
            ViewData["Search"] = search;
            ViewData["Title"] = "Trash";

            return View("Inbox", mails);
        }



        public async Task<IActionResult> Important(string sortBy = "Date", int page = 1, int pageSize = 10, string search = "")
        {
            var userId = _userManager.GetUserId(User);

            var query = _context.MailRecipients
                .Where(r => r.UserId == userId && r.Mail.IsImportant && !r.Mail.IsTrash)
                .Include(r => r.Mail).ThenInclude(m => m.Sender)
                .Include(r => r.Mail).ThenInclude(m => m.Attachments)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r =>
                    r.Mail.Subject.Contains(search) ||
                    r.Mail.Body.Contains(search) ||
                    r.Mail.Sender.Email.Contains(search));
            }

            query = sortBy switch
            {
                "Subject" => query.OrderBy(r => r.Mail.Subject),
                "Size" => query.OrderByDescending(r => r.Mail.Attachments.Sum(a => a.FileSize)),
                _ => query.OrderByDescending(r => r.Mail.SentAt)
            };

            var totalItems = await query.CountAsync();
            var mails = await query.Skip((page - 1) * pageSize).Take(pageSize).Select(r => r.Mail).ToListAsync();

            ViewData["CurrentSort"] = sortBy;
            ViewData["CurrentPage"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalItems"] = totalItems;
            ViewData["Search"] = search;
            ViewData["Title"] = "Important";

            return View("Inbox", mails);
        }


        public async Task<IActionResult> Starred(string sortBy = "Date", int page = 1, int pageSize = 10, string search = "")
        {
            var userId = _userManager.GetUserId(User);

            var query = _context.MailRecipients
                .Where(r => r.UserId == userId && r.Mail.IsStarred && !r.Mail.IsTrash)
                .Include(r => r.Mail).ThenInclude(m => m.Sender)
                .Include(r => r.Mail).ThenInclude(m => m.Attachments)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r =>
                    r.Mail.Subject.Contains(search) ||
                    r.Mail.Body.Contains(search) ||
                    r.Mail.Sender.Email.Contains(search));
            }

            query = sortBy switch
            {
                "Subject" => query.OrderBy(r => r.Mail.Subject),
                "Size" => query.OrderByDescending(r => r.Mail.Attachments.Sum(a => a.FileSize)),
                _ => query.OrderByDescending(r => r.Mail.SentAt)
            };

            var totalItems = await query.CountAsync();
            var mails = await query.Skip((page - 1) * pageSize).Take(pageSize).Select(r => r.Mail).ToListAsync();

            ViewData["CurrentSort"] = sortBy;
            ViewData["CurrentPage"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalItems"] = totalItems;
            ViewData["Search"] = search;
            ViewData["Title"] = "Starred";

            return View("Inbox", mails);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite(int MailId)
        {
            var mail = await _context.Mails.FindAsync(MailId);
            if (mail == null) return NotFound();

            mail.IsStarred = !mail.IsStarred;
            await _context.SaveChangesAsync();

            return Redirect(Request.Headers["Referer"].ToString());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var mail = await _context.Mails.FindAsync(id);
            if (mail == null)
                return NotFound();

            mail.IsTrash = true; // chuyển trạng thái vào Trash
            await _context.SaveChangesAsync();

            // Quay về trang hiện tại (hoặc RedirectToAction "Inbox")
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDelete(string MailIds)
        {
            if (string.IsNullOrEmpty(MailIds)) return RedirectToAction("Inbox");

            var idList = MailIds.Split(',').Select(id => int.Parse(id)).ToList();

            var mails = await _context.Mails.Where(m => idList.Contains(m.Id)).ToListAsync();
            _context.Mails.RemoveRange(mails);

            await _context.SaveChangesAsync();
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}