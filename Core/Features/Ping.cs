using MediatR;

namespace Core.Features;

public record PingRequest : IRequest<string>
{
    public required string Message { get; init; }
}

public class PingHandler : IRequestHandler<PingRequest, string>
{
    public Task<string> Handle(PingRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult($"Pong: {request.Message}");
    }
}