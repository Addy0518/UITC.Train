using System.ComponentModel;
using System.Reflection;

namespace Lab.API.TODO.Common.Extensions;

public static class EnumExtension
{
    public static string GetDescription(this Enum enumValue)
    {
        return enumValue
            .GetType()
            .GetMember(enumValue.ToString())
            .First()
            .GetCustomAttribute<DescriptionAttribute>()
            .Description;
    }
}
