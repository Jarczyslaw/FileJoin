using System;
using System.Collections.Generic;

namespace JToolbox.Desktop.Dialogs
{
    public interface IDialogsService
    {
        void ShowInfo(string message, string details = null, IntPtr? owner = null);
        void ShowWarning(string message, string details = null, IntPtr? owner = null);
        void ShowError(string error, string details = null, IntPtr? owner = null);
        void ShowException(Exception exception, string message = null, IntPtr? owner = null);
        void ShowCriticalException(Exception exception, string message = null, IntPtr? owner = null);
        bool ShowYesNoQuestion(string question, IntPtr? owner = null);
        T ShowCustomButtonsQuestion<T>(string question, IEnumerable<CustomButtonData<T>> customButtons, IntPtr? owner = null);
        void ShowProgressDialog(string caption, string text, string instruction, IntPtr? owner = null);
        string OpenFile(string title, string initialDirectory, DialogFilterPair filter);
        string OpenFile(string title, string initialDirectory, List<DialogFilterPair> filters);
        List<string> OpenFiles(string title, string initialDirectory, DialogFilterPair filter);
        List<string> OpenFiles(string title, string initialDirectory, List<DialogFilterPair> filters);
        string OpenFolder(string title, string initialDirectory);
        string SaveFile(string title, string initialDirectory, string defaultFileName, DialogFilterPair filter);
    }
}
