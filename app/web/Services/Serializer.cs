using System;
using System.Text;
using Newtonsoft.Json;

namespace LangBot.Web.Services
{
    public class Serializer
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public Serializer(JsonSerializerSettings jsonSerializerSettings)
        {
            _jsonSerializerSettings = jsonSerializerSettings;
        }

        public T JsonToObject<T>(string json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));
            return JsonConvert.DeserializeObject<T>(json, _jsonSerializerSettings);
        }

        public string ObjectToJson(object value)
        {
            return JsonConvert.SerializeObject(value, _jsonSerializerSettings);
        }

        public byte[] Base64UrlToBytes(string base64url)
        {
            var base64 = Base64UrlToBase64(base64url);
            return Convert.FromBase64String(base64);
        }

        public string BytesToBase64Url(byte[] bytes)
        {
            var base64 = Convert.ToBase64String(bytes);
            return Base64ToBase64Url(base64);
        }

        public T Base64ToObject<T>(string base64)
        {
            if (base64 == null) throw new ArgumentNullException(nameof(base64));

            var bytes = Convert.FromBase64String(base64);
            var json = Encoding.UTF8.GetString(bytes);
            return JsonToObject<T>(json);
        }

        public string ObjectToBase64(object value)
        {
            var json = ObjectToJson(value);
            var bytes = Encoding.UTF8.GetBytes(json);
            return Convert.ToBase64String(bytes);
        }

        public T Base64UrlToObject<T>(string base64url)
        {
            if (base64url == null) throw new ArgumentNullException(nameof(base64url));

            var base64 = Base64UrlToBase64(base64url);
            return Base64ToObject<T>(base64);
        }

        public string ObjectToBase64Url(object value)
        {
            var base64 = ObjectToBase64(value);
            return Base64ToBase64Url(base64);
        }

        public string Base64ToBase64Url(string base64)
        {
            if (base64 == null) throw new ArgumentNullException(nameof(base64));

            return base64.TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        public string Base64UrlToBase64(string base64url)
        {
            if (base64url == null) throw new ArgumentNullException(nameof(base64url));

            var modified = base64url.Replace('_', '/').Replace('-', '+');
            switch (modified.Length % 4)
            {
                case 2: return modified += "==";
                case 3: return modified += "=";
                default: return modified;
            }
        }
    }
}
