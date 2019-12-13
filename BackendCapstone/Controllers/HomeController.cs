using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BackendCapstone.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using BackendCapstone.Data;
using Microsoft.EntityFrameworkCore;
using BackendCapstone.Models.HomeViewModels;

namespace BackendCapstone.Controllers
{ 
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public async Task<IActionResult> Index()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser.UserTypeId == 1)
            {
                return RedirectToAction(nameof(AdminHome));
            }
                return View();
        }

        public async Task<IActionResult> AdminHome()
        {          
            var admin = await _context.ApplicationUsers
                        .Include(u => u.UserType)
                        .Where(u => u.UserTypeId == 1)
                        .ToListAsync();
            var reps = await _context.ApplicationUsers
                        .Include(u => u.UserType)
                        .Where(u => u.UserTypeId == 2)
                        .ToListAsync();
            var clientPages = await _context.ClientPages
                        .ToListAsync();
            var viewModel = new AdminHomeViewModel()
            {
                ClientPages = clientPages,
                Reps = reps,
                Admins = admin
            };
            return View(viewModel);
        }

        
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
