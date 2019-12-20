using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BackendCapstone.Data;
using BackendCapstone.Models;
using BackendCapstone.Models.ClientPageViewModels;
using BackendCapstone.Models.EventViewModels;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

namespace BackendCapstone.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public EventsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events                             
                .Where(eu => eu.Id == id)
                .FirstOrDefaultAsync();

            var eventUsersBool = _context.EventUsers
                .Any(eu => eu.EventId == id);


            if (eventUsersBool == true)
            {
                var attendees = await _context.EventUsers
                    .Include(eu => eu.Event)
                    .Include(eu => eu.User)
                    .Where(eu => eu.EventId == @event.Id)
                    .Select(eu => eu.User)
                    .ToListAsync();

                @event.Attendees = attendees;               
            }


            
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        public async Task<IActionResult> Attend(int id)
        {
            var user = await GetCurrentUserAsync();

            var eventUser = new EventUser()
            {
                EventId = id,
                UserId = user.Id
            };

            _context.Add(eventUser);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { Id = id });
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            var viewModel = new EventCreateEditViewModel();
            return View(viewModel);
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventCreateEditViewModel viewModel)
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
                    viewModel.Event.ImgPath = uniqueFileName;
                }
                _context.Add(viewModel.Event);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }


        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);

            var viewModel = new EventCreateEditViewModel()
            {
                Event = @event
            };

            if (@event == null)
            {
                return NotFound();
            }
            return View(viewModel);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EventCreateEditViewModel viewModel)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    var currentFileName = viewModel.Event.ImgPath;
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
                        viewModel.Event.ImgPath = uniqueFileName;
                    }
                    
                    _context.Update(viewModel.Event);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(viewModel.Event.Id))
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
            return View(viewModel);
        }
        

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.Id == id);


            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            var currentFileName = @event.ImgPath;
            if (currentFileName != null)
            {
                var images = Directory.GetFiles("wwwroot/images");
                var fileToDelete = images.First(i => i.Contains(currentFileName));
                System.IO.File.Delete(fileToDelete);
            }
            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
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
