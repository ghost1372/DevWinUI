namespace DevWinUI;
public static partial class DispatcherExtensions
{
    extension(IDispatcher dispatcher)
    {
        /// <summary>
        /// Asynchronously executes an operation on the UI thread.
        /// </summary>
        /// <param name="dispatcher">The dispatcher to use to execute the operation.</param>
        /// <param name="action">The async operation to execute.</param>
        /// <param name="token">An cancellation token to cancel the async operation.</param>
        /// <returns>A ValueTask to asynchronously track the completion of the operation.</returns>
        public async ValueTask ExecuteAsync(AsyncAction action, CancellationToken token)
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
        public async ValueTask ExecuteAsync(AsyncAction action)
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
        public ValueTask<TResult> ExecuteAsync<TResult>(AsyncFunc<TResult> func)
            => dispatcher.ExecuteAsync(
                async ct => await func(ct),
                CancellationToken.None);

        /// <summary>
        /// Asynchronously executes an operation on the UI thread.
        /// </summary>
        /// <param name="dispatcher">The dispatcher to use to execute the operation.</param>
        /// <param name="action">The async operation to execute.</param>
        /// <returns>A ValueTask to asynchronously track the completion of the operation.</returns>
        public async ValueTask ExecuteAsync(Action action)
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
        public async ValueTask ExecuteAsync(Action<CancellationToken> action, CancellationToken token)
            => await dispatcher.ExecuteAsync(
                async ct =>
                {
                    action(ct);
                    return true;
                },
                token);
    }
}
