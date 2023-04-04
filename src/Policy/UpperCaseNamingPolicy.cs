using System.Text.Json;

namespace MinimalAPIsDemo.Policy
{
    public class UpperCaseNamingPolicy : JsonNamingPolicy
    {
        public static JsonNamingPolicy UpperCase { get; } = new UpperCaseNamingPolicy();
        public override string ConvertName(string name) =>
            name.ToUpper();
    }
}
