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
        private string separator = "##### {number} - {fileName} ({filePath}) #####";
        private string filesFilter = @"^\w+\.\w{3}$";
        private int entersAfter = 2;
        private readonly IDialogsService dialogsService;
        private ObservableCollection<FileEntry> fileEntries = new ObservableCollection<FileEntry>();
        private ObservableCollection<FileEntry> selectedFileEntries;

        public MainViewModel()
        {
            dialogsService = new DialogsService();
        }

        public RelayCommand MergeCommand => new RelayCommand(() =>
        {
            try
            {
                if (FileEntries.Count == 0)
                {
                    dialogsService.ShowError("No files selected");
                    return;
                }

                var mergedFile = PrepareMergedFile();

                DialogFilterPair dialogFilterPair = null;
                var extension = GetExtensionFromFiles();
                if (!string.IsNullOrEmpty(extension))
                {
                    dialogFilterPair = new DialogFilterPair(extension);
                }
                var outputFile = dialogsService.SaveFile("Save merged file", null, null, dialogFilterPair);
                if (!string.IsNullOrEmpty(outputFile))
                {
                    File.WriteAllText(outputFile, mergedFile);
                    Status = $"Merged {FileEntries.Count} into {outputFile}";
                }
            }
            catch (Exception exc)
            {
                dialogsService.ShowException(exc, "File merging error");
            }
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

        public int EntersAfter
        {
            get => entersAfter;
            set => Set(ref entersAfter, value);
        }

        public string FilesFilter
        {
            get => filesFilter;
            set => Set(ref filesFilter, value);
        }

        public string Separator
        {
            get => separator;
            set => Set(ref separator, value);
        }

        private string PrepareMergedFile()
        {
            var result = string.Empty;
            for (int i = 0; i < FileEntries.Count; i++)
            {
                var fileEntry = fileEntries[i];
                var file = string.Empty;
                if (!string.IsNullOrEmpty(Separator))
                {
                    file += PrepareSeparator(fileEntry) + Environment.NewLine;
                }

                file += File.ReadAllText(fileEntry.FullPath);

                if (i != FileEntries.Count - 1)
                {
                    for (int j = 0; j < entersAfter; j++)
                    {
                        file += Environment.NewLine;
                    }
                }

                result += file;
            }
            return result;
        }

        private string PrepareSeparator(FileEntry fileEntry)
        {
            var result = Separator;
            result = result.Replace("{number}", fileEntry.Number.ToString());
            result = result.Replace("{fileName}", fileEntry.FileName);
            result = result.Replace("{filePath}", fileEntry.FilePath);
            return result;
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
            if (string.IsNullOrEmpty(FilesFilter))
            {
                return files;
            }
            else
            {
                var filterRegex = new Regex(FilesFilter);
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

        private string GetExtensionFromFiles()
        {
            return FileEntries.Select(s => s.Extension).Distinct().Count() == 1 ? FileEntries[0].Extension : null;
        }
    }
}