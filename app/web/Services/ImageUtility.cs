using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Boilerplate.AspNetCore;
using LangBot.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LangBot.Web.Services
{
    public class ImageUtility
    {
        private readonly IOptions<LangOptions> _options;
        private readonly Serializer _serializer;
        private readonly ConfigService _configService;
        private readonly IUrlHelper _urlHelper;
        private readonly TextSplitter _textSplitter;

        public ImageUtility(IOptions<LangOptions> options, Serializer serializer, ConfigService configService, IUrlHelper urlHelper, TextSplitter textSplitter)
        {
            _options = options;
            _serializer = serializer;
            _configService = configService;
            _urlHelper = urlHelper;
            _textSplitter = textSplitter;
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

        public async Task<string> GetImageUrl(string message, TemplateConfig.Template template)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (template == null) throw new ArgumentNullException(nameof(template));

            var config = await _configService.GetConfig();
            var boxes = template.Boxes ?? config.TemplateDefaults.Boxes;
            var textLines = _textSplitter.SplitText(message.ToUpper(), boxes.Count);

            var imageModel = new ImageModel
            {
                ImageId = template.Id,
                Boxes = boxes.SelectWithIndex((box, i) => new TextBox
                {
                    Text = textLines[i],
                    X = box.X,
                    Y = box.Y,
                    Width = box.Width,
                    Height = box.Height,
                    Vertical = box.Vertical,
                    Horizontal = box.Horizontal,
                    LineColor = box.LineColor,
                    FillColor = box.FillColor,
                }).ToList()
            };

            var imageRequest = CreateRequest(imageModel);
            var imageUrl = _urlHelper.AbsoluteAction("Get", "Image", imageRequest);
            return imageUrl;
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
