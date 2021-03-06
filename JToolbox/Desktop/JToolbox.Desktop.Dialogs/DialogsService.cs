﻿using JToolbox.Desktop.Dialogs.Builders;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JToolbox.Desktop.Dialogs
{
    public class DialogsService : IDialogsService
    {
        public void ShowInfo(string message, string details = null, IntPtr? owner = null)
        {
            var builder = new TaskDialogBuilder()
                .Initialize(Resources.Resources.Information, message, TaskDialogStandardIcon.Information, Resources.Resources.Information)
                .AddDetails(Resources.Resources.ShowDetails, Resources.Resources.HideDetails, details)
                .SetOwner(GetOwnerHandle(owner));

            using (var dialog = builder.Build())
            {
                dialog.Show();
            }
        }

        public void ShowWarning(string message, string details = null, IntPtr? owner = null)
        {
            var builder = new TaskDialogBuilder()
                .Initialize(Resources.Resources.Warning, message, TaskDialogStandardIcon.Warning, Resources.Resources.Warning)
                .AddDetails(Resources.Resources.ShowDetails, Resources.Resources.HideDetails, details)
                .SetOwner(GetOwnerHandle(owner));

            using (var dialog = builder.Build())
            {
                dialog.Show();
            }
        }

        public void ShowError(string error, string details = null, IntPtr? owner = null)
        {
            var builder = new TaskDialogBuilder()
                .Initialize(Resources.Resources.Error, error, TaskDialogStandardIcon.Error, Resources.Resources.Error)
                .AddDetails(Resources.Resources.ShowDetails, Resources.Resources.HideDetails, details)
                .SetOwner(GetOwnerHandle(owner));

            using (var dialog = builder.Build())
            {
                dialog.Show();
            }
        }

        public void ShowException(Exception exception, string message = null, IntPtr? owner = null)
        {
            ShowExceptionDialog(Resources.Resources.Exception, exception, message, owner);
        }

        public void ShowCriticalException(Exception exception, string message = null, IntPtr? owner = null)
        {
            ShowExceptionDialog(Resources.Resources.CriticalException, exception, message, owner);
        }

        public bool ShowYesNoQuestion(string question, IntPtr? owner = null)
        {
            var builder = new TaskDialogBuilder()
                .Initialize(Resources.Resources.Question, question, TaskDialogStandardIcon.Information, Resources.Resources.Question)
                .SetButtons(TaskDialogStandardButtons.Yes, TaskDialogStandardButtons.No)
                .SetOwner(GetOwnerHandle(owner));

            var result = false;
            using (var dialog = builder.Build())
            {
                result = dialog.Show() == TaskDialogResult.Yes;
            }

            return result;
        }

        public T ShowCustomButtonsQuestion<T>(string question, IEnumerable<CustomButtonData<T>> customButtons, IntPtr? owner = null)
        {
            var builder = new TaskDialogBuilder()
                .Initialize(Resources.Resources.Question, question, TaskDialogStandardIcon.Information, Resources.Resources.Question)
                .SetOwner(GetOwnerHandle(owner));

            var result = default(T);
            using (var dialog = builder.Build())
            {
                foreach (var customButton in customButtons)
                {
                    builder.AddCustomButton(customButton.Name, customButton.Caption, (s, e) =>
                    {
                        result = customButton.Value;
                        dialog.Close();
                    }, customButton.Default);
                }
                dialog.Show();
            }

            return result;
        }

        public void ShowProgressDialog(string caption, string text, string instruction, IntPtr? owner = null)
        {
            var builder = new TaskDialogBuilder()
                .Initialize(caption, text, TaskDialogStandardIcon.Information, instruction)
                .SetButtons(TaskDialogStandardButtons.Cancel)
                .AddProgressbar(0, 100, TaskDialogProgressBarState.Marquee)
                .SetOwner(GetOwnerHandle(owner));

            using (var dialog = builder.Build())
            {
                dialog.Show();
            }
        }

        public string OpenFile(string title, string initialDirectory = null, List<DialogFilterPair> filters = null)
        {
            var builder = new CommonOpenDialogBuilder()
                .Initialize(title, initialDirectory)
                .SetAsFileDialog(false)
                .AddFilters(filters);

            string result = null;
            using (var dialog = builder.Build())
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    result = dialog.FileName;
                }
            }
            return result;
        }

        public List<string> OpenFiles(string title, string initialDirectory = null, List<DialogFilterPair> filters = null)
        {
            var builder = new CommonOpenDialogBuilder()
                .Initialize(title, initialDirectory)
                .SetAsFileDialog(true)
                .AddFilters(filters);

            List<string> result = null;
            using (var dialog = builder.Build())
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    result = dialog.FileNames.ToList();
                }
            }
            return result;
        }

        public string OpenFolder(string title, string initialDirectory = null)
        {
            var builder = new CommonOpenDialogBuilder()
                .Initialize(title, initialDirectory)
                .SetAsFolderDialog();

            string result = null;
            using (var dialog = builder.Build())
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    result = dialog.FileName;
                }
            }
            return result;
        }

        public string SaveFile(string title, string initialDirectory, string defaultFileName = null, DialogFilterPair filter = null)
        {
            var builder = new CommonSaveDialogBuilder()
                .Initialize(title, initialDirectory)
                .SetDefaults(defaultFileName, filter.ExtensionsList)
                .AddFilter(filter);

            string result = null;
            using (var dialog = builder.Build())
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    result = dialog.FileName;
                }
            }
            return result;
        }

        private void ShowExceptionDialog(string caption, Exception exception, string message, IntPtr? owner)
        {
            var text = exception.Message;
            if (!string.IsNullOrEmpty(message))
            {
                text = message + Environment.NewLine + text;
            }
            var builder = new TaskDialogBuilder()
                .Initialize(caption, text, TaskDialogStandardIcon.Error, Resources.Resources.ExceptionOccured)
                .AddDetails(Resources.Resources.ShowDetails, Resources.Resources.HideDetails, exception.StackTrace)
                .SetOwner(GetOwnerHandle(owner));

            using (var dialog = builder.Build())
            {
                dialog.Show();
            }
        }

        private IntPtr GetOwnerHandle(IntPtr? owner)
        {
            if (owner == null)
            {
                return NativeMethods.GetActiveWindow();
            }
            else if (owner.Value == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }
            else
            {
                return owner.Value;
            }
        }
    }
}