using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackendCapstone.Models.StoryBoardViewModels
{
    public class StoryBoardCreateEditViewModel
    {
        public StoryBoard StoryBoard { get; set; }

        [Display(Name = "Text")]
        public string UpdatedText { get; set; }

        [Display(Name = "Post Date")]
        public DateTime UpdatedPostDateTime { get; set; }

        [Display(Name = "Image")]
        public IFormFile Img { get; set; }
    }
}
