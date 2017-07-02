using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.Bot.Connector;
using VocalisBot.Cognitive;

namespace VocalisBot.Dialogs
{
    [Serializable]
    public class CrowdInsightsDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            PromptDialog.Attachment(context, ResumeAfterPictureClarification, Response.CrowdInsights_PictureStart);
        }

        private async Task ResumeAfterPictureClarification(IDialogContext context, IAwaitable<IEnumerable<Attachment>> result)
        {
            await context.PostAsync(Response.CrowdInsights_PictureSent);
            try
            {
                var attachments = await result;
                var contentUrl = attachments.First().ContentUrl;
                
                // Let the Cognitive Services to their work
                var detectFacesAndGenderTask = FaceApiService.DetectFacesAndGenderAsync(contentUrl);
                var visionTask = ComputerVisionService.DescribeAsync(contentUrl);
                await Task.WhenAll(detectFacesAndGenderTask, visionTask);
                var facesAndGender = await detectFacesAndGenderTask;
                var vision = await visionTask;

                // Parse the result
                await context.PostAsync($"I think you're looking at _{vision.Text}_ , neat! I'm about _{Math.Floor(vision.Confidence * 100)}_% sure.");
                await context.PostAsync($"Your crowd consists of *{facesAndGender.Length}* people, from which *{facesAndGender.Where(x => x.FaceAttributes.Gender.Equals("male")).Count()}* are male and *{facesAndGender.Where(x => x.FaceAttributes.Gender.Equals("female")).Count()}* are female.");
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