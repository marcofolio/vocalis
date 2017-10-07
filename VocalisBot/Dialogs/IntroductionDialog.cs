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
    public class IntroductionDialog : IDialog<object>
    {
        private enum IntroductionType
        {
            Hobbies, Picture, Work, Residence, Nothing
        }

        private string _name;

        public IntroductionDialog(string name)
        {
            _name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.ToLower());
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(string.Format(Response.Introduction_Introduce, _name));
            IntroductionTypeClarification(context);
        }

        public async Task RestartAsync(IDialogContext context)
        {
            await context.PostAsync(string.Format(Response.Introduction_Restart, _name));
            IntroductionTypeClarification(context);
        }

        private void IntroductionTypeClarification(IDialogContext context)
        {
            var options = new IntroductionType[] { IntroductionType.Hobbies, IntroductionType.Picture, IntroductionType.Work, IntroductionType.Residence, IntroductionType.Nothing };
            var descriptions = new string[] { "My hobbies", "A picture of me", "Something about work", "My residence", "Nothing" };
            PromptDialog.Choice<IntroductionType>(context, ResumeAfterIntroductionTypeClarification, options, Response.Introduction_Choice, descriptions: descriptions);
        }

        private async Task ResumeAfterIntroductionTypeClarification(IDialogContext context, IAwaitable<IntroductionType> result)
        {
            IntroductionType introductionType = await result;

            switch (introductionType)
            {
                case IntroductionType.Hobbies:
                    PromptDialog.Text(context, ResumeAfterHobbiesClarification, Response.Introduction_HobbiesStart);
                    break;
                case IntroductionType.Picture:
                    PromptDialog.Attachment(context, ResumeAfterPictureClarification, Response.Introduction_PictureStart);
                    break;
                case IntroductionType.Work:
                    PromptDialog.Text(context, ResumeAfterWorkClarification, Response.Introduction_WorkStart);
                    break;
                case IntroductionType.Nothing:
                    context.Done<object>(null);
                    break;
                default:
                    await context.PostAsync(Response.Introduction_UnknownStart);
                    context.Done<object>(null);
                    break;
            }
        }

        private async Task ResumeAfterHobbiesClarification(IDialogContext context, IAwaitable<string> result)
        {
            var hobbyHeroes = new Dictionary<string, string>()
            {
                { "cooking", "Jamie Oliver" },
                { "travelling", "Christopher Columbus" },
                { "soccer", "Lionel Messi" },
                { "developing", "Alan Turing" },
            };

            var foundHeroes = new List<string>();

            var hobbies = await result;
            hobbies = hobbies.Replace(",", "").Replace("and", "").Replace("&", "");
            var hobbiesArray = hobbies.Split(' ');
            foreach(var hobby in hobbiesArray)
            {
                var hobbyLower = hobby.ToLower();
                if(hobbyHeroes.ContainsKey(hobbyLower))
                {
                    foundHeroes.Add(hobbyHeroes[hobbyLower]);
                }
            }
            
            await context.PostAsync(string.Format(Response.Introduction_HobbiesHeroes, string.Join(", ", foundHeroes)));
            await RestartAsync(context);
        }

        private async Task ResumeAfterPictureClarification(IDialogContext context, IAwaitable<IEnumerable<Attachment>> result)
        {
            await context.PostAsync(Response.Introduction_PictureSent);
            try
            {
                var attachments = await result;
                var faceAttributes = await FaceApiService.DetectFaceAttributesAsync(attachments.First().ContentUrl);
                
                // Parse the result
                var gender = faceAttributes.Gender;
                var age = (int) Math.Round(faceAttributes.Age);

                await context.PostAsync($"Now that's one good looking *{gender}*! I'm guessing you're around *{age}* years of age?");
            }
            catch (Exception ex)
            {
                await context.PostAsync($"ERROR: {ex.Message}");
                await context.PostAsync(Response.Error);
            }

            await RestartAsync(context);
        }

        private async Task ResumeAfterWorkClarification(IDialogContext context, IAwaitable<string> result)
        {
            var company = await result;

            if(company.ToLowerInvariant().Equals("infosupport"))
            {
                var reply = context.MakeMessage();
                var heroCard = new HeroCard()
                {
                    Title = "InfoSupport",
                    Subtitle = "InfoSupport is an IT-company based in Veenendaal, The Netherlands. We specialize in building, hosting and maintaining custom software solutions.",
                    Images = new List<CardImage>
                        {
                            new CardImage(url: "https://innovationprojects.infosupport.com/wp-content/uploads/2015/12/Info-Support-30cm-300DPI-PNG.png")
                        },
                    Buttons = new List<CardAction>
                        {
                            new CardAction()
                                {
                                    Value = "https://www.infosupport.com/",
                                    Type = "openUrl",
                                    Title = "Visit website"
                                }
                        }
                };

                reply.Attachments = new List<Attachment>
                {
                    heroCard.ToAttachment()
                };
                await context.PostAsync(reply);
                await context.PostAsync($"I've been told you're a *manager*, but you prefer yourself to be called a *Code Monkey*, correct?");
            }

            await RestartAsync(context);
        }
    }
}