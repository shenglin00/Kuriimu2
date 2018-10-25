﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using Kontract.Interfaces;
using Kore;
using Kore.SamplePlugins;
using Kuriimu2.Interface;
using Microsoft.Win32;

namespace Kuriimu2.ViewModels
{
    public sealed class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        #region Private

        private Kore.Kore _kore;

        #endregion

        public ShellViewModel()
        {
            DisplayName = "Kuriimu2";
            _kore = new Kore.Kore();

            // Load passed-in file
            if (AppBootstrapper.Args.Length > 0 && File.Exists(AppBootstrapper.Args[0]))
                LoadFile(AppBootstrapper.Args[0]);
        }

        public void OpenButton()
        {
            var ofd = new OpenFileDialog { Filter = _kore.FileFilters, Multiselect = true };
            if (ofd.ShowDialog() != true) return;

            foreach (var file in ofd.FileNames)
                LoadFile(file);
        }

        public void FileDrop(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files == null) return;

            foreach (var file in files)
                LoadFile(file);
        }

        public bool SaveButtonsEnabled() => (ActiveItem as IFileEditor)?.KoreFile.Adapter is ISaveFiles;

        public void SaveButton()
        {
            SaveFile();
        }

        public void SaveAsButton()
        {
            var filter = "Any File (*.*)|*.*";

            if (ActiveItem is IFileEditor editor)
            {
                filter = editor.KoreFile.Filter;

                var sfd = new SaveFileDialog { FileName = editor.KoreFile.FileInfo.Name, Filter = filter };
                if (sfd.ShowDialog() != true) return;

                SaveFile(sfd.FileName);
            }
            else
            {

            }
        }

        public void DebugButton()
        {
            _kore.Debug();
        }

        // Toolbars
        public Visibility TextEditorToolsVisible => ActiveItem is TextEditor2ViewModel ? Visibility.Visible : Visibility.Hidden;

        public void TextEditorExportFile()
        {
            // TODO: Introduce a way to select from a list of ITextAdapter + ICreateFiles.
            var editor = (IFileEditor)ActiveItem;
            if (!(editor.KoreFile.Adapter is ITextAdapter adapter)) return;

            var kup = new KupAdapter();
            kup.Create();
            kup.Entries = adapter.Entries.Select(entry => new TextEntry
            {
                Name = entry.Name,
                EditedText = entry.EditedText,
                OriginalText = entry.OriginalText.Length == 0 ? entry.EditedText : entry.OriginalText,
                MaxLength = entry.MaxLength
            });

            var kfi = new KoreFileInfo { Adapter = kup };

            var sfd = new SaveFileDialog { FileName = editor.KoreFile.FileInfo.Name + kfi.Extension, Filter = kfi.Filter };
            if (sfd.ShowDialog() != true) return;

            kup.Save(sfd.FileName);
        }

        // Tabs
        public void TabChanged(SelectionChangedEventArgs args)
        {
            NotifyOfPropertyChange(() => TextEditorToolsVisible);
        }

        public void CloseTab(IScreen tab)
        {
            tab.TryClose();
            switch (tab)
            {
                case IFileEditor editor:
                    _kore.CloseFile(editor.KoreFile);
                    break;
            }
        }

        public void CloseAllTabs()
        {
            for (var i = Items.Count - 1; i >= 0; i--)
                CloseTab(Items[i]);
        }

        #region Private Methods

        private void LoadFile(string filename)
        {
            KoreFileInfo kfi = null;

            try
            {
                kfi = _kore.LoadFile(filename);
            }
            catch (LoadFileException ex)
            {
                MessageBox.Show(ex.ToString(), "Open File", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (kfi == null) return;

            switch (kfi.Adapter)
            {
                case ITextAdapter txt2:
                    ActivateItem(new TextEditor2ViewModel(kfi));
                    break;
                case IImageAdapter img:
                    ActivateItem(new ImageEditorViewModel(kfi));
                    break;
                case IFontAdapter fnt:
                    ActivateItem(new FontEditorViewModel(kfi));
                    break;
            }
        }

        private void SaveFile(string filename = "")
        {
            (ActiveItem as IFileEditor)?.Save(filename);
        }

        #endregion
    }
}
