﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using OnlineShop.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace OnlineShop.Hubs
{
    public class ChatHub : Hub
    {// Dictionary lưu trữ kết nối của từng UserId
        private static Dictionary<string, string> _connections = new Dictionary<string, string>();
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChatHub(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public override Task OnConnectedAsync()
        {
            // Lưu trữ kết nối khi người dùng kết nối
            var userId = _httpContextAccessor.HttpContext.Session.GetString("userId");
            if(userId != null) {
                var connectionId = Context.ConnectionId;
                _connections[userId] = connectionId;
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            // Kiểm tra HttpContext và Session có null không
            if (_httpContextAccessor.HttpContext != null &&
                _httpContextAccessor.HttpContext.Session != null)
            {
                var userId = _httpContextAccessor.HttpContext.Session.GetString("userId");

                // Kiểm tra userId có null không
                if (!string.IsNullOrEmpty(userId) && _connections.ContainsKey(userId))
                {
                    _connections.Remove(userId);
                }
            }


            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToUser(string userId, string senderId, string message)
        {
            if (_connections.ContainsKey(userId))
            {
                var connectionId = _connections[userId];
               
                    await Clients.Client(connectionId).SendAsync("ReceiveMessage", senderId, message);
                
            }
            else
            {
                // Lưu trữ tin nhắn nếu người dùng không trực tiếp kết nối
                // và gửi khi họ kết nối trở lại
                // (Cần cơ chế lưu trữ và gửi lại tin nhắn khi người dùng kết nối trở lại)
            }
        }

		public async Task SendImage(string userId, string senderId, string imageData)
		{
			if (_connections.ContainsKey(userId))
			{
				var connectionId = _connections[userId];

				await Clients.Client(connectionId).SendAsync("ReceiveImage", senderId, imageData);

			}
			else
			{
				// Lưu trữ tin nhắn nếu người dùng không trực tiếp kết nối
				// và gửi khi họ kết nối trở lại
				// (Cần cơ chế lưu trữ và gửi lại tin nhắn khi người dùng kết nối trở lại)
			}
		}

        public async Task CloseCallToUser(string userId)
        {
            if (_connections.ContainsKey(userId))
            {
                var connectionId = _connections[userId];

                await Clients.Client(connectionId).SendAsync("CallEnded");

            }
            else
            {
                // Lưu trữ tin nhắn nếu người dùng không trực tiếp kết nối
                // và gửi khi họ kết nối trở lại
                // (Cần cơ chế lưu trữ và gửi lại tin nhắn khi người dùng kết nối trở lại)
            }
        }
		public async Task NotifyIncomingCall(string userId, string senderId=null)

		{
            if (_connections.ContainsKey(userId))
            {
                var connectionId = _connections[userId];
                if (senderId == null)
                {
					await Clients.Client(connectionId).SendAsync("IncomingCall");
                }
                else
                {
					await Clients.Client(connectionId).SendAsync("IncomingCall", senderId);
				}
                
            }
           
		}


        public async Task Notice(string userId)

        {
            if (_connections.ContainsKey(userId))
            {
                var connectionId = _connections[userId];
                await Clients.Client(connectionId).SendAsync("SignalNotice");

            }

        }

    }
}
