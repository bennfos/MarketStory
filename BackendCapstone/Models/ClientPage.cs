﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackendCapstone.Models
{
    public class ClientPage 
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ImgPath { get; set; }
         
      
        [NotMapped]
        public List<ApplicationUser> Users { get; set; }
        [NotMapped]
        public List<ApplicationUser> ClientUsers { get; set; }

       [NotMapped]
        public StoryBoard StoryBoard { get; set; }

        public List<StoryBoard> StoryBoards { get; set; }

    }
}
