﻿using System;
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

            if (currentUser.UserTypeId == 2)
            {
                return RedirectToAction("RepHome", new { Id = currentUser.Id});
            }

            if (currentUser.UserTypeId == 3)
            {
                var clientPageUser = await _context.ClientPageUsers                
                    .Where(cpu => cpu.UserId == currentUser.Id)                   
                    .FirstOrDefaultAsync();

                return RedirectToAction("Details", "ClientPages", new { Id = clientPageUser.ClientPageId});
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

        public async Task<IActionResult> RepHome(string id)
        {
            var upcomingStoryBoards = await _context.StoryBoards
                .OrderBy(sb => sb.PostDateTime)
                .Include(sb => sb.ClientPage)
                .Where(sb => sb.UserId == id)
                .Take(6)
                .ToListAsync();

            var clientPageUsers = await _context.ClientPageUsers
                .Include(cp => cp.ClientPage)
                .Include(cp => cp.User)
                .Where(cp => cp.UserId == id)
                .ToListAsync();

            var viewModel = new RepHomeViewModel()
            {
                User = await GetCurrentUserAsync(),
                StoryBoards = upcomingStoryBoards,
                ClientPageUsers = clientPageUsers            
            };

            return View(viewModel);
        }

        public async Task<IActionResult> ClientHome(string id)
        {
            
            var clientPage = await _context.ClientPageUsers
                .Where(cpu => cpu.UserId == id)
                .Select(cpu => cpu.ClientPage)
                .FirstOrDefaultAsync();     
            var users = await _context.ClientPageUsers
                .Include(cpu => cpu.User)
                .Where(cpu => cpu.ClientPageId == clientPage.Id)
                .Select(cpu => cpu.User)
                .ToListAsync();
            var orderedStoryBoards = await _context.StoryBoards
                .OrderBy(sb => sb.PostDateTime)
                .Where(sb => sb.ClientPageId == clientPage.Id)
                .ToListAsync();

            clientPage.Users = users;
            clientPage.StoryBoards = orderedStoryBoards;
            return View(clientPage);
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
