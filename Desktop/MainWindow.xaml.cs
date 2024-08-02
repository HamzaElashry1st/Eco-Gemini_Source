/*
Use instructions:
    1. Download and install Visual Studio 2022.
    2. Install .NET desktop development from Visual Studio Installer.
    3. Create a new C# Console Application.
    4. Paste the code from Program.cs to the project.
    5. Build the project.
    6. Create a new WPF Application project.
    7. Install Microsoft.Web.WebView2 from NuGet Package Manager.
    8. Paste the code from MainWindow.xaml, MainWindow.xaml.cs to the project.
    9. Put the website files and the Console Application build files in the build path.
    10. Build the project.
    11. Start the application.

    -- Localhosts are required for cookies --
    -- Cookies and data are saved in next to the build files (Application.exe.WebView2 folder). --
*/

#pragma warning disable
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Eco_Gemini
{
    public partial class MainWindow : Window
    {
        public static bool CheckConnection(int timeout = 10000, string URL = "http://www.gstatic.com/generate_204")
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(URL);

                request.KeepAlive = false;
                request.Timeout = timeout;
                using (var response = (HttpWebResponse)request.GetResponse())
                    return true;
            }
            catch
            {
                return false;
            }
        }

        int Id;
        string shared;
        WindowState before;
        Grid grid = new Grid();
        TextBlock text = new TextBlock();
        TextBlock subtitle = new TextBlock();
        WebView2 wv = new WebView2();
        public MainWindow()
        {
            InitializeComponent();
            bool online = CheckConnection(10000, "http://www.gstatic.com/generate_204");
            if (online)
            {
                subtitle.Text = "Press F11 for full screen.\nPress F5 to return the the main page.";
                subtitle.FontSize = 36;
                subtitle.Foreground = (Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#a0a0a0");
                subtitle.VerticalAlignment = VerticalAlignment.Bottom;
                subtitle.TextAlignment = TextAlignment.Center;
                grid.Children.Add(subtitle);
                grid.Children.Add(wv);
                wv.CoreWebView2InitializationCompleted += OnCoreWebView2Ready;
            }
            else
            {
                subtitle.Text = "No internet connection.";
                subtitle.FontSize = 36;
                subtitle.TextAlignment = TextAlignment.Center;
                grid.Children.Add(subtitle);
            }
            grid.Children.Add(text);
            this.KeyDown += MainWindow_KeyDown;
            this.Closing += MainWindow_OnClosing;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Process process = new Process();
            string prev = System.IO.File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Localhoster\\127.0.0.1.port");
            process.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "\\127.0.0.1.exe";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            string stport;
            while ((prev == System.IO.File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Localhoster\\127.0.0.1.port")))
            {
                Thread.Sleep(100);
            }
            stport = System.IO.File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Localhoster\\127.0.0.1.port");
            shared = stport;
            Id = process.Id;
            wv.Source = new System.Uri($"http://127.0.0.1:{stport}/index.html");
            this.Content = grid;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11)
            {
                if (this.WindowStyle == WindowStyle.None)
                {
                    this.WindowState = before;
                    this.WindowStyle = WindowStyle.SingleBorderWindow;
                }
                else
                {
                    before = this.WindowState;
                    this.WindowState = WindowState.Normal;
                    this.WindowState = WindowState.Maximized;
                    this.WindowStyle = WindowStyle.None;
                }
            }

            if (e.Key == Key.Escape && this.WindowStyle == WindowStyle.None)
            {
                this.WindowState = WindowState.Normal;
                this.WindowStyle = WindowStyle.SingleBorderWindow;
            }

            if (e.Key == Key.F5)
            {
                wv.Source = new Uri($"http://127.0.0.1:{shared}/index.html");
            }
        }

        private void OnCoreWebView2Ready(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (wv.CoreWebView2 != null)
            {
                wv.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                wv.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
                wv.CoreWebView2.Settings.AreDevToolsEnabled = false;
                wv.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
                wv.CoreWebView2.Settings.IsBuiltInErrorPageEnabled = false;
                wv.CoreWebView2.Settings.IsPasswordAutosaveEnabled = false;
                wv.CoreWebView2.Settings.IsStatusBarEnabled = false;
                wv.CoreWebView2.Settings.IsZoomControlEnabled = false;
            }
        }

        private void MainWindow_OnClosing(object sender, EventArgs e)
        {
            Process[] All = Process.GetProcesses();
            foreach (Process process in All)
            {
                if (process.Id == Id)
                {
                    try
                    {
                        process.Kill();
                    }
                    catch
                    {
                        process.CloseMainWindow();
                    }
                }
            }
        }
    }
}