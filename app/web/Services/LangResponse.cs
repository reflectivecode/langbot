using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Boilerplate.AspNetCore;
using LangBot.Web.Enums;
using LangBot.Web.Models;
using LangBot.Web.Slack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LangBot.Web.Services
{
    public class LangResponse
    {
        private readonly IOptions<LangOptions> _options;
        private readonly IUrlHelper _urlHelper;
        private readonly ConfigService _templateService;
        private readonly ImageUtility _imageUtility;
        private readonly Serializer _serializer;
        private readonly DatabaseRepo _repo;
        private readonly TextSplitter _textSplitter;

        private static string GetText(MemeMessage message) => (message.IsAnonymous ? "_anonymous_" : $"<@{message.UserId}>") + $" used `{Constants.Commands.Lang}`";

        public LangResponse(IUrlHelper urlHelper, IOptions<LangOptions> options, ConfigService templateService, ImageUtility imageUtility, Serializer serializer, DatabaseRepo repo, TextSplitter textSplitter)
        {
            _urlHelper = urlHelper;
            _options = options;
            _templateService = templateService;
            _imageUtility = imageUtility;
            _serializer = serializer;
            _repo = repo;
            _textSplitter = textSplitter;
        }

        public async Task<Message> CreateMessage(
            string teamId,
            string teamDomain,
            string channelId,
            string channelName,
            string userId,
            string userName,
            string message)
        {
            if (teamId == null) throw new ArgumentNullException(nameof(teamId));
            if (teamDomain == null) throw new ArgumentNullException(nameof(teamDomain));
            if (channelId == null) throw new ArgumentNullException(nameof(channelId));
            if (channelName == null) throw new ArgumentNullException(nameof(channelName));
            if (userId == null) throw new ArgumentNullException(nameof(userId));
            if (userName == null) throw new ArgumentNullException(nameof(userName));
            if (message == null) throw new ArgumentNullException(nameof(message));

            var channelType = "unknown"; //TODO

            var templates = await _templateService.GetTemplatesForUser(userId);
            var template = templates.FirstOrDefault(x => x.Default == true) ?? templates.First();
            var imageUrl = await _imageUtility.GetImageUrl(message, template);

            var memeMessage = await _repo.InsertMessage(
                teamId: teamId,
                teamDomain: teamDomain,
                channelId: channelId,
                channelName: channelName,
                channelType: channelType,
                userId: userId,
                userName: userName,
                templateId: template.Id,
                message: message,
                imageUrl: imageUrl,
                isAnonymous: false);

            return await RenderPreview(memeMessage);
        }

        public async Task<Message> ChangeTemplate(Guid guid, bool isAnonymous)
        {
            var message = await _repo.UpdatePreview(
                guid: guid,
                isAnonymous: isAnonymous);

            if (message == null || message.PublishDate.HasValue || message.DeleteDate.HasValue) return await RenderDelete();

            return await RenderPreview(message);
        }

        public async Task<Message> ToggleUpVote(Guid guid, string userId, string userName)
        {
            if (userId == null) throw new ArgumentNullException(nameof(userId));
            if (userName == null) throw new ArgumentNullException(nameof(userName));

            var alreadyUpvoted = await _repo.HasReacted(guid, Constants.Reactions.UpVote, userId);
            if (alreadyUpvoted)
            {
                var message = await _repo.RemoveReaction(guid, Constants.Reactions.UpVote, userId);
                return await RenderPublished(message);
            }
            else
            {
                var message = await _repo.AddReaction(guid, Constants.Reactions.UpVote, userId, userName, null);
                return await RenderPublished(message);
            }
        }

        public async Task<Message> RenderPreview(MemeMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var config = await _templateService.GetTemplates();
            var isPrivilegedUser = config.Privileged.Contains(message.UserId);
            var templates = config.Templates.Where(t => isPrivilegedUser || !t.Privileged).ToList();
            var template = GetTemplate(templates, message.TemplateId);

            return new Message
            {
                ResponseType = MessageResponseTypes.Ephemeral,
                Text = GetText(message),
                Attachments = new[]
                {
                    new MessageAttachment
                    {
                        ImageUrl = message.ImageUrl,
                        Fallback = message.Message,
                    },
                    new MessageAttachment
                    {
                        Title = "This is a preview of your meme",
                        Text = "_hint: use a semicolon to separate lines of text_",
                        Fallback = "Here you would choose to confirm posting your meme",
                        CallbackId = Constants.CallbackIds.Meme,
                        Color = "#3AA3E3",
                        MrkdwnIn = new[] { "text" },
                        Actions = new IMessageAction []
                        {
                            new MessageButton
                            {
                                Name = $"{Constants.ActionNames.Cancel}:{message.Guid}",
                                Text = "Cancel",
                            },
                            new MessageSelect
                            {
                                Name = $"{Constants.ActionNames.Switch}:{message.Guid}",
                                Text = "Image",
                                SelectedOptions = new[]
                                {
                                     new MessageOption
                                    {
                                        Text = template.Name,
                                        Value = template.Id,
                                    }
                                },
                                OptionGroups = new[]
                                {
                                    new MessageOptionGroup
                                    {
                                        Text = "Change Image",
                                        Options = templates.Select(t => new MessageOption
                                        {
                                            Text = t.Name,
                                            Description = t == template ? "(selected)" : null,
                                            Value = t.Id,
                                        }).ToList()
                                    },
                                    new MessageOptionGroup
                                    {
                                        Text = "Change Anonymity",
                                        Options = new[]
                                        {
                                            new MessageOption
                                            {
                                                Text = "Include username",
                                                Description = message.IsAnonymous ? null : "(selected)",
                                                Value = "false",
                                            },
                                            new MessageOption
                                            {
                                                Text = "Post anonymously",
                                                Description = message.IsAnonymous ? "(selected)" : null,
                                                Value = "true",
                                            },
                                        }
                                    },
                                }
                            },
                            new MessageButton
                            {
                                Name = $"${Constants.ActionNames.Switch}:{message.Guid}",
                                Text = "Next",
                                Style = MessageButtonStyles.Default,
                                Value = templates.GetItemAfter(template).Id,
                            },
                            new MessageButton
                            {
                                Name = $"${Constants.ActionNames.Edit}:{message.Guid}",
                                Text = "Edit",
                                Style = MessageButtonStyles.Default,
                            },
                            new MessageButton
                            {
                                Name = $"{Constants.ActionNames.Submit}:{message.Guid}",
                                Text = "Post",
                                Style = MessageButtonStyles.Primary
                            },
                        }
                    }
                }
            };
        }

        private string GetImageUrl(string message, TemplateConfig config, TemplateConfig.Template template)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (template == null) throw new ArgumentNullException(nameof(template));

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

            var imageRequest = _imageUtility.CreateRequest(imageModel);
            var imageUrl = _urlHelper.AbsoluteAction("Get", "Image", imageRequest);
            return imageUrl;
        }

        private TemplateConfig.Template GetTemplate(IList<TemplateConfig.Template> templates, string id)
        {
            if (String.IsNullOrEmpty(id)) return templates.FirstOrDefault(x => x.Default == true) ?? templates.First();
            var template = templates.FirstOrDefault(t => t.Id == id);
            if (template == null) throw new SlackException($"Template not found: {id}");
            return template;
        }

        public Task<Message> RenderPublished(MemeMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            return Task.FromResult(new Message
            {
                ResponseType = MessageResponseTypes.InChannel,
                Text = GetText(message),
                Attachments = new List<MessageAttachment>()
                {
                    new MessageAttachment
                    {
                        Fallback = message.Message,
                        ImageUrl = message.ImageUrl,
                    },
                    new MessageAttachment
                    {
                        CallbackId = Constants.CallbackIds.Meme,
                        Actions = new IMessageAction []
                        {
                            new MessageButton
                            {
                                Name = $"{Constants.ActionNames.UpVote}:{message.Guid}",
                                Text = ":+1: Like" + (message.UpVoteCount > 0 ? message.UpVoteCount.ToString("(#)") : ""),
                                Style = MessageButtonStyles.Primary,
                            },
                            new MessageButton
                            {
                                Name = $"{Constants.ActionNames.Flag}:{message.Guid}",
                                Text = "Flag",
                                Style = MessageButtonStyles.Danger,
                            },
                        }
                    }
                },
            });
        }

        public Task<Message> RenderDelete()
        {
            return Task.FromResult(new Message
            {
                DeleteOriginal = true,
            });
        }
    }
}
