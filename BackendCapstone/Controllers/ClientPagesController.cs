using System;
using System.Collections.Generic;
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

namespace BackendCapstone.Controllers
{
    public class ClientPagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ClientPagesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: ClientPages
        public async Task<IActionResult> Index()
        {
            return View(await _context.ClientPages.ToListAsync());
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
            if (clientPage == null)
            {
                return NotFound();
            }

            return View(clientPage);
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
            var viewModel = new ClientPageCreateEditViewModel()
            {
                ClientPage = clientPage
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
                        var images = Directory.GetFiles("wwwroot/images");
                        var fileToDelete = images.First(i => i.Contains(currentFileName));
                        System.IO.File.Delete(fileToDelete);
                        var uniqueFileName = GetUniqueFileName(viewModel.Img.FileName);
                        var imageDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                        var filePath = Path.Combine(imageDirectory, uniqueFileName);
                        using (var myFile = new FileStream(filePath, FileMode.Create))
                        {
                            viewModel.Img.CopyTo(myFile);
                        }
                        viewModel.ClientPage.ImgPath = uniqueFileName;
                        _context.Update(clientPage);
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

            return View(clientPage);
        }

        // POST: ClientPages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clientPage = await _context.ClientPages.FindAsync(id);
            var currentFileName = clientPage.ImgPath;
            var images = Directory.GetFiles("wwwroot/images");
            var fileToDelete = images.First(i => i.Contains(currentFileName));
            System.IO.File.Delete(fileToDelete);
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
