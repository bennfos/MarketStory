using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackendCapstone.Models.ApplicationUserViewModels
{
    public class EditMarketingUserViewModel
    {
        public string UserId {get; set;}
        
        public ApplicationUser User { get; set; }

        [Display(Name="User Type")]
        public int UserTypeId { get; set; }

        public List<SelectListItem> UserTypeOptions { get; set; }

        public List<ClientPage> AssignedClientPages { get; set; }

        public List<SelectListItem> ClientPageOptions { get; set; }

        [Display(Name = "Assign Client Page")]
        public ClientPageUser ClientPageUser { get; set; }
    }
}