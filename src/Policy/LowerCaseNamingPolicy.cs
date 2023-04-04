using System.Text.Json;

namespace MinimalAPIsDemo.Policy
{
    public class LowerCaseNamingPolicy : JsonNamingPolicy
    {
        public static JsonNamingPolicy LowerCase { get; } = new LowerCaseNamingPolicy();
        public override string ConvertName(string name) =>
            name.ToLower();
    }
}
