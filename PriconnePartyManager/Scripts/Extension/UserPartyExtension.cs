using System.Text.Encodings.Web;
using System.Text.Json;
using PriconnePartyManager.Scripts.DataModel;

namespace PriconnePartyManager.Scripts.Extension
{
    public static class UserPartyExtension
    {
        public static string GetJson(this UserParty self)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
            };
            return JsonSerializer.Serialize(self, options);
        }
    }
}