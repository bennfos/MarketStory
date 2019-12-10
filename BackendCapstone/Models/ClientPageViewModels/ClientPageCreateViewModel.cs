using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendCapstone.Models.ClientPageViewModels
{
    public class ClientPageCreateViewModel
    {
        public ClientPage ClientPage { get; set; }

        public IFormFile Img { get; set; }
    }
}
