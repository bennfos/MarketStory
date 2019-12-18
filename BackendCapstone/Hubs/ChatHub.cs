using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendCapstone.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string storyBoardId, string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", storyBoardId, user, message);
        }
    }
}
