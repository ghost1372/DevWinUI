namespace DevWinUI;

public static partial class Growl
{
    private sealed partial class Entry
    {
        public Entry(GrowlHost host, string key)
        {
            Host = new WeakReference<GrowlHost>(host);
            Key = key;
        }

        public WeakReference<GrowlHost> Host { get; }

        public string Key { get; set; }

        public GrowlHost? Target => Host.TryGetTarget(out var host) ? host : null;
    }

    private static readonly object Gate = new();
    private static readonly List<Entry> Entries = new();
    private static WeakReference<GrowlHost>? _default;

    public static GrowlHost? DefaultHost
    {
        get
        {
            lock (Gate)
            {
                Prune();

                if (_default is not null && _default.TryGetTarget(out var host))
                {
                    return host;
                }

                return Entries.Count > 0 ? Entries[0].Target : null;
            }
        }
    }

    public static void Register(GrowlHost host, string? key = null, bool isDefault = false)
    {
        ArgumentNullException.ThrowIfNull(host);

        if (key is not null)
        {
            host.HostKey = key;
        }

        var hostKey = host.HostKey ?? string.Empty;

        lock (Gate)
        {
            Prune();

            if (hostKey.Length > 0)
            {
                Entries.RemoveAll(e => e.Target != host && e.Key == hostKey);
            }

            var entry = Find(host);

            if (entry is null)
            {
                Entries.Add(new Entry(host, hostKey));
            }
            else
            {
                entry.Key = hostKey;
            }

            if (isDefault)
            {
                _default = new WeakReference<GrowlHost>(host);
            }

            ClearStaleDefault();
        }
    }

    public static void Unregister(GrowlHost host)
    {
        lock (Gate)
        {
            Entries.RemoveAll(e => e.Target is null || e.Target == host);
            ClearStaleDefault();
        }
    }

    public static void Unregister(string key)
    {
        var host = GetHost(key);

        if (host is not null)
        {
            Unregister(host);
        }
    }

    public static void SetDefault(GrowlHost host)
    {
        Register(host, null, true);
    }

    public static GrowlHost? GetHost(string? key = null)
    {
        if (key is null)
        {
            return DefaultHost;
        }

        lock (Gate)
        {
            Prune();

            return Entries.Find(e => e.Key == key)?.Target;
        }
    }

    public static GrowlHost? GetHost(XamlRoot xamlRoot)
    {
        foreach (var host in Snapshot())
        {
            if (host.XamlRoot == xamlRoot)
            {
                return host;
            }
        }

        return null;
    }

    public static GrowlHandle Show(GrowlOptions options, string? hostKey = null)
    {
        ArgumentNullException.ThrowIfNull(options);

        return options.Scope == GrowlScope.Global
            ? GrowlGlobal.Show(options)
            : Resolve(hostKey).Show(options);
    }

    public static Task<GrowlHandle> ShowAsync(GrowlOptions options, string? hostKey = null)
    {
        ArgumentNullException.ThrowIfNull(options);

        var queue = options.Scope == GrowlScope.Global ? GrowlGlobal.DispatcherQueue : Resolve(hostKey).DispatcherQueue;

        if (queue is null)
        {
            return Task.FromException<GrowlHandle>(new InvalidOperationException("GrowlGlobal.DispatcherQueue must be set on the UI thread before using ShowAsync with global scope."));
        }

        if (queue.HasThreadAccess)
        {
            return Task.FromResult(Show(options, hostKey));
        }

        var source = new TaskCompletionSource<GrowlHandle>(TaskCreationOptions.RunContinuationsAsynchronously);

        bool queued = queue.TryEnqueue(() =>
        {
            try
            {
                source.SetResult(Show(options, hostKey));
            }
            catch (Exception ex)
            {
                source.SetException(ex);
            }
        });

        if (!queued)
        {
            source.SetException(new InvalidOperationException("The dispatcher queue is shut down."));
        }

        return source.Task;
    }

    internal static void UpdateKey(GrowlHost host, string key)
    {
        lock (Gate)
        {
            var entry = Find(host);

            if (entry is null)
            {
                return;
            }

            entry.Key = key;

            if (key.Length > 0)
            {
                Entries.RemoveAll(e => e != entry && e.Key == key);
            }

            ClearStaleDefault();
        }
    }

    private static Entry? Find(GrowlHost host)
    {
        return Entries.Find(e => e.Target == host);
    }

    private static void Prune()
    {
        Entries.RemoveAll(e => e.Target is null);
        ClearStaleDefault();
    }

    private static void ClearStaleDefault()
    {
        if (_default is not null && (!_default.TryGetTarget(out var host) || Find(host) is null))
        {
            _default = null;
        }
    }

    private static List<GrowlHost> Snapshot()
    {
        lock (Gate)
        {
            Prune();

            return Entries.Select(e => e.Target).OfType<GrowlHost>().ToList();
        }
    }

    public static void ClearAll(string? hostKey = null)
    {
        GetHost(hostKey)?.ClearAll();
    }

    public static void ClearAllHosts()
    {
        GrowlGlobal.ClearAll();

        foreach (var host in Snapshot())
        {
            host.ClearAll();
        }
    }

    private static GrowlHost Resolve(string? key)
    {
        return GetHost(key) ?? throw new InvalidOperationException(
            key is null
                ? "No GrowlHost is registered."
                : $"No GrowlHost is registered with key '{key}'.");
    }
}
