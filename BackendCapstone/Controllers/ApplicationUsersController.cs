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
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BackendCapstone.Controllers
{
    public class ApplicationUsersController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public ApplicationUsersController(IConfiguration config, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
            _config = config;
        }

        public SqlConnection Connection
        {
            
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
     

        public async Task<IActionResult> Details(string id)
        {
            var user = await _context.ApplicationUsers
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();

            if (user.UserTypeId == 1 || user.UserTypeId == 2)
            {            
                return RedirectToAction("MarketingUserDetails", new { Id = user.Id });
            }

            return RedirectToAction("ClientUserDetails", new { Id = user.Id });
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

            var clientPageUsers = await _context.ClientPageUsers
                .Include(cp => cp.ClientPage)
                .Where(cp => cp.UserId == id).ToListAsync();          

            var viewModel = new MarketingUserDetailsViewModel()
            {
                User = marketingUser,
                StoryBoards = upcomingStoryBoards,
                ClientPageUsers = clientPageUsers
            };

            return View(viewModel);
        }

        public async Task<IActionResult> ClientUserDetails([FromRoute] string id)
        {
            var clientUser = await _context.ApplicationUsers
                .Include(u => u.UserType)
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();

            var clientPageUsers = await _context.ClientPageUsers
                .Include(cp => cp.ClientPage)
                .Where(cp => cp.UserId == id)
                .ToListAsync();       

            return View(clientUser);
        }




        public async Task<IActionResult> EditMarketingUser(string id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.ApplicationUsers
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();
            var userTypeOptions = await _context.UserTypes
                .Where(ut => ut.Id == 1 || ut.Id == 2)
                .Select(ut => new SelectListItem(ut.Type, ut.Id.ToString()))
                .ToListAsync();
            var assignedClientPages = await _context.ClientPageUsers
                .Where(cp => cp.UserId == id)
                .Select(cp => cp.ClientPage)
                .ToListAsync();
           

            List<SelectListItem> assignClientPageOptions = new List<SelectListItem>();
                using (SqlConnection conn = Connection)
                    {
                        conn.Open();
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = @"SELECT DISTINCT cp.Id, cp.Name
                                FROM ClientPages cp LEFT JOIN ClientPageUsers cpu ON cpu.ClientPageId = cp.Id
                                WHERE cpu.Id IS NULL 
                                OR cpu.UserId != @id
                                AND cpu.ClientPageId NOT IN
                                (SELECT cpu.ClientPageId
                                FROM ClientPageUsers cpu WHERE cpu.UserId = @id);
                            ";
                            
                            cmd.Parameters.Add(new SqlParameter("@id", id));

                            SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {                      
                        if (!reader.IsDBNull(reader.GetOrdinal("Name")))
                        {
                            var clientPageName = reader.GetString(reader.GetOrdinal("Name"));
                            var clientPageId = reader.GetInt32(reader.GetOrdinal("Id"));
                            SelectListItem selectListItem = new SelectListItem(clientPageName, clientPageId.ToString());
                            assignClientPageOptions.Add(selectListItem);
                        };
                    }

                    reader.Close();
                }
            }
                
            assignClientPageOptions.Insert(0, new SelectListItem
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
                ClientPageOptions = assignClientPageOptions,
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
                return RedirectToAction("MarketingUserDetails", new { Id = viewModel.UserId });
            }
            return View(viewModel);
        }

        // GET: ClientPageUsers/UnassignClientPage/5
        public async Task<IActionResult> UnassignClientPage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var clientPageUser = await _context.ClientPageUsers
                .Include(cp => cp.ClientPage)
                .FirstOrDefaultAsync(m => m.Id == id);

            var assignedUsers = await _context.ClientPageUsers
                .Include(cpu => cpu.User)
                .Where(cpu => cpu.ClientPageId == clientPageUser.ClientPageId)
                .Select(cpu => cpu.User)
                .ToListAsync();

            clientPageUser.ClientPage.Users = assignedUsers;
            
            if (clientPageUser == null)
            {
                return NotFound();
            }

            return View(clientPageUser);
        }

        // POST: ClientPages/Delete/5
        [HttpPost, ActionName("UnassignClientPage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnassignClientPageConfirmed(int id)
        {
            var clientPageUser = await _context.ClientPageUsers.FindAsync(id);                              
            _context.ClientPageUsers.Remove(clientPageUser);
            await _context.SaveChangesAsync();
            return RedirectToAction("MarketingUserDetails", new { Id = clientPageUser.UserId });
        }

       
    }
}
