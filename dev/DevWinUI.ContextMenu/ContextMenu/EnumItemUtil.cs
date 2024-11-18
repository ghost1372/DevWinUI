using System.Collections.Immutable;

namespace DevWinUI;
public static partial class EnumItemUtil
{
    public static IReadOnlyList<EnumItem> GetEnumItems<T>() where T : struct, Enum
    {
        // No need to check if T is an enum because the generic constraint ensures it's an Enum
        return Enum.GetValues<T>()
            .Select(x => new EnumItem
            {
                Label = x.GetType().GetField(x.ToString()).GetCustomAttributes(false).OfType<DescriptionAttribute>().FirstOrDefault()?.Description ?? x.ToString(),
                Value = Convert.ToInt32(x)
            })
            .ToImmutableList();
    }

}
