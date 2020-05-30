using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace DawnLauncher
{
    class setNewPath
    {
        public bool newPath()
        {
			var dlg = new CommonOpenFileDialog();
			dlg.Title = "My Title";
			dlg.IsFolderPicker = false;
			dlg.InitialDirectory = null;// ((App)App.Current).settings.gameDir;

			dlg.AddToMostRecentlyUsedList = false;
			dlg.AllowNonFileSystemItems = false;
			dlg.DefaultDirectory = "C:\\";
			dlg.EnsureFileExists = true;
			dlg.EnsurePathExists = true;
			dlg.EnsureReadOnly = false;
			dlg.EnsureValidNames = true;
			dlg.Multiselect = false;
			dlg.ShowPlacesList = true;

			if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
			{
				var file = dlg.FileName;
				if (!file.Contains(".exe"))
				{
					MessageBox.Show("le fichier n'est pas un .exe");
					return false;
				}
				((App)App.Current).settings.gameDir = System.IO.Directory.GetParent(file).ToString();
				return true;
			}
			return false;
		}
    }
}
