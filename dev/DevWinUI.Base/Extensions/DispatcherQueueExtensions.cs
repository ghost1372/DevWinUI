using Microsoft.UI.Dispatching;

namespace DevWinUI;
public static partial class DispatcherQueueExtensions
{
    extension(DispatcherQueue dispatcher)
    {
        public async ValueTask<TResult> ExecuteAsync<TResult>(AsyncFunc<TResult> actionWithResult, CancellationToken cancellation)
        {
            var completion = new TaskCompletionSource<TResult>();
            dispatcher.TryEnqueue(async () =>
            {
                try
                {
                    var result = await actionWithResult(cancellation);
                    completion.SetResult(result);
                }
                catch (Exception ex)
                {
                    completion.SetException(ex);
                }
            });
            return await completion.Task;
        }

        public ValueTask<TResult> ExecuteAsync<TResult>(Func<ValueTask<TResult>> actionWithResult)
        {
            return dispatcher.ExecuteAsync(async (CancellationToken t) =>
            {
                return await actionWithResult();
            }, CancellationToken.None);
        }
    }
}
