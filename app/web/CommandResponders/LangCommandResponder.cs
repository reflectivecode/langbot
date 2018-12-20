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
        private readonly Serializer _serializer;

        public LangCommandResponder(LangResponse langResponse, DatabaseRepo databaseRepo, ConfigService configService, ImageUtility imageUtility, Serializer serializer)
        {
            _langResponse = langResponse;
            _databaseRepo = databaseRepo;
            _configService = configService;
            _imageUtility = imageUtility;
            _serializer = serializer;
        }

        public async Task<SlackMessage> Respond(SlackCommandRequest command)
        {
            if (command.Command != Constants.Commands.Lang) return null;

            if (await IsPostCommand(command))
                return RenderPostPreview(command);

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

        private async Task<bool> IsPostCommand(SlackCommandRequest command)
        {
            if (!command.Text.StartsWith('!')) return false;
            var isPrivilegedUser = await _configService.IsPrivilegedUser(command.UserId);
            if (!isPrivilegedUser) return false;
            return true;
        }

        private SlackMessage RenderPostPreview(SlackCommandRequest command)
        {
            var message = command.Text.Substring(1);
            return new SlackMessage
            {
                ResponseType = SlackMessageResponseTypes.Ephemeral,
                Text = message,
                Attachments = new []
                {
                    new SlackMessageAttachment
                    {
                        Fallback = "Approve message here",
                        CallbackId = Constants.CallbackIds.Post,
                        Actions = new[]
                        {
                            new SlackMessageButton
                            {
                                Name = Constants.ActionNames.Cancel,
                                Text = "Cancel",
                            },
                            new SlackMessageButton
                            {
                                Name = Constants.ActionNames.Submit,
                                Text = "Submit",
                                Value = _serializer.ObjectToBase64Url(message)
                            },
                        }
                    }
                }
            };
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
