using System;
using System.Linq;
using System.Threading.Tasks;
using LangBot.Web.Services;
using LangBot.Web.Slack;

namespace LangBot.Web
{
    public class LangCommandResponder : ISlackCommandResponder
    {
        private readonly LangResponse _langResponse;
        private readonly DatabaseRepo _databaseRepo;
        private readonly ConfigService _configService;
        private readonly ImageUtility _imageUtility;

        public LangCommandResponder(LangResponse langResponse, DatabaseRepo databaseRepo, ConfigService configService, ImageUtility imageUtility)
        {
            _langResponse = langResponse;
            _databaseRepo = databaseRepo;
            _configService = configService;
            _imageUtility = imageUtility;
        }

        public async Task<SlackMessage> Respond(SlackCommandRequest command)
        {
            if (command.Command != Constants.Commands.Lang) return null;

            var channelType = GetChannelType(command.ChannelId);

            var templates = await _configService.GetTemplatesForUser(command.UserId);
            var template = templates.FirstOrDefault(x => x.Default == true) ?? templates.First();
            var imageUrl = await _imageUtility.GetImageUrl(command.Text, template);

            var message = await _databaseRepo.InsertMessage(
                teamId: command.TeamId,
                teamDomain: command.TeamDomain,
                channelId: command.ChannelId,
                channelName: command.ChannelName,
                channelType: channelType,
                userId: command.UserId,
                userName: command.UserName,
                templateId: template.Id,
                message: command.Text,
                imageUrl: imageUrl,
                isAnonymous: false);

            return await _langResponse.RenderPreview(message);
        }

        private static ChannelType GetChannelType(string channelId)
        {
            if (channelId == null) throw new ArgumentNullException(nameof(channelId));

            switch (channelId[0])
            {
                case 'C': return ChannelType.Public;
                case 'G': return ChannelType.Private;
                case 'D': return ChannelType.Direct;
                default: return ChannelType.Unknown;
            }
        }
    }
}
