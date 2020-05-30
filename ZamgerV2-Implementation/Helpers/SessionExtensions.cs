using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace ZamgerV2_Implementation.Helpers
{
    public static class SessionExtensions
    {
        public static void SetJson(this ISession session, string key, object value) => session.SetString(key, JsonConvert.SerializeObject(value));
        public static T GetJson<T>(this ISession session, string key) => session.GetString(key) == null ? default(T) : JsonConvert.DeserializeObject<T>(session.GetString(key));
    }
}
