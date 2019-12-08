using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendCapstone.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsComplete { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime Timestampe { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
