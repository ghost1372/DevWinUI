namespace DevWinUI;
public interface IDispatcher
{
    /// <summary>
    ///  Gets a value that specifies whether the current execution context is on the UI thread.
    /// </summary>
    bool HasThreadAccess { get; }

    /// <summary>
    /// Adds a task to the queue which will be executed on the thread associated with the dispatcher.
    /// </summary>
    /// <remarks>This is the raw version which allows to interact with the native dispatcher the fewest overhead possible.</remarks>
    /// <param name="action">The task to execute.</param>
    /// <returns>True indicates that the task was added to the queue; false, otherwise.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)] // Applications should prefer to use the ExecuteAsync which allow to track the execution
    bool TryEnqueue(Action action);

    /// <summary>
    /// Asynchronously executes an operation on the UI thread.
    /// </summary>
    /// <typeparam name="TResult">Type of the result of the operation.</typeparam>
    /// <param name="action">The async operation to execute.</param>
    /// <param name="cancellation">An cancellation token to cancel the async operation.</param>
    /// <returns>A ValueTask to asynchronously get the result of the operation.</returns>
    ValueTask<TResult> ExecuteAsync<TResult>(AsyncFunc<TResult> action, CancellationToken cancellation);
}
