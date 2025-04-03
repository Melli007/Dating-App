using System;
using API.DTOs;
using API.Models;

namespace API.Helpers;

public static class MessageMapper
{
    public static MessageDto MapMessageToDto(Message message)
    {
        return new MessageDto
        {
         Id = message.Id,
        SenderId = message.SenderId,
        SenderUsername = message.SenderUsername,
        // Use null-conditional operator to prevent null reference exceptions
        SenderPhotoUrl = message.Sender.Photos.FirstOrDefault(x => x.IsMain)!.Url ,
        RecipientId = message.RecipientId,
        RecipientUsername = message.RecipientUsername,
        RecipientPhotoUrl = message.Recipient.Photos.FirstOrDefault(x => x.IsMain)!.Url,
        Content = message.Content,
        // Convert DateRead and MessageSent to UTC explicitly
        DateRead = message.DateRead.HasValue 
                      ? DateTime.SpecifyKind(message.DateRead.Value, DateTimeKind.Utc) 
                      : null,
        MessageSent = DateTime.SpecifyKind((DateTime)message.MessageSent!, DateTimeKind.Utc)
        };
    }
}

