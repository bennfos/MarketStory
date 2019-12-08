using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendCapstone.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public DateTime DateTime { get; set; }

        public string Location { get; set; }

        public bool IsClosed { get; set; }

        public string ImgPath { get; set; }

        public List<ApplicationUser> Attendees { get; set; }
    }
}
