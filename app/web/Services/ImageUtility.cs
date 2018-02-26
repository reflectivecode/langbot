using System;
using System.Security.Cryptography;
using System.Text;
using LangBot.Web.Models;
using Microsoft.Extensions.Options;

namespace LangBot.Web.Services
{
    public class ImageUtility
    {
        private readonly IOptions<LangOptions> _options;
        private static readonly char[] _padding = { '=' };
        private readonly Serializer _serializer;

        public ImageUtility(IOptions<LangOptions> options, Serializer serializer)
        {
            _options = options;
            _serializer = serializer;
        }

        public ImageModel DeserializeImage(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return _serializer.Base64UrlToObject<ImageModel>(value);
        }

        public string SerializeImage(ImageModel image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            return _serializer.ObjectToBase64Url(image);
        }

        public string ImageHash(string image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));

            var hashName = _options.Value.ImageHashName;
            var hashSecret = _options.Value.ImageHashSecret;
            var bytes1 = _serializer.Base64UrlToBytes(hashSecret);
            var bytes2 = _serializer.Base64UrlToBytes(image);
            var bytes = ConcatArrays(bytes1, bytes2);

            using (var algorithm = (HashAlgorithm)CryptoConfig.CreateFromName(hashName))
            {
                var hash = algorithm.ComputeHash(bytes);
                return _serializer.BytesToBase64Url(hash);
            }
        }

        public ImageRequest CreateRequest(ImageModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var image = SerializeImage(model);
            var hash = ImageHash(image);

            return new ImageRequest
            {
                Image = image,
                Hash = hash,
            };
        }

        private T[] ConcatArrays<T>(T[] first, T[] second)
        {
            var result = new T[first.Length + second.Length];
            Array.Copy(first, 0, result, 0, first.Length);
            Array.Copy(second, 0, result, first.Length, second.Length);
            return result;
        }
    }
}
