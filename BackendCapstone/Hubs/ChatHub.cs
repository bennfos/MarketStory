using BackendCapstone.Data;
using BackendCapstone.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace BackendCapstone.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatHub(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

       
        public async System.Threading.Tasks.Task SendMessage(string storyBoardId, string userId, string message)
        {
            var chat = new Chat()
            {
                Text = message,
                StoryBoardId = int.Parse(storyBoardId),
                UserId = userId,
                Timestamp = DateTime.Now
            };           
                    
            _context.Add(chat);
            await _context.SaveChangesAsync();

            var chatUser = await _context.Chat
                .Include(c => c.User)
                .Where(c => c.Timestamp == chat.Timestamp)
                .Select(c => c.User)
                .FirstOrDefaultAsync();

            var otherChatUserName = $"{chatUser.FirstName} {chatUser.LastName}";

            await Clients.Others.SendAsync("OthersReceiveMessage", storyBoardId, userId, otherChatUserName, message);
            await Clients.Caller.SendAsync("CallerReceiveMessage", storyBoardId, userId, message);
            
        }

        

        public async System.Threading.Tasks.Task SendApproval(string storyBoardId)
        {
            var storyBoard = await _context.StoryBoards
                .Where(sb => sb.Id == int.Parse(storyBoardId))
                .FirstOrDefaultAsync();

            if (storyBoard.IsApproved == false)
            {
                storyBoard.IsApproved = true;
                _context.Update(storyBoard);
                await _context.SaveChangesAsync();
                await Clients.All.SendAsync("ReceiveApproval", storyBoardId);
            }
            else 
            {
                storyBoard.IsApproved = false;
                _context.Update(storyBoard);
                await _context.SaveChangesAsync();
                await Clients.All.SendAsync("RemoveApproval", storyBoardId);
            }                   
        }

        
    }
}
