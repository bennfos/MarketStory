using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendCapstone.Models.StoryBoardViewModels
{
    public class StoryBoardCreateEditViewModel
    {
        public StoryBoard StoryBoard { get; set; }

        public IFormFile Img { get; set; }
    }
}
