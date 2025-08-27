namespace DevWinUI;
public static partial class DispatcherExtensions
{
    /// <summary>
    /// Asynchronously executes an operation on the UI thread.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to use to execute the operation.</param>
    /// <param name="action">The async operation to execute.</param>
    /// <param name="token">An cancellation token to cancel the async operation.</param>
    /// <returns>A ValueTask to asynchronously track the completion of the operation.</returns>
    public static async ValueTask ExecuteAsync(this IDispatcher dispatcher, AsyncAction action, CancellationToken token)
        => await dispatcher.ExecuteAsync(
            async ct =>
            {
                await action(ct);
                return default(object);
            },
            token);


    /// <summary>
    /// Asynchronously executes an operation on the UI thread.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to use to execute the operation.</param>
    /// <param name="action">The async operation to execute.</param>
    /// <returns>A ValueTask to asynchronously track the completion of the operation.</returns>
    public static async ValueTask ExecuteAsync(this IDispatcher dispatcher, AsyncAction action)
        => await dispatcher.ExecuteAsync(
            async ct =>
            {
                await action(ct);
                return default(object);
            },
            CancellationToken.None);

    /// <summary>
    /// Asynchronously executes an operation on the UI thread.
    /// </summary>
    /// <typeparam name="TResult">Type of the result of the operation.</typeparam>
    /// <param name="dispatcher">The dispatcher to use to execute the operation.</param>
    /// <param name="func">The async operation to execute.</param>
    /// <returns>A ValueTask to asynchronously get the result of the operation.</returns>
    public static ValueTask<TResult> ExecuteAsync<TResult>(this IDispatcher dispatcher, AsyncFunc<TResult> func)
        => dispatcher.ExecuteAsync(
            async ct => await func(ct),
            CancellationToken.None);

    /// <summary>
    /// Asynchronously executes an operation on the UI thread.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to use to execute the operation.</param>
    /// <param name="action">The async operation to execute.</param>
    /// <returns>A ValueTask to asynchronously track the completion of the operation.</returns>
    public static async ValueTask ExecuteAsync(this IDispatcher dispatcher, Action action)
        => await dispatcher.ExecuteAsync(
            async _ =>
            {
                action();
                return true;
            },
            CancellationToken.None);


    /// <summary>
    /// Asynchronously executes an operation on the UI thread.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to use to execute the operation.</param>
    /// <param name="action">The async operation to execute.</param>
    /// <param name="token">An cancellation token to cancel the async operation.</param>
    /// <returns>A ValueTask to asynchronously track the completion of the operation.</returns>
    public static async ValueTask ExecuteAsync(this IDispatcher dispatcher, Action<CancellationToken> action, CancellationToken token)
        => await dispatcher.ExecuteAsync(
            async ct =>
            {
                action(ct);
                return true;
            },
            token);
}
