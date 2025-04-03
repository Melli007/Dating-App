using System;
using API.DTOs;
using API.Helpers;
using API.Interfaces;
using API.Models;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repository;

public class MessageRepository(DataContext context) : IMessageRepository
{
    public void AddGroup(Group group)
    {
       context.Groups.Add(group);
    }

    public void AddMessage(Message message)
    {
        context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
       context.Messages.Remove(message);
    }

    public async Task<Connection?> GetConnection(string connectionId)
    {
        return await context.Connections.FindAsync(connectionId);
    }

    public async Task<Group?> GetGroupForConnection(string connectionId)
    {
        return await context.Groups
        .Include(x => x.Connections)
        .Where(x => x.Connections.Any( c => c.ConnectionId == connectionId))
        .FirstOrDefaultAsync();
    }

    public async Task<Message?> GetMessage(int id)
    {
        return await context.Messages.FindAsync(id);
    }

    public async Task<Group?> GetMessageGroup(string groupName)
    {
        return await context.Groups
        .Include(x => x.Connections)
        .FirstOrDefaultAsync(x => x.Name == groupName);
    }

    public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
{
    var query = context.Messages
        .Include(m => m.Sender).ThenInclude(s => s.Photos)  
        .Include(m => m.Recipient).ThenInclude(r => r.Photos)  
        .OrderByDescending(x => x.MessageSent)
        .AsQueryable();

    query = messageParams.Container switch
    {
        "Inbox" => query.Where(x => x.Recipient.UserName == messageParams.Username 
        && x.RecipientDeleted == false),
        "Outbox" => query.Where(x => x.Sender.UserName == messageParams.Username 
        && x.SenderDeleted == false),
        _ => query.Where(x => x.Recipient.UserName == messageParams.Username && x.DateRead == null 
        && x.RecipientDeleted == false)
    };

    // Use `CreateAsync` to handle pagination in the database
    var pagedMessages = await PagedList<Message>.CreateAsync(query, messageParams.PageNumber, messageParams.PageSize);

    // Map each Message to a MessageDto using your helper
    var messageDtos = pagedMessages.Select(MessageMapper.MapMessageToDto).ToList();

    // Return paginated MessageDto list
    return new PagedList<MessageDto>(messageDtos, pagedMessages.TotalCount, pagedMessages.CurrentPage, pagedMessages.PageSize);
}

    public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
    {
        var query =  context.Messages
        .Include(m => m.Sender).ThenInclude(s => s.Photos)
        .Include(m => m.Recipient).ThenInclude(r => r.Photos)
        .Where(x => 
        x.RecipientUsername == currentUsername
         && x.RecipientDeleted == false 
         && x.SenderUsername == recipientUsername ||
        x.SenderUsername == currentUsername 
        && x.SenderDeleted == false 
        && x.RecipientUsername == recipientUsername
        ).OrderBy(x => x.MessageSent)
        .AsQueryable();

        var unreadMessages = query.Where(x => x.DateRead == null && x.RecipientUsername == currentUsername).ToList();

        if (unreadMessages.Count != 0)
        {
            unreadMessages.ForEach(x => x.DateRead = DateTime.UtcNow );
        }

      return await query.Select(message => MessageMapper.MapMessageToDto(message)).ToListAsync();
    }

    public void RemoveConnection(Connection connection)
    {
        context.Connections.Remove(connection);
    }
}
