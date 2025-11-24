using MediatR;
using Microsoft.Extensions.Logging;
using ProjektGrupowy.Application.CQRS;
using ProjektGrupowy.Application.Interfaces.UnitOfWork;

namespace ProjektGrupowy.Application.Pipelines;

/// <summary>
/// Pipeline behavior that wraps command execution in a database transaction.
/// Only applies to commands (BaseCommand derivatives), not queries.
/// Transaction is committed on success and rolled back on failure.
/// </summary>
public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(
        IUnitOfWork unitOfWork,
        ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Only apply transaction to commands, not queries
        if (!IsCommand())
        {
            return await next();
        }

        _logger.LogCritical("Beginning transaction for {CommandName}", typeof(TRequest).Name);

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var response = await next();

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogCritical("Transaction committed successfully for {CommandName}", typeof(TRequest).Name);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed for {CommandName}. Rolling back.", typeof(TRequest).Name);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private static bool IsCommand()
    {
        var requestType = typeof(TRequest);

        // Check if TRequest inherits from BaseCommand<TResponse>
        var baseType = requestType.BaseType;
        while (baseType != null)
        {
            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition().Name.StartsWith("BaseCommand"))
            {
                return true;
            }
            baseType = baseType.BaseType;
        }

        return false;
    }
}
