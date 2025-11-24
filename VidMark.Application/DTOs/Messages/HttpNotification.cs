namespace VidMark.Application.DTOs.Messages;

public record HttpNotification(string UserId, string Type, string Message);