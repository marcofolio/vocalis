using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace VocalisBot.Dialogs
{
    [LuisModel(Constants.LUIS_MODEL_ID, Constants.LUIS_SUBSCRIPTION_KEY)]
    [Serializable]
    public class RootDialog : LuisDialog<object>
    {
        #region None

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(Response.None_Empty);
        }

        #endregion

        #region Introduce

        [LuisIntent("Introduce")]
        public async Task Introduce(IDialogContext context, LuisResult result)
        {
            string name = TryFindEntity(result, "name");

            if (string.IsNullOrEmpty(name))
            {
                PromptDialog.Text(context, ResumeAfterNameClarification, Response.Introduce_AskName);
            }
            else
            {
                await StartIntroductionDialog(context, name);
            }
        }

        private async Task ResumeAfterNameClarification(IDialogContext context, IAwaitable<string> result)
        {
            string name = await result;
            await StartIntroductionDialog(context, name);
        }

        private async Task StartIntroductionDialog(IDialogContext context, string name)
        {
            context.Call(new IntroductionDialog(name), ResumeAfterIntroductionDialog);
        }

        private async Task ResumeAfterIntroductionDialog(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync(Response.Introduce_Done);
        }

        #endregion

        #region CrowdInsights

        [LuisIntent("CrowdInsights")]
        public async Task CrowdInsights(IDialogContext context, LuisResult result)
        {
            context.Call(new CrowdInsightsDialog(), ResumeAfterCrowdInsightsDialog);
        }

        private async Task ResumeAfterCrowdInsightsDialog(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync(Response.CrowdInsights_Done);
        }

        #endregion

        #region SendPicture

        [LuisIntent("SendPicture")]
        public async Task SendPicture(IDialogContext context, LuisResult result)
        {
            string objectPicture = TryFindEntity(result, "object");

            if (string.IsNullOrEmpty(objectPicture))
            {
                PromptDialog.Text(context, ResumeAfterObjecPictureClarification, Response.SendPicture_Clarify);
            }
            else
            {
                await StartSendPictureDialog(context, objectPicture);
            }
        }

        private async Task ResumeAfterObjecPictureClarification(IDialogContext context, IAwaitable<string> result)
        {
            string objectPicture = await result;
            await StartSendPictureDialog(context, objectPicture);
        }

        private async Task StartSendPictureDialog(IDialogContext context, string objectPicture)
        {
            context.Call(new SendPictureDialog(objectPicture), ResumeAfterSendPictureDialog);
        }

        private async Task ResumeAfterSendPictureDialog(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync(Response.SendPicture_Done);
        }

        #endregion

        #region PlayGame

        [LuisIntent("PlayGame")]
        public async Task PlayGame(IDialogContext context, LuisResult result)
        {
            context.Call(new PlayGameDialog(), ResumeAfterPlayGameDialog);
        }

        private async Task ResumeAfterPlayGameDialog(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync(Response.PlayGame_Done);
        }

        #endregion

        private string TryFindEntity(LuisResult result, string type)
        {
            // Check if LUIS has identified the entity that we should look for.
            string output = null;
            EntityRecommendation rec;
            if (result.TryFindEntity(type, out rec)) output = rec.Entity;
            return output;
        }
    }
}