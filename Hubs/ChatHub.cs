using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ApartmentNetwork.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.SignalR;
//CHANGED THIS FROM SIGNALR

namespace ApartmentNetwork.Hubs
{
    public class ChatHub : Hub
    {
        private MyContext _context;
        public ChatHub(MyContext context)
        {
            _context = context;
        }
        // private readonly UserManager<User> userManager;
        // public ChatHub(UserManager<User> userManager)
        // {
        //     this.userManager = userManager;
        // }
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        // public async override Task OnConnectedAsync()
        // {
        //     int? sessionID = HttpContext.Session.GetInt32("UserId");

        //     // var user = await userManager.FindByNameAsync(Context.User == _context.Users);
        //     var user = await _context.Users
        //             .FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
        //     Console.WriteLine("This is the user from ChatHub:" + user);
        // }
        // if (user != null)
            //         // {
            //             // if (user.UserType == UserType.Administrator)
            //             // {
            //             //     await AddToGroup("Administrators");
            //             // }
            //             // else if (user.UserType == UserType.Employee)
            //             // {
            //             //     await AddToGroup("Employees");
            //             // }

        //*****// AJG Im trying to add in the BUILDING ID into the message so the receiving HTML knows which messages to display, as long as they match the sending Users's Building ID

        public async Task SendNotification(string user, string message, string bldgId)
        {
            // if (_context.Users.Any(u => u.BuildingId.ToString() == bldgId))
            // {
            Console.WriteLine("You're about to send to user with a matching buliding Id");
            await Clients.Group(bldgId).SendAsync("ReceiveNotification",user, message, bldgId);
            // }
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
            Console.WriteLine("Now youre in a group!");
        }
        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
        }
    }
}

    // public class ChatHub : Hub
    // {
    //     private readonly UserManager<User> userManager;
    //     public ChatHub(UserManager<User> userManager)
    //     {
    //         this.userManager = userManager;
    //     }
    //     public async override Task OnConnectedAsync()
    //     {
    //         var user = await userManager.FindByNameAsync(Context.User.Identity.Name);
    //         Console.WriteLine("This is the user from ChatHub:" +user);
    //         // if (user != null)
    //         // {
    //             // if (user.UserType == UserType.Administrator)
    //             // {
    //             //     await AddToGroup("Administrators");
    //             // }
    //             // else if (user.UserType == UserType.Employee)
    //             // {
    //             //     await AddToGroup("Employees");
    //             // }
    //         // }
    //         // else
    //         // {
    //         //     await Clients.Caller.SendAsync("ReceiveNotification", "Connection Error", 0);
    //         // }
    //     }


    //     public async Task SendNotification(string group, string message, int messageType)
    //     {
    //         await Clients.Group(group).SendAsync("ReceiveNotification", message, messageType);
    //     }

    //     public async Task AddToGroup(string groupName)
    //     {
    //         await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    //     }

    //     public async Task RemoveFromGroup(string groupName)
    //     {
    //         await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    //     }
    // }