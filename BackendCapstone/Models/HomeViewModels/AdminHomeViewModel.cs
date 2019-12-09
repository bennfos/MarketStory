using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendCapstone.Models.HomeViewModels
{
    public class AdminHomeViewModel
    {
        public List<ClientPage> ClientPages { get; set; }

        public List<ApplicationUser> Reps { get; set; }

        public List<ApplicationUser> Admins { get; set; }
    }
}
