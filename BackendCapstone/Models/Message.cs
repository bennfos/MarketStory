﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendCapstone.Models
{
    public class Message
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public bool IsRead { get; set; }

        public DateTime Timestamp { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }        

        public string OtherUserId { get; set; }

        public ApplicationUser OtherUser { get; set; }
    }
}
