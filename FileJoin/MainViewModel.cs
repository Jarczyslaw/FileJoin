using JToolbox.Desktop.Dialogs;
using JToolbox.WPF.Core.Base;
using System.Collections.ObjectModel;
using System.IO;

namespace FileJoin
{
    public class MainViewModel : BaseViewModel
    {
        private string status;
        private bool useSeparator;
        private ObservableCollection<FileEntry> fileEntries = new ObservableCollection<FileEntry>();
        private readonly IDialogsService dialogsService;

        public MainViewModel()
        {
            dialogsService = new DialogsService();

            FileEntries.Add(new FileEntry
            {
                FullPath = @"e:\TEMP\file1.pdf"
            });
            FileEntries.Add(new FileEntry
            {
                FullPath = @"e:\TEMP\file2.reg"
            });
            FileEntries.Add(new FileEntry
            {
                FullPath = @"e:\TEMP\file3.pdf"
            });
        }

        public RelayCommand MergeCommand => new RelayCommand(() =>
        {
            Status = "Status";
        });

        public ObservableCollection<FileEntry> FileEntries
        {
            get => fileEntries;
            set => Set(ref fileEntries, value);
        }

        public string Status
        {
            get => status;
            set => Set(ref status, value);
        }

        public bool UseSeparator
        {
            get => useSeparator;
            set => Set(ref useSeparator, value);
        }
    }
}