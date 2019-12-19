using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BackendCapstone.Data;
using BackendCapstone.Models;
using System.IO;
using Microsoft.AspNetCore.Http;
using BackendCapstone.Models.ClientPageViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BackendCapstone.Controllers
{
    [Authorize]
    public class ClientPagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClientPagesController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: ClientPages
        public async Task<IActionResult> Index()
        {
            var currentUser = await GetCurrentUserAsync();
            
            //Admin users see all Client Pages
            if (currentUser.UserTypeId == 1)
            {
                var allClientPages = await _context.ClientPages.ToListAsync();
                return View(allClientPages);
            }

            //Rep users see a list of their currently assigned Client Pages
            if (currentUser.UserTypeId == 2)
            {
                var assignedClientPages = await _context.ClientPageUsers
                .Where(cp => cp.UserId == currentUser.Id)
                .Select(cp => cp.ClientPage)
                .ToListAsync();
                return View(assignedClientPages);           
            }

            //So that client users cannot access other Client Pages using URL:
            return NotFound();
        }

        // GET: ClientPages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var clientPage = await _context.ClientPages
                .Include(m => m.StoryBoards)
                .FirstOrDefaultAsync(m => m.Id == id);

            var orderedStoryBoards = await _context.StoryBoards             
                .Include(sb => sb.Chats)
                .ThenInclude(c => c.User)
                .OrderBy(sb => sb.PostDateTime)
                .Where(sb => sb.ClientPageId == clientPage.Id)
                .ToListAsync();
            
            var assignedUsers = await _context.ClientPageUsers
                .Include(cpu => cpu.User)
                .Where(cpu => cpu.ClientPageId == id)
                .Select(cpu => cpu.User)
                .ToListAsync();

            

            clientPage.StoryBoards = orderedStoryBoards;
            clientPage.Users = assignedUsers;

            var viewModel = new ClientPageDetailsViewModel()
            {
                ClientPage = clientPage,             
            };

            if (clientPage == null)
            {
                return NotFound();
            }
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostChat(ClientPageDetailsViewModel viewModel)
        {
            var currentUser = await GetCurrentUserAsync();

            var chat = new Chat()
            {
                Text = viewModel.ChatText,
                StoryBoardId = viewModel.StoryBoardId,
                UserId = currentUser.Id,
                Timestamp = DateTime.Now
            };
           
            if (ModelState.IsValid)
            {
                _context.Add(chat);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { Id = viewModel.ClientPage.Id });
        }

        // GET: ClientPages/Create
        public IActionResult Create()
        {
            var viewModel = new ClientPageCreateEditViewModel();
            return View(viewModel);
        }

        // POST: ClientPages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientPageCreateEditViewModel viewModel)
        {           
            if (ModelState.IsValid)
            {
                if (viewModel.Img != null)
                {
                    var uniqueFileName = GetUniqueFileName(viewModel.Img.FileName);
                    var imageDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    var filePath = Path.Combine(imageDirectory, uniqueFileName);
                    using (var myFile = new FileStream(filePath, FileMode.Create))
                    {
                        viewModel.Img.CopyTo(myFile);
                    }                   
                    viewModel.ClientPage.ImgPath = uniqueFileName;
                }
                _context.Add(viewModel.ClientPage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));         
            }
            return View(viewModel);
        }     

       

        // GET: ClientPages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientPage = await _context.ClientPages.FindAsync(id);

            var clientUsers = await _context.ApplicationUsers
                .Where(u => u.UserTypeId == 3)
                .ToListAsync();

            var clientUserOptions = clientUsers.Select(u => new SelectListItem(u.FirstName + " " + u.LastName, u.Id)).ToList();

            var viewModel = new ClientPageCreateEditViewModel()
            {
                ClientPage = clientPage,
                ClientUsers = clientUsers,
                ClientUserOptions = clientUserOptions
            };

            
            return View(viewModel);
        }

        // POST: ClientPages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClientPageCreateEditViewModel viewModel)
         {
            var clientPage = viewModel.ClientPage;
            if (id != clientPage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var currentFileName = viewModel.ClientPage.ImgPath;
                    if (viewModel.Img != null && viewModel.Img.FileName != currentFileName)
                    {
                        if (currentFileName != null)
                        {
                            var images = Directory.GetFiles("wwwroot/images");
                            var fileToDelete = images.First(i => i.Contains(currentFileName));
                            System.IO.File.Delete(fileToDelete);
                        }
                        var uniqueFileName = GetUniqueFileName(viewModel.Img.FileName);
                        var imageDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                        var filePath = Path.Combine(imageDirectory, uniqueFileName);
                        using (var myFile = new FileStream(filePath, FileMode.Create))
                        {
                            viewModel.Img.CopyTo(myFile);
                        }
                        viewModel.ClientPage.ImgPath = uniqueFileName;                       
                    }
                    if (viewModel.ClientUserId != null)
                    {
                        var clientPageUser = new ClientPageUser()
                        {
                            UserId = viewModel.ClientUserId,
                            ClientPageId = viewModel.ClientPage.Id
                        };
                        _context.Add(clientPageUser);
                        await _context.SaveChangesAsync();
                    }
                    _context.Update(clientPage);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientPageExists(clientPage.Id))
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
            return View(clientPage);
        }


        // GET: ClientPages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientPage = await _context.ClientPages
                .FirstOrDefaultAsync(m => m.Id == id);

            if (clientPage == null)
            {
                return NotFound();
            }

            var assignedUsers = await _context.ClientPageUsers
                .Include(cpu => cpu.User)
                .Where(cpu => cpu.ClientPageId == id)
                .Select(cpu => cpu.User)
                .ToListAsync();

            clientPage.Users = assignedUsers;


            return View(clientPage);
        }

        // POST: ClientPages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clientPage = await _context.ClientPages.FindAsync(id);
            var currentFileName = clientPage.ImgPath;
            if (currentFileName != null)
            {
                var images = Directory.GetFiles("wwwroot/images");
                var fileToDelete = images.First(i => i.Contains(currentFileName));
                System.IO.File.Delete(fileToDelete);
            }
            _context.ClientPages.Remove(clientPage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientPageExists(int id)
        {
            return _context.ClientPages.Any(e => e.Id == id);
        }

        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
        }
    }
}
