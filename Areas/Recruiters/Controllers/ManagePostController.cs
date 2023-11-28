using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecruitmentApp.Models;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using RecruitmentApp.Data;

namespace RecruitmentApp.Areas.Recruiters.Controllers
{
    [Area("Recruiters")]

    [Authorize(Roles = RoleName.Recuiter)]
    [Route("/nha-tuyen-dung/quan-ly-bai-dang/[action]/{id?}")]
    public class ManagePostController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly string userId;

        public ManagePostController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            
        }

        // GET: Post
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var posts =  _context.Posts
                .Include(p => p.Company)
                .Where(p => p.Company.RecruiterId == userId)
                .AsSplitQuery()
                .ToList();
            return View(posts);
        }

        // GET: Post/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Post/Create
        public IActionResult Create()
        {

            var listSkill = _context.Skills.ToList();
            ViewData["list"] = new MultiSelectList(listSkill, "SkillId", "Name");
            return View();
        }

        // POST: Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,Title,IsHot,Salary,Expired,WorkSpace,Description,JobRequirement,Benifit,SkillIds, LevelIds")] Post post)
        {
            if (ModelState.IsValid)
            {
                _context.Add(post);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Post/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.PostSkills)
                .Include(p => p.PostLevels)
                .ThenInclude(p => p.Level)
                .FirstOrDefaultAsync(p => p.PostId == id);

            post.SkillIds = post.PostSkills.Select(p => p.SkillID).ToArray();
            post.LevelIds = post.PostLevels.Select(p => p.LevelID).ToArray();
            if (post == null)
            {
                return NotFound();
            }
            var listSkill = _context.Skills.ToList();
            ViewData["list"] = new MultiSelectList(listSkill, "SkillId", "Name");
            ViewData["listLevel"] = new MultiSelectList(_context.Levels.ToList(), "LevelId", "Name");
            return View(post);
        }

        // POST: Post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,IsHot,MinSalary,MaxSalary,WorkSpace,Description,JobRequirement,Benifit, SkillIds, LevelIds")] Post newPost)
        {
            if (id != newPost.PostId)
            {
                return NotFound();
            }

            var post = await _context.Posts
            .Include(p => p.PostLevels)
            .Include(p => p.PostSkills)
            .FirstOrDefaultAsync(p => p.PostId == id);

            if (post == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    post.CreatedAt = DateTime.Now;
                    post.Title = newPost.Title;
                    post.IsHot = newPost.IsHot;
                    post.MaxSalary = newPost.MaxSalary;
                    post.MinSalary = newPost.MinSalary;
                    post.WorkSpace = newPost.WorkSpace;
                    post.Description = newPost.Description;
                    post.JobRequirement = newPost.JobRequirement;
                    post.Benifit = newPost.Benifit;

                    if (newPost.SkillIds == null) newPost.SkillIds = new int[] { };
                    if (newPost.LevelIds == null) newPost.LevelIds = new int[] { };

                    var oldSkills = post.PostSkills.Select(e => e.SkillID).ToArray();
                    var newSkills = newPost.SkillIds;

                    var removeSkills = from c in post.PostSkills
                                       where (!newSkills.Contains(c.SkillID))
                                       select c;
                    _context.RemoveRange(removeSkills);
                    var addSkills = from c in newSkills
                                    where (!oldSkills.Contains(c))
                                    select c;


                    //
                    var oldLevels = post.PostLevels.Select(e => e.LevelID).ToArray();
                    var newLevels = newPost.LevelIds;

                    var removeLevels = from c in post.PostLevels
                                       where (!newLevels.Contains(c.LevelID))
                                       select c;
                    _context.RemoveRange(removeLevels);
                    var addLevels = from c in newLevels
                                    where (!oldLevels.Contains(c))
                                    select c;


                    foreach (var levelId in addLevels)
                    {
                        _context.PostLevels.Add(new PostLevel()
                        {
                            PostID = id,
                            LevelID = levelId
                        });
                    }


                    foreach (var skillId in addSkills)
                    {
                        _context.PostSkills.Add(new PostSkills()
                        {
                            PostID = id,
                            SkillID = skillId,
                        });
                    }
                    _context.Update(post);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(newPost.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Post/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }

        [HttpPost]
        public IActionResult Favourite(string userId, int? postId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", controllerName: "Account", new { area = "Identity" });
            }
            if (postId != null)
            {
                _context.Favourites.Add(new Favourite
                {
                    PostID = (int)postId,
                    UserID = userId
                });
            }
            _context.SaveChanges();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public IActionResult UnFavourite(string userId, int? postId)
        {

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", controllerName: "Account", new { area = "Identity" });
            }
            if (postId != null)
            {
                var follower = _context.Favourites.FirstOrDefault(e => e.PostID == postId && e.UserID == userId);
                _context.Favourites.Remove(follower);
            }
            _context.SaveChanges();
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }

}
