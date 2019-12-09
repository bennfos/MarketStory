using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendCapstone.Models
{
    public class ClientPageEvent
    {
        public int Id { get; set; }

        public int ClientPageId { get; set; }

        public ClientPage ClientPage { get; set; }

        public int EventId { get; set; }

        public Event Event { get; set; }
    }
}
