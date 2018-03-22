using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LangBot.Web.Models;
using LangBot.Web.Slack;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LangBot.Web.Services
{
    public class ConfigService
    {
        private readonly IOptions<LangOptions> _options;
        private readonly IHostingEnvironment _env;
        private readonly IMemoryCache _memoryCache;

        public ConfigService(IOptions<LangOptions> options, IHostingEnvironment env, IMemoryCache memoryCache)
        {
            _options = options;
            _env = env;
            _memoryCache = memoryCache;
        }

        public async Task<TemplateConfig> GetConfig()
        {
            return await _memoryCache.GetOrCreateAsync(nameof(ConfigService), async entry =>
            {
                var yaml = await File.ReadAllTextAsync(_options.Value.TemplateConfig);
                using (var reader = new StringReader(yaml))
                {
                    var deserializer = new DeserializerBuilder()
                        .WithNamingConvention(new CamelCaseNamingConvention())
                        .WithTypeConverter(new YamlColorConverter())
                        .Build();

                    return deserializer.Deserialize<TemplateConfig>(reader);
                }
            });
        }

        public async Task<IList<TemplateConfig.Template>> GetTemplatesForUser(string userId)
        {
            if (userId == null) throw new ArgumentNullException(nameof(userId));

            var config = await GetConfig();
            var isPrivilegedUser = config.Privileged.Contains(userId);
            return config.Templates.Where(t => isPrivilegedUser || !t.Privileged).ToList();
        }

        public async Task<TemplateConfig.Template> GetTemplate(string templateId)
        {
            if (templateId == null) throw new ArgumentNullException(nameof(templateId));

            var config = await GetConfig();
            return config.Templates.FirstOrDefault(x => x.Id == templateId) ?? throw new SlackException($"Template id not found: {templateId}");
        }

        public async Task<TemplateConfig.Template> GetTemplate(string templateId, string userId)
        {
            if (templateId == null) throw new ArgumentNullException(nameof(templateId));
            if (userId == null) throw new ArgumentNullException(nameof(userId));

            var templates = await GetTemplatesForUser(userId);
            return templates.FirstOrDefault(x => x.Id == templateId) ?? throw new SlackException($"Template id not found: {templateId}");
        }

        public string GetTemplatePath(TemplateConfig.Template template)
        {
            if (template == null) throw new ArgumentNullException(nameof(template));

            return Path.Combine(_env.ContentRootPath, _options.Value.ImageDirectory, template.File);
        }

        public string GetFontPath()
        {
            return Path.Combine(_env.ContentRootPath, _options.Value.FontPath);
        }
    }
}
