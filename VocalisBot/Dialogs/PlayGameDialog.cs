using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace VocalisBot.Dialogs
{
    [Serializable]
    public class PlayGameDialog : IDialog<object>
    {
        private Hangman _game;

        public async Task StartAsync(IDialogContext context)
        {
            _game = new Hangman();
            PromptDialog.Text(context, ResumeAfterInput, Response.PlayGame_Start);
        }

        private async Task ResumeAfterRetryConfirmation(IDialogContext context, IAwaitable<bool> result)
        {
            var retry = await result;
            if(retry)
            {
                _game = new Hangman();
                PromptDialog.Text(context, ResumeAfterInput, Response.PlayGame_Retry);
            }
            else
            {
                context.Done<object>(null);
            }
        }

        
        private async Task ResumeAfterInput(IDialogContext context, IAwaitable<string> result)
        {
            var input = (await result).ToLower();
            if(input.Equals("quit"))
            {
                context.Done<object>(null);
            }
            else
            {
                var inputChar = input[0];
                if(_game.IsCorrectGuess(inputChar))
                {
                    if(_game.GameWon)
                    {
                        PromptDialog.Confirm(context, ResumeAfterRetryConfirmation, string.Format(Response.PlayGame_Won, _game.CurrentWord));
                    }
                    else
                    {
                        PromptDialog.Text(context, ResumeAfterInput, string.Format(Response.PlayGame_CorrectGuess, _game.HiddenWord));
                    }
                }
                else if (_game.GameLost)
                {
                    PromptDialog.Confirm(context, ResumeAfterRetryConfirmation, string.Format(Response.PlayGame_Lost, _game.CurrentWord));
                }
                else
                {
                    PromptDialog.Text(context, ResumeAfterInput, $"Sadly, that was wrong. Your guesses: [{string.Join(",", _game.GuessedCharacters)}] \n {_game.CurrentPic} \n What's your next guess?");
                }
            }
        }
    }
}