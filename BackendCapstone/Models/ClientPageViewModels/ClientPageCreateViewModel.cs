using Microsoft.AspNetCore.Http;
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

        [Display(Name = "Profile Image")]
        public IFormFile Img { get; set; }
    }
}
