using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BackendCapstone.Data;
using BackendCapstone.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace BackendCapstone.Controllers
{
    public class StoryBoardsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StoryBoardsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: StoryBoards
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.StoryBoards.Include(s => s.ClientPage).Include(s => s.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: StoryBoards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storyBoard = await _context.StoryBoards
                .Include(s => s.ClientPage)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (storyBoard == null)
            {
                return NotFound();
            }

            return View(storyBoard);
        }

        // GET: StoryBoards/Create
        public IActionResult Create([FromRoute]int id)
        {
            var storyBoard = new StoryBoard()
            {
                ClientPageId = id
            };

            return View(storyBoard);
        }

        // POST: StoryBoards/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Text,ImgPath,Timestamp,PostDateTime,ClientPageId,UserId")] StoryBoard storyBoard)
        {
            var currentUser = await GetCurrentUserAsync();
            if (ModelState.IsValid)
            {
                storyBoard.Timestamp = DateTime.Now;
                storyBoard.UserId = currentUser.Id;
                _context.Add(storyBoard);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "ClientPages", new { Id = storyBoard.ClientPageId } );
            }
           
            return View(storyBoard);
        }

        // GET: StoryBoards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storyBoard = await _context.StoryBoards
                .Where(sb => sb.Id == id)
                .FirstOrDefaultAsync();
            if (storyBoard == null)
            {
                return NotFound();
            }
            
            return View(storyBoard);
        }

        // POST: StoryBoards/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Text,ImgPath,Timestamp,PostDateTime,ClientPageId,UserId")] StoryBoard storyBoard)
        {
            if (id != storyBoard.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(storyBoard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoryBoardExists(storyBoard.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "ClientPages", new { Id = storyBoard.ClientPageId });
            }
            
            return View(storyBoard);
        }

        // GET: StoryBoards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storyBoard = await _context.StoryBoards
                .Include(s => s.ClientPage)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (storyBoard == null)
            {
                return NotFound();
            }

            return View(storyBoard);
        }

        // POST: StoryBoards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var storyBoard = await _context.StoryBoards.FindAsync(id);
            _context.StoryBoards.Remove(storyBoard);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "ClientPages", new { Id = storyBoard.ClientPageId });
        }

        private bool StoryBoardExists(int id)
        {
            return _context.StoryBoards.Any(e => e.Id == id);
        }
    }
}
