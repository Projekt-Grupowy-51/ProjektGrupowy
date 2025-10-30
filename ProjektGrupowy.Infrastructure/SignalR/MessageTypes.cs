namespace ProjektGrupowy.Infrastructure.SignalR
{
    public static class MessageTypes
    {
        public const string Success = "NotifyFromBackendServerSuccess";
        public const string Error = "NotifyFromBackendServerError";
        public const string Warning = "NotifyFromBackendServerWarning";
        public const string Info = "NotifyFromBackendServerInfo";
        public const string Message = "NotifyFromBackendServerMessage";
        public const string LabelersCountChanged = "LabelersCountChanged";
        public const string ReportGenerated = "ReportGenerated";
    }
}