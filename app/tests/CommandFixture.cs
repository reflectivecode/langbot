using System;
using Xunit;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
                { "user_id", "U2147483697" },
                { "command", "/lang" },
                { "text", "This is a test" },
            });

            var response = await Client.PostAsync("/api/command", content);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode); Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var actual = JObject.Parse(await response.Content.ReadAsStringAsync());
            var imageUrl = String.Format("http://localhost/api/Image?Image={0}&Hash=SLvqeGu8PQ5_VjFBxktQAOW75js6aZ-OucXePjkY-1E", Base64UrlEncode(new
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
                        vertical = 0,
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
                        vertical = 2,
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
                                text = "Image",
                                type = "select",
                                selected_options = new []
                                {
                                    new
                                    {
                                        text = "One Does Not Simply",
                                        value = Base64UrlEncode(new
                                        {
                                            text = "This is a test",
                                            template_id = "one-does-not-simply",
                                            user_id = "U2147483697",
                                            anonymous = false
                                        }),
                                    }
                                },
                                option_groups = new []
                                {
                                    new
                                    {
                                        text = "Change Image",
                                        options = new object []
                                        {
                                            new
                                            {
                                                text = "One Does Not Simply",
                                                value = Base64UrlEncode(new
                                                {
                                                    text = "This is a test",
                                                    template_id = "one-does-not-simply",
                                                    user_id = "U2147483697",
                                                    anonymous = false
                                                }),
                                                description = "(selected)"
                                            },
                                            new
                                            {
                                                text = "Success Kid",
                                                value = Base64UrlEncode(new
                                                {
                                                    text = "This is a test",
                                                    template_id = "success-kid",
                                                    user_id = "U2147483697",
                                                    anonymous = false
                                                })
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
                                                value = Base64UrlEncode(new
                                                {
                                                    text = "This is a test",
                                                    template_id = "one-does-not-simply",
                                                    user_id = "U2147483697",
                                                    anonymous = false
                                                }),
                                                description = "(selected)"
                                            },
                                            new
                                            {
                                                text = "Post anonymously",
                                                value = Base64UrlEncode(new
                                                {
                                                    text = "This is a test",
                                                    template_id = "one-does-not-simply",
                                                    user_id = "U2147483697",
                                                    anonymous = true
                                                }),
                                            }
                                        }
                                    }
                                }
                            },
                            new
                            {
                                name = "submit",
                                style = "primary",
                                text = "Post",
                                type = "button",
                                value = Base64UrlEncode(new
                                {
                                    image_url = imageUrl,
                                    fallback = "This is a test",
                                    user_id = "U2147483697"
                                }),
                            },
                        },
                        callback_id = "meme",
                        color = "#3AA3E3",
                        fallback = "Here you would choose to confirm posting your meme",
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
