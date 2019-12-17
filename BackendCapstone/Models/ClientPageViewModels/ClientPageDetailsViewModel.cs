using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendCapstone.Models.ClientPageViewModels
{
    public class ClientPageDetailsViewModel
    {
        public ClientPage ClientPage {get; set;}

        public int StoryBoardId { get; set; }
       
        public string ChatText { get; set; }
    }
}
