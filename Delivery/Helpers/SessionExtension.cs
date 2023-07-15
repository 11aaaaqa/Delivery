using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Delivery.Helpers
{
    public class SessionExtension
    {
        public static void Set<T>(ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
