using JToolbox.Desktop.Dialogs;
using JToolbox.WPF.Core.Base;
using System.Collections;
using System.Collections.ObjectModel;

namespace FileJoin
{
    public class MainViewModel : BaseViewModel
    {
        private string status;
        private string separator;
        private string fileFilter;
        private ObservableCollection<FileEntry> fileEntries = new ObservableCollection<FileEntry>();
        private readonly IDialogsService dialogsService;
        private ObservableCollection<FileEntry> selectedFileEntries;

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
            SelectedFileEntries = fileEntries;
        });

        public RelayCommand AddFileCommand => new RelayCommand(() =>
        {
            Status = "Status";
        });

        public RelayCommand AddFolderCommand => new RelayCommand(() =>
        {
            Status = "Status";
        });

        public RelayCommand AddFoldersCommand => new RelayCommand(() =>
        {
            Status = "Status";
        });

        public RelayCommand MoveUpCommand => new RelayCommand(() =>
        {
            Status = "Status";
        });

        public RelayCommand MoveDownCommand => new RelayCommand(() =>
        {
            Status = "Status";
        });

        public RelayCommand RemoveCommand => new RelayCommand(() =>
        {
            Status = "Status";
        });

        public ObservableCollection<FileEntry> SelectedFileEntries
        {
            get => selectedFileEntries;
            set => Set(ref selectedFileEntries, value);
        }

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

        public string FileFilter
        {
            get => fileFilter;
            set => Set(ref fileFilter, value);
        }

        public string Separator
        {
            get => separator;
            set => Set(ref separator, value);
        }
    }
}