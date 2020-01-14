using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackendCapstone.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime DateTime { get; set; }

        public string Location { get; set; }

        [Display(Name = "Employees Only")]
        public bool IsClosed { get; set; }

        public string ImgPath { get; set; }

        public List<ApplicationUser> Attendees { get; set; }
    }
}
