using System.Diagnostics.CodeAnalysis;

namespace DevWinUI;
internal static partial class ArgumentNullExceptionExtensions
{
    [DoesNotReturn]
    public static void Throw(this ArgumentNullException? _, string? parameterName)
    {
        throw new ArgumentNullException(parameterName);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowIfNull(this ArgumentNullException? _, [NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
    {
        if (argument is null)
        {
            Throw(parameterName);
        }
    }

    [DoesNotReturn]
    private static void Throw(string? parameterName)
    {
        throw new ArgumentNullException(parameterName);
    }
}
