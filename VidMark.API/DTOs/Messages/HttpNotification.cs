namespace VidMark.API.DTOs.Messages;

public record HttpNotification(string UserId, string Type, string Message);