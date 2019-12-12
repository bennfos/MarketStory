using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendCapstone.Data;
using BackendCapstone.Models;
using BackendCapstone.Models.ApplicationUserViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendCapstone.Controllers
{
    public class ApplicationUsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public ApplicationUsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> MarketingUserDetails([FromRoute] string id)
        {
            var marketingUser = await _context.ApplicationUsers
                .Include(u => u.UserType)
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();

            var upcomingStoryBoards = await _context.StoryBoards
                .OrderBy(sb => sb.PostDateTime)
                .Include(sb => sb.ClientPage)
                .Where(sb => sb.UserId == id)                
                .Take(6)
                .ToListAsync();

            var userClientPages = await _context.ClientPageUsers
                .Include(cp => cp.ClientPage)
                .Where(cp => cp.UserId == id)                
                .Select(ucp => ucp.ClientPage).ToListAsync();

            var viewModel = new MarketingUserDetailsViewModel()
            {
                User = marketingUser,
                StoryBoards = upcomingStoryBoards,
                ClientPages = userClientPages
            };

            return View(viewModel);
        }
    }
}