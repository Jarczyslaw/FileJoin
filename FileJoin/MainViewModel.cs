using JToolbox.Core.Extensions;
using JToolbox.Desktop.Dialogs;
using JToolbox.WPF.Core.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FileJoin
{
    public class MainViewModel : BaseViewModel
    {
        private string status;
        private string separator = "##### {number} - {fileName} #####";
        private string fileFilter = @"^\w+\.\w{3}$";
        private ObservableCollection<FileEntry> fileEntries = new ObservableCollection<FileEntry>();
        private readonly IDialogsService dialogsService;
        private ObservableCollection<FileEntry> selectedFileEntries;

        public MainViewModel()
        {
            dialogsService = new DialogsService();
        }

        public RelayCommand MergeCommand => new RelayCommand(() =>
        {
            SelectedFileEntries = fileEntries;
        });

        public RelayCommand AddFilesCommand => new RelayCommand(() =>
        {
            try
            {
                var selectedFiles = dialogsService.OpenFiles("Add files");
                if (selectedFiles != null)
                {
                    AddFiles(selectedFiles);
                }
            }
            catch (Exception exc)
            {
                dialogsService.ShowException(exc, "Error while adding files");
            }
        });

        public RelayCommand AddFolderCommand => new RelayCommand(() =>
        {
            try
            {
                var selectedFolder = dialogsService.OpenFolder("Open folder");
                if (!string.IsNullOrEmpty(selectedFolder))
                {
                    var files = Directory.EnumerateFiles(selectedFolder);
                    AddFiles(files);
                }
            }
            catch (Exception exc)
            {
                dialogsService.ShowException(exc, "Error while adding folder");
            }
        });

        public RelayCommand AddFoldersCommand => new RelayCommand(() =>
        {
            try
            {
                var selectedFolder = dialogsService.OpenFolder("Open folder");
                if (!string.IsNullOrEmpty(selectedFolder))
                {
                    var files = Directory.EnumerateFiles(selectedFolder, "*.*", SearchOption.AllDirectories);
                    AddFiles(files);
                }
            }
            catch (Exception exc)
            {
                dialogsService.ShowException(exc, "Error while adding folders");
            }
        });

        public RelayCommand MoveUpCommand => new RelayCommand(() =>
        {
            var selectedEntries = SelectedFileEntries.OrderBy(s => s.Number);
            foreach (var entry in selectedEntries)
            {
                var index = FileEntries.IndexOf(entry);
                if (index > 0)
                {
                    var entryToShift = FileEntries[index - 1];
                    if (!selectedEntries.Contains(entryToShift))
                    {
                        FileEntries.ShiftLeft(index);
                    }
                }
            }
            RenumarateFiles();
        });

        public RelayCommand MoveDownCommand => new RelayCommand(() =>
        {
            var selectedEntries = SelectedFileEntries.OrderBy(s => s.Number)
                .ToList();
            for (int i = selectedEntries.Count - 1; i >= 0; i--)
            {
                var entry = selectedEntries[i];
                var index = FileEntries.IndexOf(entry);
                if (index < FileEntries.Count - 1)
                {
                    var entryToShift = FileEntries[index + 1];
                    if (!selectedEntries.Contains(entryToShift))
                    {
                        FileEntries.ShiftRight(index);
                    }
                }
            }
            RenumarateFiles();
        });

        public RelayCommand RemoveCommand => new RelayCommand(() =>
        {
            if (SelectedFileEntries != null && SelectedFileEntries.Count != 0)
            {
                var count = SelectedFileEntries.Count;
                foreach (var selectedFile in SelectedFileEntries)
                {
                    FileEntries.Remove(selectedFile);
                }
                RenumarateFiles();
                Status = $"Removed {count} files";
            }
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

        private void AddFiles(IEnumerable<string> files)
        {
            if (files != null)
            {
                var filteredFiles = FilterFiles(files);
                foreach (var file in filteredFiles)
                {
                    FileEntries.Add(new FileEntry(file));
                }
                RenumarateFiles();

                var diff = files.Count() - filteredFiles.Count();
                var message = $"Added {filteredFiles.Count()} files";
                if (diff > 0)
                {
                    message += $" ({diff} filtered)";
                }
                Status = message;
            }
        }

        private IEnumerable<string> FilterFiles(IEnumerable<string> files)
        {
            if (string.IsNullOrEmpty(FileFilter))
            {
                return files;
            }
            else
            {
                var filterRegex = new Regex(FileFilter);
                return files.Where(f => filterRegex.IsMatch(Path.GetFileName(f)));
            }
        }

        private void RenumarateFiles()
        {
            var number = 1;
            foreach (var file in FileEntries)
            {
                file.Number = number;
                number++;
            }
        }
    }
}