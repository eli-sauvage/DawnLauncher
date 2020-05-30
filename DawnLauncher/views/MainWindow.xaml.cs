using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Input;
using System.Diagnostics;
using Newtonsoft.Json;
using System.IO;

namespace DawnLauncher
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DL dl = new DL();
        private string currentGameVersion = null;
        public MainWindow(string gameVersion)
        {
            InitializeComponent();
            Console.WriteLine("debut");
            dynamic test = JsonConvert.DeserializeObject("{'test':null,'nombre':64}");
            currentGameVersion = gameVersion;
            //((App)App.Current).settings.gameDir = "E:\\Game\\WindowsNoEditor";
            if(gameVersion != ((App)App.Current).settings.gameVersion)
            {
                dlBtn.Content = "Download update";
            }
            browser.Navigate(new Uri("http://google.com/", UriKind.RelativeOrAbsolute), string.Empty, null, string.Format("User-Agent: {0}", "Opera/9.80 (J2ME/MIDP; Opera Mini/9 (Compatible; MSIE:9.0; iPhone; BlackBerry9700; AppleWebKit/24.746; U; en) Presto/2.5.25 Version/10.54"));
            progressBar.Visibility = Visibility.Hidden;
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // test.Text = click++.ToString();
            //Console.WriteLine("click");
        }
        private void btnDlEvent(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("dl");
            if((string)dlBtn.Content == "Play")
            {
                Console.WriteLine("PLAY");
                string file = ((App)App.Current).settings.gameDir + "\\" + ((App)App.Current).gameName;
                if (!File.Exists(file))
                {
                    MessageBox.Show("jeu non trouvé/le jeu doit s'appeller" + ((App)App.Current).gameName);
                    ((App)App.Current).settings.gameDir = null;
                    dlBtn.Content = "Download update";
                }
                else
                {
                    this.Visibility = Visibility.Hidden;
                    Process.Start(file);
                    Application.Current.Shutdown();
                }

            }
            else
            {
                ((App)App.Current).settings.gameDir = dl.startDl(dlBtn, progressBar, currentGameVersion);
            }
        }

        private void btnAboutEvent(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("about Clicked");
            Process.Start("https://github.com/elicolh");

        }

        private void btnQuitEvent(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            this.DragMove();
        }
        private void btnMinimizeEvent(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void optionsBtnEvent(object sender, RoutedEventArgs e)
        {
            browser.Visibility = (browser.Visibility == Visibility.Visible) ? Visibility.Hidden : Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((App)App.Current).settings.resetSettings();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ((App)App.Current).settings.gameVersion = null;
            dlBtn.Content = "Download update";
        }

        private void setPathEvent(object sender, RoutedEventArgs e)
        {
            setNewPath setnewpath = new setNewPath();
            _ = setnewpath.newPath() ? dlBtn.Content = "Play" : null;
        }
    }
}
