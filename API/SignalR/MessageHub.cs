using System;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        public MessageHub(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _messageRepository = messageRepository;
        }

        public override async Task OnConnectedAsync()
        {
            var username = Context.User.GetUserName();
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"];
            var groupName = GetGroupName(username, otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var messages = await _messageRepository.GetMessageThread(username, otherUser);

            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var userName = Context.User.GetUserName();
            var recipientUserName = createMessageDto.RecipientUserName.ToLower();

            if (userName == recipientUserName)
                throw new HubException("You cannot send messages to yourself");

            var sender = await _userRepository.GetUserByUserNameAsync(userName);
            var recipient = await _userRepository.GetUserByUserNameAsync(recipientUserName);

            if (recipient == null) throw new HubException("Not found user");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUserName = sender.UserName,
                RecipientUserName = recipient.UserName,
                Content = createMessageDto.Content
            };

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync())
            {
                var group = GetGroupName(sender.UserName, recipient.UserName);
                await Clients.Group(group).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }
            else
                throw new HubException("Failed to send message");
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}