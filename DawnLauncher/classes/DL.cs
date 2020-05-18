using System;
using System.Net;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using WinForms = System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Reflection;
using System.Diagnostics;
using form = System.Windows.Forms;
using DawnLauncher;

public class DL
{
	Button btn = null;
	ProgressBar progressBar = null;
	string newGameVersion = null;
	public DL()
	{
	}
	public string startDl(Button dl,ProgressBar progress, string newGameversionInput)
	{
		newGameVersion = newGameversionInput;
		var dlg = new CommonOpenFileDialog();
		dlg.Title = "My Title";
		dlg.IsFolderPicker = true;
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
			var folder = dlg.FileName;
			Console.WriteLine(folder);
			btn = dl;
			progressBar = progress;
			progressBar.Visibility = Visibility.Visible;
			Console.WriteLine("DL");
			if (!IsDirectoryWritable(folder))
			{
				form.DialogResult choice = (form.DialogResult)MessageBox.Show("pour installer ici, il faut les droits admin et l'application doit redémarrer", "permissions", MessageBoxButton.OKCancel);
				if (choice == form.DialogResult.OK)
				{
					reqAdmin();
				}
				else if (choice == form.DialogResult.Cancel)
				{
					return null;
				}
				return null;
			}
			WebClient webClient = new WebClient();
			webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
			webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
			string gros = "https://download.visualstudio.microsoft.com/download/pr/d8cf1fe3-21c2-4baf-988f-f0152996135e/0c00b94713ee93e7ad5b4f82e2b86607/windowsdesktop-runtime-3.1.4-win-x64.exe";
			string petit = "https://github.com/debauchee/barrier/releases/download/v2.3.2/BarrierSetup-2.3.2.exe";
			webClient.DownloadFileAsync(new Uri(petit), folder + "\\" + ((App)App.Current).gameName);
			return folder;
		}
		return null;

	}
	private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
	{
		double percent = (double)e.BytesReceived / e.TotalBytesToReceive;
		percent *= 100;
		progressBar.Value = percent;
		btn.Content = "Downloading...";
	}

	private void Completed(object sender, AsyncCompletedEventArgs e)
	{
		if (e.Error != null)
		{
			Console.WriteLine("admin req");
			Console.WriteLine(e.Error.ToString());
			MessageBox.Show(e.Error.ToString(), e.Error.Message);
			return;
		}
		Console.WriteLine("DONE");
		((App)App.Current).settings.gameVersion = newGameVersion;
		progressBar.Visibility = Visibility.Hidden;
		btn.Content = "Play";
	}
	private bool CheckFolderPermission(string folderPath)
	{
		DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
		try
		{
			DirectorySecurity dirAC = dirInfo.GetAccessControl(AccessControlSections.All);
			return true;
		}
		catch (PrivilegeNotHeldException)
		{
			return false;
		}
	}
	private void reqAdmin()
	{
		WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
		bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);
		if (!hasAdministrativeRight)
		{
			// relaunch the application with admin rights
			string fileName = Assembly.GetExecutingAssembly().Location;
			ProcessStartInfo processInfo = new ProcessStartInfo();
			processInfo.Verb = "runas";
			processInfo.FileName = fileName;

			try
			{
				Process.Start(processInfo);
				System.Windows.Application.Current.Shutdown();
			}
			catch (Win32Exception)
			{
				// This will be thrown if the user cancels the prompt
			}

			return;
		}
	}
	public bool IsDirectoryWritable(string dirPath, bool throwIfFails = false)
	{
		try
		{
			using (FileStream fs = File.Create(
				Path.Combine(
					dirPath,
					Path.GetRandomFileName()
				),
				1,
				FileOptions.DeleteOnClose)
			)
			{ }
			return true;
		}
		catch
		{
			if (throwIfFails)
				throw;
			else
				return false;
		}
	}
}
