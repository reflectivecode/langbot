using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LangBot.Web.Slack;

namespace LangBot.Web.Services
{
    public class LangResponse
    {
        private readonly ConfigService _configService;

        private static string GetText(MemeMessage message) => (message.IsAnonymous ? "_anonymous_" : $"<@{message.UserId}>") + $" used `{Constants.Commands.Lang}`";

        public LangResponse(ConfigService configService)
        {
            _configService = configService;
        }

        public async Task<SlackMessage> RenderPreview(MemeMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var template = await _configService.GetTemplate(message.TemplateId, message.UserId);
            var templates = await _configService.GetTemplatesForUser(message.UserId);

            return new SlackMessage
            {
                ResponseType = SlackMessageResponseTypes.Ephemeral,
                Text = GetText(message),
                Attachments = new[]
                {
                    new SlackMessageAttachment
                    {
                        ImageUrl = message.ImageUrl,
                        Fallback = message.Message,
                    },
                    new SlackMessageAttachment
                    {
                        Title = "This is a preview of your meme",
                        Text = "_hint: use a semicolon to separate lines of text_",
                        Fallback = "Here you would choose to confirm posting your meme",
                        CallbackId = $"{Constants.CallbackIds.Meme}:{message.Guid}",
                        Color = "#3AA3E3",
                        MrkdwnIn = new[] { "text" },
                        Actions = new ISlackMessageAction []
                        {
                            new SlackMessageButton
                            {
                                Name = Constants.ActionNames.Cancel,
                                Text = "Cancel",
                            },
                            new SlackMessageSelect
                            {
                                Name = Constants.ActionNames.Switch,
                                Text = "Image",
                                SelectedOptions = new[]
                                {
                                     new SlackMessageOption
                                    {
                                        Text = template.Name,
                                        Value = template.Id,
                                    }
                                },
                                OptionGroups = new[]
                                {
                                    new SlackMessageOptionGroup
                                    {
                                        Text = "Change Image",
                                        Options = templates.Select(t => new SlackMessageOption
                                        {
                                            Text = t.Name,
                                            Description = t == template ? "(selected)" : null,
                                            Value = t.Id,
                                        }).ToList()
                                    },
                                    new SlackMessageOptionGroup
                                    {
                                        Text = "Change Anonymity",
                                        Options = new[]
                                        {
                                            new SlackMessageOption
                                            {
                                                Text = "Include username",
                                                Description = message.IsAnonymous ? null : "(selected)",
                                                Value = "false",
                                            },
                                            new SlackMessageOption
                                            {
                                                Text = "Post anonymously",
                                                Description = message.IsAnonymous ? "(selected)" : null,
                                                Value = "true",
                                            },
                                        }
                                    },
                                }
                            },
                            new SlackMessageButton
                            {
                                Name = Constants.ActionNames.Switch,
                                Text = "Next",
                                Style = SlackMessageButtonStyles.Default,
                                Value = templates.GetItemAfter(template).Id,
                            },
                            new SlackMessageButton
                            {
                                Name = Constants.ActionNames.Edit,
                                Text = "Edit",
                                Style = SlackMessageButtonStyles.Default,
                            },
                            new SlackMessageButton
                            {
                                Name = Constants.ActionNames.Submit,
                                Text = "Post",
                                Style = SlackMessageButtonStyles.Primary
                            },
                        }
                    }
                }
            };
        }

        public Task<SlackMessage> RenderPublished(MemeMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            return Task.FromResult(new SlackMessage
            {
                ResponseType = SlackMessageResponseTypes.InChannel,
                Text = GetText(message),
                Attachments = new List<SlackMessageAttachment>()
                {
                    new SlackMessageAttachment
                    {
                        Fallback = message.Message,
                        ImageUrl = message.ImageUrl,
                    },
                    new SlackMessageAttachment
                    {
                        CallbackId = $"{Constants.CallbackIds.Meme}:{message.Guid}",
                        Actions = new ISlackMessageAction []
                        {
                            new SlackMessageButton
                            {
                                Name = Constants.ActionNames.UpVote,
                                Text = ":+1: Like" + (message.UpVoteCount > 0 ? message.UpVoteCount.ToString("(#)") : ""),
                                Style = SlackMessageButtonStyles.Primary,
                            },
                            new SlackMessageButton
                            {
                                Name = Constants.ActionNames.Flag,
                                Text = "Flag",
                                Style = SlackMessageButtonStyles.Danger,
                            },
                        }
                    }
                },
            });
        }

        public Task<SlackMessage> RenderDelete()
        {
            return Task.FromResult(new SlackMessage
            {
                DeleteOriginal = true,
            });
        }
    }
}
