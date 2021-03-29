using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers.Pagination;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public MessageRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public void AddGroup(Group group)
        => _context.Groups.Add(group);

        public async Task<Group> GetGroupForConnection(string connectionId)
        => await _context.Groups.Include(c => c.Connections)
            .Where(c => c.Connections.Any(x => x.ConnectionId == connectionId))
            .FirstOrDefaultAsync();

        public async Task<Connection> GetConnection(string connectionId)
        => await _context.Connections.FindAsync(connectionId);

        public async Task<Group> GetMessageGroup(string groupName)
        => await _context.Groups.Include(x => x.Connections).FirstOrDefaultAsync(x => x.Name == groupName);

        public void RemoveConnection(Connection connection)
        => _context.Connections.Remove(connection);

        public void AddMessage(Message message)
        => _context.Messages.Add(message);

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages.OrderByDescending(m => m.MessageSent)
                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider).AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.RecipientUserName == messageParams.UserName && u.RecipientDeleted == false),
                "Outbox" => query.Where(u => u.SenderUserName == messageParams.UserName && u.SenderDeleted == false),
                _ => query.Where(u => u.RecipientUserName == messageParams.UserName && u.RecipientDeleted == false && u.DateRead == null)
            };

            return await PagedList<MessageDto>.CreateAsync(query, messageParams.CurrentPage, messageParams.ItemsPerPage);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
        {
            var messages = _context.Messages
            .Where(m =>
                m.RecipientDeleted == false
                && m.RecipientUserName == currentUserName && m.SenderUserName == recipientUserName
                || m.RecipientUserName == recipientUserName && m.SenderUserName == currentUserName
                && m.SenderDeleted == false)
                .OrderBy(m => m.MessageSent)
                .AsQueryable();


            var unreadMessages = messages.Where(m => m.DateRead == null && m.RecipientUserName == currentUserName).ToList();

            if (unreadMessages.Any())
                unreadMessages.ForEach(m => m.DateRead = DateTime.UtcNow);

            return await messages.ProjectTo<MessageDto>(_mapper.ConfigurationProvider).ToListAsync();
        }
    }
}