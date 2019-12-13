using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendCapstone.Models.ApplicationUserViewModels
{
    public class MarketingUserDetailsViewModel
    {
       
        public ApplicationUser User { get; set; }

        public List<ClientPage> ClientPages { get; set; }

        public List<StoryBoard> StoryBoards { get; set; }

        public List<Message> Messages { get; set; }
    }
}
