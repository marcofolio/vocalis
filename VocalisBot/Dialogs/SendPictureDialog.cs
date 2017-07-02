using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.Bot.Connector;
using VocalisBot.Cognitive;
using System.Net;

namespace VocalisBot.Dialogs
{
    [Serializable]
    public class SendPictureDialog : IDialog<object>
    {
        private string _objectPicture;

        public SendPictureDialog(string objectPicture)
        {
            _objectPicture = objectPicture;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(Response.SendPicture_Start);

            try
            {
                var image = await BingImageService.GetBestImageAsync(_objectPicture);

                var reply = context.MakeMessage();
                var heroCard = new HeroCard()
                {
                    Title = $"Bing: '{_objectPicture}'",
                    Subtitle = $"Top image for '{_objectPicture}' query",
                    Images = new List<CardImage>
                        {
                            new CardImage(url: image.ContentUrl)
                        },
                        Buttons = new List<CardAction>
                        {
                            new CardAction()
                                {
                                    Value = image.WebSearchUrl,
                                    Type = "openUrl",
                                    Title = image.Name
                                }
                        }
                };

                reply.Attachments = new List<Attachment>
                {
                    heroCard.ToAttachment()
                };
                await context.PostAsync(reply);
            }
            catch (Exception ex)
            {
                await context.PostAsync($"ERROR: {ex.Message}");
                await context.PostAsync(Response.Error);
            }
            context.Done<object>(null);
        }
    }
}