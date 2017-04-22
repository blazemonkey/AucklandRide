using System;
using System.Threading.Tasks;

namespace Services.MessageDialogService
{
    public interface IMessageDialogService
    {
        Task Show(string text);
        Task<bool> ShowYesNo(string text, Action executeOnYes);
    }
}
