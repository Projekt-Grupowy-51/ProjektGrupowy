using MediatR;

namespace ProjektGrupowy.Application.CQRS;

public abstract record BaseCommand<TResponse>(string UserId, bool IsAdmin) : IRequest<TResponse>;
