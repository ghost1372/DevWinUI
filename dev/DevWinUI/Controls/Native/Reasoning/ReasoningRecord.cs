namespace DevWinUI;

public sealed record ReasoningSource(string Title, Uri Url)
{
    public string Host => Url.Host;
}

public sealed record ReasoningRow(string Label, ReasoningStepStatus Status = ReasoningStepStatus.Done, Uri? Url = null, int Added = 0, int Removed = 0);
