using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendCapstone.Data;
using BackendCapstone.Models;
using BackendCapstone.Models.ApplicationUserViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public async Task<IActionResult> EditMarketingUser(string id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.ApplicationUsers.Where(u => u.Id == id).FirstOrDefaultAsync();
            var userTypeOptions = await _context.UserTypes.Where(ut => ut.Id == 1 || ut.Id == 2).Select(ut => new SelectListItem(ut.Type, ut.Id.ToString())).ToListAsync();
            var assignedClientPages = await _context.ClientPageUsers.Where(cp => cp.UserId == id).Select(cp => cp.ClientPage).ToListAsync();
            var clientPageOptions = await _context.ClientPageUsers.Where(cpu => cpu.UserId != id).Select(cpu => new SelectListItem(cpu.ClientPage.Name, cpu.ClientPageId.ToString())).ToListAsync();
           
            clientPageOptions.Insert(0, new SelectListItem
            {
                Text = "Choose Client Page to assign....",
                Value = "0"
            });
            var viewModel = new EditMarketingUserViewModel()
            {
                User = user,
                UserTypeId = user.UserTypeId,
                UserTypeOptions = userTypeOptions,
                AssignedClientPages = assignedClientPages,
                ClientPageOptions = clientPageOptions,
                UserId = id
                
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMarketingUser(EditMarketingUserViewModel viewModel)
        {
           
            var clientPageUser = viewModel.ClientPageUser;
            var userId = viewModel.UserId;
            var userToEdit = await _context.ApplicationUsers.Where(u => u.Id == userId).FirstOrDefaultAsync();
            ModelState.Remove("User.FirstName");
            ModelState.Remove("User.LastName");
            if (ModelState.IsValid)
            {
                if (clientPageUser.ClientPageId != 0)
                {
                    clientPageUser.UserId = userId;
                    _context.Update(clientPageUser);
                    await _context.SaveChangesAsync();                      
                }
                if (userToEdit.UserTypeId != viewModel.UserTypeId)
                {
                    userToEdit.UserTypeId = viewModel.UserTypeId;
                    _context.Update(userToEdit);
                    await _context.SaveChangesAsync();
                
                }
                return RedirectToAction(nameof(MarketingUserDetails));
            }
            return View(viewModel);
        }
    }
}
