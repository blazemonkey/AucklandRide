using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Services.MessageDialogService
{
    public class MessageDialogService : IMessageDialogService
    {
        public async Task Show(string text)
        {
            var dialog = new MessageDialog(text);
            await dialog.ShowAsync();
        }

        public async Task<bool> ShowYesNo(string text, Action executeOnYes)
        {
            var dialog = new MessageDialog(text);
            var result = false;
            dialog.Commands.Add(new UICommand("Yes", delegate (IUICommand command)
            {
                executeOnYes.Invoke();
                result = true;
            }));
            dialog.Commands.Add(new UICommand("No", delegate (IUICommand command)
            {
                result = false;
            }));

            await dialog.ShowAsync();

            return result;
        }
    }
}
