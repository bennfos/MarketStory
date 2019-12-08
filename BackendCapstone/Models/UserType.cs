using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackendCapstone.Models
{
    public class UserType
    {
        public int Id { get; set; }

        public string Type { get; set; }

        [NotMapped]
        public List<ApplicationUser> Users { get; set; }

    }
}
