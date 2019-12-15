using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackendCapstone.Models.ClientPageViewModels
{
    public class ClientPageCreateEditViewModel
    {
        public ClientPage ClientPage { get; set; }

        public List<ApplicationUser> ClientUsers { get; set; }

        public string ClientUserId { get; set; }

        public List<SelectListItem> ClientUserOptions { get; set; }

        [Display(Name = "Profile Image")]
        public IFormFile Img { get; set; }
    }
}
