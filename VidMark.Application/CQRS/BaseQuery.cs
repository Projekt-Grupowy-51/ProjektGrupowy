using MediatR;

namespace VidMark.Application.CQRS;

public abstract record BaseQuery<TResponse>(string UserId, bool IsAdmin) : IRequest<TResponse>;
