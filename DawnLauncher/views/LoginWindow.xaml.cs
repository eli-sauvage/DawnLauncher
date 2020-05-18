using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Threading;
using System.Windows.Threading;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics;

namespace DawnLauncher
{
    /// <summary>
    /// Logique d'interaction pour LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            WarningMessage.Visibility = Visibility.Hidden;
            pseudoBox.Text = ((App)App.Current).settings.savedLogin;
            if(pseudoBox.Text != null && pseudoBox.Text != "")
            {
                saveLoginCheckbox.IsChecked = true;
                mdpBox.Focus();
            }
            else
            {
                pseudoBox.Focus();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            try
            {
                this.DragMove();

            }
            catch
            {

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            initLogin();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                initLogin();
        }


        private void initLogin()
        {
            // Create a request using a URL that can receive a post.
            WebRequest request = WebRequest.Create("http://localhost:1234");
            // Set the Method property of the request to POST.
            request.Method = "POST";

            // Create POST data and convert it to a byte array.
            JsonConstruct toSend = new JsonConstruct
            {
                pseudo = pseudoBox.Text,
                mdp = mdpBox.Password,
            };
            string postData = JsonConvert.SerializeObject(toSend);
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/json";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            try
            {
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();

                // Get the response.
                WebResponse response = request.GetResponse();
                // Display the status.
                Console.WriteLine("response status : " + ((HttpWebResponse)response).StatusDescription);
                // Get the stream containing content returned by the server.
                // The using block ensures the stream is automatically closed.
                using (dataStream = response.GetResponseStream())
                {
                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.
                    string responseFromServer = reader.ReadToEnd();
                    // Display the content.
                    Console.WriteLine(responseFromServer);
                    dynamic res = JsonConvert.DeserializeObject(responseFromServer);
                    Console.WriteLine(res.ok);
                    if ((bool)res.ok)
                    {
                        if ((bool)saveLoginCheckbox.IsChecked)
                        {
                            ((App)App.Current).settings.savedLogin = pseudoBox.Text;
                        }
                        else
                        {
                            ((App)App.Current).settings.savedLogin = null;
                        }
                        MainWindow main = new MainWindow((string)res.gameVersion);
                        main.Show();
                        this.Close();
                    }
                    else
                    {
                        mdpBox.Password = "";
                        mdpBox.Focus();
                        WarningMessage.Visibility = Visibility.Visible;
                        saveLoginCheckbox.Visibility = Visibility.Hidden;
                        saveLoginLabel.Visibility = Visibility.Hidden;
                        Task.Run(async delegate
                        {
                            await Task.Delay(1500);
                            Dispatcher.Invoke(DispatcherPriority.Normal, new Action<TextBlock>(hideTxtBlok), WarningMessage);
                            Dispatcher.Invoke(DispatcherPriority.Normal, new Action<Label>(makeLabelVisible), saveLoginLabel);
                            Dispatcher.Invoke(DispatcherPriority.Normal, new Action<CheckBox>(makeCheckboxVisible), saveLoginCheckbox);
                            return;
                        });
                    }
                }

                // Close the response.
                response.Close();
            }
            catch
            {
                MessageBox.Show("impossible de joindre le serveur.");
            }

        }
        #region //////////////////////////////toggle visibility functions def
        private void hideTxtBlok(TextBlock txt)
        {
            txt.Visibility = Visibility.Hidden;
        }
        private void makeCheckboxVisible(CheckBox chbx)
        {
            chbx.Visibility = Visibility.Visible;
        }
        private void makeLabelVisible(Label label)
        {
            label.Visibility = Visibility.Visible;
        }
        #endregion
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {//creer comtpe
            Process.Start("https://youtube.com");
        }

        private void TextBlock_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {//mdp oublié
            Process.Start("https://twitch.tv");
        }
    }
    class JsonConstruct
    {
        public string pseudo { get; set; }
        public string mdp { get; set; }
    }
}
