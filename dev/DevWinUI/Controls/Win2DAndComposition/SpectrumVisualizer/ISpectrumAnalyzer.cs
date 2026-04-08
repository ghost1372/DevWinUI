namespace DevWinUI;

public interface ISpectrumAnalyzer : IDisposable
{
    event Action<float[]> SpectrumDataUpdated;

    void Start();
    void Stop();
}

