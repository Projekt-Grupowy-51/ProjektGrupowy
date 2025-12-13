namespace VidMark.Domain.Utils.Constants;

public static class VideoQualities
{
    public const string Quality1080P = "1080p";
    public const string Quality720P = "720p";
    public const string Quality480P = "480p";
    public const string Quality360P = "360p";

    public static readonly string[] AllQualities = [Quality1080P, Quality720P, Quality480P, Quality360P];
}