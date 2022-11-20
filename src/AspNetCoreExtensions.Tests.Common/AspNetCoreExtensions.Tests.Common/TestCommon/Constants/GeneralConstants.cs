using System.Text.Json;

namespace TechBuddy.Extensions.Tests.Common.TestCommon.Constants;
public sealed class GeneralConstants
{
    public static JsonSerializerOptions JsonOptions { get; } = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };
}
