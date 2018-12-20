using System;
using System.Drawing;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace LangBot.Web
{
    public class YamlColorConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type) => type == typeof(Color);

        public object ReadYaml(IParser parser, Type type)
        {
            var color = parser.Expect<Scalar>();
            var converter = new System.Drawing.ColorConverter();
            return (Color)converter.ConvertFromInvariantString(color.Value);
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            var color = (Color)value;
            var converter = new System.Drawing.ColorConverter();
            var str = converter.ConvertToInvariantString(color);
            emitter.Emit(new Scalar(str));
        }
    }
}
