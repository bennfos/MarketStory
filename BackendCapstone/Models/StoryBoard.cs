using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendCapstone.Models
{
    public class StoryBoard
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string ImgPath { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime PostDateTime { get; set; }
        public int ClientPageId { get; set; }
        public ClientPage ClientPage { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

    }
}
