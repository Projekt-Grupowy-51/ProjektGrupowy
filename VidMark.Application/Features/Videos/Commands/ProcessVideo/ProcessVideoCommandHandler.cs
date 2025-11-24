using System.Net.WebSockets;
using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using FluentResults;
using MediatR;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.SignalR;
using VidMark.Application.Interfaces.UnitOfWork;

namespace VidMark.Application.Features.Videos.Commands.ProcessVideo;

public class ProcessVideoCommandHandler(
    IVideoRepository videoRepository,
    IMessageService messageService,
    IUnitOfWork unitOfWork) : IRequestHandler<ProcessVideoCommand, Result>
{
    public async Task<Result> Handle(ProcessVideoCommand request, CancellationToken cancellationToken)
    {
        var video = await videoRepository.GetVideoAsync(request.Id, request.UserId, request.IsAdmin);
        if (video is null) return Result.Fail("No video found");

        var info = await FFProbe.AnalyseAsync(video.Path, cancellationToken: cancellationToken);
        var v = info.PrimaryVideoStream;
        if (v is null) return Result.Ok();

        var baseW = v.Width;
        var baseH = v.Height;
        var (w2, h2) = Halve(baseW, baseH);
        var (w4, h4) = Quarter(baseW, baseH);

        var dir = Path.GetDirectoryName(video.Path)!;
        Directory.CreateDirectory(dir); // ensure exists

        var stem = Path.GetFileNameWithoutExtension(video.Path);
        var out2 = Path.Combine(dir, $"{stem}_{w2}x{h2}.mp4");
        var out4 = Path.Combine(dir, $"{stem}_{w4}x{h4}.mp4");

        await using var fs2 = File.Open(out2, FileMode.Create, FileAccess.Write, FileShare.None);
        await using var fs4 = File.Open(out4, FileMode.Create, FileAccess.Write, FileShare.None);

        var t2 = FFMpegArguments
            .FromFileInput(video.Path)
            .OutputToPipe(new StreamPipeSink(fs2), options => options
                .ForceFormat("mp4")
                .WithVideoCodec(VideoCodec.LibX264)
                .WithAudioCodec(AudioCodec.Aac)
                .WithVideoFilters(f => f.Scale(w2, h2))
                .WithConstantRateFactor(23)
                .WithCustomArgument("-movflags +frag_keyframe+empty_moov"))
            .ProcessAsynchronously();

        var t4 = FFMpegArguments
            .FromFileInput(video.Path)
            .OutputToPipe(new StreamPipeSink(fs4), options => options
                .ForceFormat("mp4")
                .WithVideoCodec(VideoCodec.LibX264)
                .WithAudioCodec(AudioCodec.Aac)
                .WithVideoFilters(f => f.Scale(w4, h4))
                .WithConstantRateFactor(23)
                .WithCustomArgument("-movflags +frag_keyframe+empty_moov"))
            .ProcessAsynchronously();

        await Task.WhenAll(t2, t4);

        await messageService.SendVideoProcessedAsync(request.UserId, "Video processed successfully.", cancellationToken);
        await messageService.SendSuccessAsync(request.UserId, "Video processed successfully.", cancellationToken);

        var baseQuality = $"{baseW}x{baseH}";
        video.OriginalQuality = baseQuality;
        video.AvailableQualities = [baseQuality, $"{w2}x{h2}", $"{w4}x{h4}"];
        videoRepository.UpdateVideo(video);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();

        static int Even(int x) => x & ~1;
        static (int w, int h) Halve(int w, int h) => (Math.Max(2, Even(w / 2)), Math.Max(2, Even(h / 2)));
        static (int w, int h) Quarter(int w, int h) => (Math.Max(2, Even(w / 4)), Math.Max(2, Even(h / 4)));
    }
}