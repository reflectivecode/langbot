using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LangBot.Web;
using Newtonsoft.Json.Linq;
using Xunit;

namespace LangBot.Tests
{
    public class CommandFixture : BaseFixture
    {
        [Fact]
        public async Task Lang_Returns_Correct_Data()
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "token", "TestToken" },
                { "command", "/lang" },
                { "text", "This is a test" },
                { "user_id", "U2147483697" },
                { "user_name", "test-user" },
                { "channel_id", "C123" },
                { "channel_name", "test-channel" },
                { "team_id", "1" },
                { "team_domain", "example.com" },
            });

            var response = await Client.PostAsync("/api/command", content);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode); Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var messageGuid = (await GetService<DatabaseRepo>().SelectAllMessages()).Last().Guid;

            var actual = JObject.Parse(await response.Content.ReadAsStringAsync());
            var imageUrl = String.Format("http://localhost/api/Image?Image={0}&Hash=1wkdvuo-3EdNW7H0xAibXIVmvY1TLNA2B5EGuQwHPj0", Base64UrlEncode(new
            {
                image_id= "one-does-not-simply",
                boxes = new []
                {
                    new
                    {
                        text = "THIS IS",
                        x = 5.0,
                        y = 2.5,
                        width = 90.0,
                        height = 25.0,
                        vertical = "Top",
                        horizontal = "Center",
                        fill_color = "White",
                        line_color = "Black",
                    },
                    new
                    {
                        text = "A TEST",
                        x = 5.0,
                        y = 72.5,
                        width = 90.0,
                        height = 25.0,
                        vertical = "Bottom",
                        horizontal = "Center",
                        fill_color = "White",
                        line_color = "Black",
                    }
                }
            }));
            var expected = JObject.FromObject(new
            {
                attachments = new object[]
                {
                    new
                    {
                        fallback = "This is a test",
                        image_url = imageUrl,
                    },
                    new
                    {
                        actions = new object[]
                        {
                            new
                            {
                                name = "cancel",
                                style = "default",
                                text = "Cancel",
                                type = "button",
                            },
                            new
                            {
                                name = "switch",
                                option_groups = new []
                                {
                                    new
                                    {
                                        text = "Change Image",
                                        options = new object []
                                        {
                                            new
                                            {
                                                text = "Distracted Boyfriend",
                                                value = "distracted-boyfriend",
                                            },
                                            new
                                            {
                                                text = "One Does Not Simply",
                                                value = "one-does-not-simply",
                                                description = "(selected)"
                                            },
                                        }
                                    },
                                    new
                                    {
                                        text = "Change Anonymity",
                                        options = new object []
                                        {
                                            new
                                            {
                                                text = "Include username",
                                                value = "false",
                                                description = "(selected)"
                                            },
                                            new
                                            {
                                                text = "Post anonymously",
                                                value = "true",
                                            }
                                        }
                                    }
                                },
                                selected_options = new []
                                {
                                    new
                                    {
                                        text = "One Does Not Simply",
                                        value = "one-does-not-simply",
                                    }
                                },
                                text = "Image",
                                type = "select",
                            },
                            new
                            {
                                name = "switch",
                                style = "default",
                                text = "Next",
                                type = "button",
                                value = "distracted-boyfriend",
                            },
                            new
                            {
                                name = "edit",
                                style = "default",
                                text = "Edit",
                                type = "button",
                            },
                            new
                            {
                                name = "submit",
                                style = "primary",
                                text = "Post",
                                type = "button",
                            },
                        },
                        callback_id = "meme:" + messageGuid,
                        color = "#3AA3E3",
                        fallback = "Here you would choose to confirm posting your meme",
                        mrkdwn_in = new[] { "text" },
                        text = "_hint: use a semicolon to separate lines of text_",
                        title = "This is a preview of your meme",
                    }
                },
                response_type = "ephemeral",
                text = "<@U2147483697> used `/lang`"
            });
            Assert.Equal(expected.ToString(), actual.ToString());
        }
    }
}
