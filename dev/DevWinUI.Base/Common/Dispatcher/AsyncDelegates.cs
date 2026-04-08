namespace DevWinUI;

public delegate ValueTask AsyncAction(CancellationToken ct);

public delegate ValueTask<TResult> AsyncFunc<TResult>(CancellationToken ct);
