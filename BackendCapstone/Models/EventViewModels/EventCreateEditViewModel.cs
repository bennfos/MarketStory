using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendCapstone.Models.EventViewModels
{
    public class EventCreateEditViewModel
    {
        public Event Event { get; set; }

        public IFormFile Img { get; set; }
    }
}
