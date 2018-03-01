using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Boilerplate.AspNetCore;
using LangBot.Web.Enums;
using LangBot.Web.Models;
using LangBot.Web.Slack;
using LangBot.Web.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LangBot.Web.Services
{
    public class LangResponse
    {
        private readonly IOptions<LangOptions> _options;
        private readonly IUrlHelper _urlHelper;
        private readonly TemplateService _templateService;
        private readonly ImageUtility _imageUtility;
        private readonly Serializer _serializer;

        public LangResponse(IUrlHelper urlHelper, IOptions<LangOptions> options, TemplateService templateService, ImageUtility imageUtility, Serializer serializer)
        {
            _urlHelper = urlHelper;
            _options = options;
            _templateService = templateService;
            _imageUtility = imageUtility;
            _serializer = serializer;
        }

        public async Task<Message> Preview(PreviewModel model)
        {
            var config = await _templateService.GetTemplates();
            var isPrivilegedUser = config.Privileged.Contains(model.UserId);
            var templates = config.Templates.Where(t => isPrivilegedUser || !t.Privileged).ToList();
            var template = GetTemplate(templates, model.TemplateId);
            var boxes = template.Boxes ?? config.TemplateDefaults.Boxes;
            var textLines = SplitText(model.Text.ToUpper(), boxes.Count);

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

            return new Message
            {
                ResponseType = MessageResponseTypes.Ephemeral,
                Text = model.Anonymous ? null : $"<@{model.UserId}> used `{Constants.Commands.Lang}`",
                Attachments = new[]
                {
                    new MessageAttachment
                    {
                        ImageUrl = imageUrl,
                        Fallback = model.Text,
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
                                Name = "cancel",
                                Text = "Cancel",
                            },
                            new MessageSelect
                            {
                                Name = "switch",
                                Text = "Image",
                                SelectedOptions = new[]
                                {
                                     new MessageOption
                                    {
                                        Text = template.Name,
                                        Value = _serializer.ObjectToBase64Url(new PreviewModel
                                        {
                                            TemplateId = template.Id,
                                            Text = model.Text,
                                            UserId = model.UserId,
                                            Anonymous = model.Anonymous,
                                        })
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
                                            Value = _serializer.ObjectToBase64Url(new PreviewModel
                                            {
                                                TemplateId = t.Id,
                                                Text = model.Text,
                                                UserId = model.UserId,
                                                Anonymous = model.Anonymous,
                                            })
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
                                                Description = model.Anonymous ? null : "(selected)",
                                                Value = _serializer.ObjectToBase64Url(new PreviewModel
                                                {
                                                    TemplateId = template.Id,
                                                    Text = model.Text,
                                                    UserId = model.UserId,
                                                    Anonymous = false,
                                                }),
                                            },
                                            new MessageOption
                                            {
                                                Text = "Post anonymously",
                                                Description = model.Anonymous ? "(selected)" : null,
                                                Value = _serializer.ObjectToBase64Url(new PreviewModel
                                                {
                                                    TemplateId = template.Id,
                                                    Text = model.Text,
                                                    UserId = model.UserId,
                                                    Anonymous = true,
                                                }),
                                            },
                                        }
                                    },
                                }
                            },
                            new MessageButton
                            {
                                Name = "switch",
                                Text = "Next",
                                Style = MessageButtonStyles.Default,
                                Value = _serializer.ObjectToBase64Url(new PreviewModel
                                {
                                    TemplateId = templates.GetItemAfter(template).Id,
                                    Text = model.Text,
                                    UserId = model.UserId,
                                    Anonymous = model.Anonymous,
                                })
                            },
                            new MessageButton
                            {
                                Name = "submit",
                                Text = "Post",
                                Style = MessageButtonStyles.Primary,
                                Value = _serializer.ObjectToBase64Url(new SubmitModel
                                {
                                    ImageUrl = imageUrl,
                                    Fallback = model.Text,
                                    UserId = model.Anonymous ?  null : model.UserId,
                                }),
                            },
                        }
                    }
                }
            };
        }

        private TemplateConfig.Template GetTemplate(IList<TemplateConfig.Template> templates, string id)
        {
            if (String.IsNullOrEmpty(id)) return templates.FirstOrDefault(x => x.Default == true) ?? templates.First();
            var template = templates.FirstOrDefault(t => t.Id == id);
            if (template == null) throw new SlackException($"Template not found: {id}");
            return template;
        }

        public Task<Message> Submit(SubmitModel model)
        {
            return Task.FromResult(new Message
            {
                DeleteOriginal = true,
                ResponseType = MessageResponseTypes.InChannel,
                Text = model.UserId == null ? null : $"<@{model.UserId}> used `{Constants.Commands.Lang}`",
                Attachments = new List<MessageAttachment>()
                {
                    new MessageAttachment
                    {
                        Fallback = model.Fallback,
                        ImageUrl = model.ImageUrl,
                    },
                },
            });
        }

        public Task<Message> Cancel()
        {
            return Task.FromResult(new Message
            {
                DeleteOriginal = true,
            });
        }

        private static readonly Regex _whitespace = new Regex(@"\s+", RegexOptions.Compiled);

        private IList<string> SplitText(string text, int count)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            text = _whitespace.Replace(text, " ").Trim();

            if (count == 1)
                return new[] { text };

            if (text.Contains(";"))
                return text.Split(';', count).TrimAll().PadToLength(count, "");

            var words = _whitespace.Split(text);
            if (words.Length <= count) return words.PadToLength(count, "");

            TextLineStack best = null;
            long bestCost = long.MaxValue;
            foreach (var stack in AllSplits(words, count))
            {
                var cost = stack.Cost;
                if (cost < bestCost)
                {
                    best = stack;
                    bestCost = cost;
                }
            }

            return best.GetLines().PadToLength(count, "");
        }

        private IEnumerable<TextLineStack> AllSplits(ArraySegment<string> words, int groupCount)
        {
            if (groupCount == 1)
            {
                yield return new TextLineStack(new TextLine(words));
                yield break;
            }

            for (int take = 1; take <= words.Count - groupCount + 1; take++)
            {
                var line = new TextLine(words.Slice(0, take));
                foreach (var tail in AllSplits(words.Slice(take), groupCount - 1))
                    yield return new TextLineStack(line, tail);
            }
        }

        private class TextLine
        {
            private readonly ArraySegment<string> _segment;

            public int Length { get; }

            public override string ToString() => String.Join(" ", _segment);

            public TextLine(ArraySegment<string> segment)
            {
                _segment = segment;
                Length = segment.Sum(x => x.Length) + segment.Count - 1;
            }

            public long Cost(int longestLine)
            {
                long deficit = longestLine - Length;
                return deficit * deficit;
            }
        }

        private class TextLineStack
        {
            private readonly ImmutableStack<TextLine> _stack;

            public int LongestLine { get; }

            public long Cost { get => _stack.Sum(x => x.Cost(LongestLine)); }
            public IList<string> GetLines() => _stack.Select(x => x.ToString()).ToList();

            public TextLineStack(TextLine line)
            {
                _stack = ImmutableStack.Create(line);
                LongestLine = line.Length;
            }

            public TextLineStack(TextLine line, TextLineStack stack)
            {
                _stack = stack._stack.Push(line);
                LongestLine = Math.Max(line.Length, stack.LongestLine);
            }
        }
    }
}
