using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendCapstone.Models.HomeViewModels
{
    public class RepHomeViewModel
    {
        public ApplicationUser User { get; set;}

        public List<StoryBoard> StoryBoards { get; set; }

        public List<ClientPageUser> ClientPageUsers { get; set; }
    }
}
