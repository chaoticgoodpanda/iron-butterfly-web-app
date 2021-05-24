// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;

namespace IronButterflyBot
{
    public class UserProfileDialog : ComponentDialog
    {
        private readonly IStatePropertyAccessor<UserProfile> _userProfileAccessor;

        public UserProfileDialog(UserState userState)
            : base(nameof(UserProfileDialog))
        {
            _userProfileAccessor = userState.CreateProperty<UserProfile>("UserProfile");

            // This array defines how the Waterfall will execute.
            var waterfallSteps = new WaterfallStep[]
            {
                ActionStepAsync,
                IOUStepAsync,
                PaymentOptionStepAsync,
                PhoneNumberStepAsync,
                AppStepAsync,
                SummaryStepAsync,
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            //AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>), AgePromptValidatorAsync));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            //AddDialog(new AttachmentPrompt(nameof(AttachmentPrompt), PicturePromptValidatorAsync));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> ActionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            // Running a prompt here means the next WaterfallStep will be run when the user's response is received.
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("ဘာလုပ်ချင်လို့လဲ / What would you like to do?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "IOUs ဝယ်ပါ", "IOU ကြည့်ပါ" }),
                }, cancellationToken);
        }

        private static async Task<DialogTurnResult> IOUStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["iouOption"] = ((FoundChoice)stepContext.Result).Value;

            var iouStatus = stepContext.Values["iouOption"];

            if(iouStatus.Equals("IOUs ဝယ်ပါ"))
            {
                
                return await stepContext.PromptAsync(nameof(ChoicePrompt),
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("ကျေးဇူးပြုပြီး NUG နဲ့ IOU အတွက်ဘယ်လောက်ပေးရမလည်းကိုရွေးချယ်ပါ / Please select how much you want to pay for an IOU with NUG."),
                        Choices = ChoiceFactory.ToChoices(new List<string> { "၁ သိန်း", "၂ သိန်း", "၅ သိန်း", "၅ သိန်းကျော်" }),
                    }, cancellationToken);
            }
            else 
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Here are your outstanding IOUs with NUG.") }, cancellationToken);
            }

            
        }

        private async Task<DialogTurnResult> PaymentOptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["paymentAmount"] = ((FoundChoice)stepContext.Result).Value;

            // We can send messages to the user at any point in the WaterfallStep.
            //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Thanks {stepContext.Result}."), cancellationToken);

            return await stepContext.PromptAsync(nameof(ChoicePrompt),
            new PromptOptions
            {
                Prompt = MessageFactory.Text("ကျေးဇူးပြု၍ သင်၏ငွေပေးချေမှုနည်းလမ်းကိုရွေးချယ်ပါ / Please select your payment method."),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Wave", "M-Pietsan", "AYA", "CB", "KBZ" }),
            }, cancellationToken);

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            //return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text("Would you like to give your age?") }, cancellationToken);
        }

        private async Task<DialogTurnResult> PhoneNumberStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["paymentMethod"] = ((FoundChoice)stepContext.Result).Value;
            
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("ကျေးဇူးပြု၍ သင့်ဖုန်းနံပါတ်ကို +09xxxxxxxxx ပုံစံဖြင့်ရိုက်ထည့်ပါ / Please enter your phone number in 09xxxxxxxxx format") }, cancellationToken);
            /*var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("ကျေးဇူးပြု၍ သင့်ဖုန်းနံပါတ်ကို +95 9 123456789 ပုံစံဖြင့်ရိုက်ထည့်ပါ / Please enter your phone number in +95 9 123456789 format."),
                //RetryPrompt = MessageFactory.Text("The value entered must be greater than 0 and less than 150."),
            };

            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);*/
            

        }

        private async Task<DialogTurnResult> AppStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["phoneNumber"] = (string)stepContext.Result;

            // We can send messages to the user at any point in the WaterfallStep.
            //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Thanks {stepContext.Result}."), cancellationToken);

            return await stepContext.PromptAsync(nameof(ChoicePrompt),
            new PromptOptions
            {
                Prompt = MessageFactory.Text("ဆက်လုပ်ရန်အတွက်အောက်ပါအက်ပလီကေးရှင်းများနှင့်ဆက်သွယ်သင့်သည်။ ကျေးဇူးပြု၍ သင်၏ဖုန်းနံပါတ်ချိတ်ဆက်ထားသောအက်ပ်ကိုရွေးပါ / We need to contact you on the following apps to continue. Please select app your phone number is connected to."),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Signal", "Telegram" }),
            }, cancellationToken);

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            //return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text("Would you like to give your age?") }, cancellationToken);
        }

        private async Task<DialogTurnResult> SummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["appType"] = ((FoundChoice)stepContext.Result).Value;

            // Get the current profile object from user state.
            var userProfile = await _userProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

            userProfile.IouOption = (string)stepContext.Values["iouOption"];
            userProfile.PaymentAmount = (string)stepContext.Values["paymentAmount"];
            userProfile.PaymentMethod = (string)stepContext.Values["paymentMethod"];
            userProfile.PhoneNumber = (string)stepContext.Values["phoneNumber"];
            userProfile.AppType = (string)stepContext.Values["appType"];

            var msg = $"You have selected IOU Option {userProfile.IouOption} and your payment amount is {userProfile.PaymentAmount}.";
            msg += $" Your payment method is {userProfile.PaymentMethod} and your phone number is {userProfile.PhoneNumber}.";
            msg += $"We will contact you shortly on {userProfile.AppType}. Thank you for your support of NUG!";

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);



            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is the end.
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }


        private static async Task<bool> PicturePromptValidatorAsync(PromptValidatorContext<IList<Attachment>> promptContext, CancellationToken cancellationToken)
        {
            if (promptContext.Recognized.Succeeded)
            {
                var attachments = promptContext.Recognized.Value;
                var validImages = new List<Attachment>();

                foreach (var attachment in attachments)
                {
                    if (attachment.ContentType == "image/jpeg" || attachment.ContentType == "image/png")
                    {
                        validImages.Add(attachment);
                    }
                }

                promptContext.Recognized.Value = validImages;

                // If none of the attachments are valid images, the retry prompt should be sent.
                return validImages.Any();
            }
            else
            {
                await promptContext.Context.SendActivityAsync("No attachments received. Proceeding without a profile picture...");

                // We can return true from a validator function even if Recognized.Succeeded is false.
                return true;
            }
        }
    }
}
