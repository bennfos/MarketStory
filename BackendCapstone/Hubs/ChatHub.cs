using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendCapstone.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message, int storyBoardId)
        {
            await Clients.All.SendAsync("ReceiveMessage", message, storyBoardId);
        }
    }
}
