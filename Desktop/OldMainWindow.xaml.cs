/*
Use instructions:
    1. Download and install Visual Studio 2022 and Python.
    2. Install .NET desktop development from Visual Studio Installer.
    3. Create a new WPF Application project.
    4. Install Microsoft.Web.WebView2 from NuGet Package Manager.
    5. Paste the code from MainWindow.xaml, MainWindow.xaml.cs to the project.
    6. Put the website files in the build path.
    7. Build the project.
    8. Start the application.

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
        int Id2;
        int shared;
        private WindowStyle _currentStyle = WindowStyle.SingleBorderWindow;

        Grid grid = new Grid();
        TextBlock text = new TextBlock();
        TextBlock subtitle = new TextBlock();
        WebView2 wv = new WebView2();
        public MainWindow()
        {
            InitializeComponent();
            Random port = new Random();
            int stport = port.Next(1000, 65535);
            shared = stport;
            try
            {
                Process Python = new Process();
                Python.StartInfo.FileName = "python.exe";
                Python.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Python.Start();
                Id2 = Python.Id;
                Python.Kill();
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
            }
            catch
            {
                text.Text = "Python is required";
                text.Cursor = Cursors.Hand;
                text.TextDecorations = TextDecorations.Underline;
                text.FontSize = 72;
                text.PreviewMouseDown += Text_PreviewKeyDown;
                text.TextAlignment = System.Windows.TextAlignment.Center;
            }
            grid.Children.Add(text);
            this.KeyDown += MainWindow_KeyDown;
            this.Closing += MainWindow_OnClosing;
            Process proc = new Process();
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.Arguments = $"start C:\Windows\127.0.0.1\127.0.0.1.exe";
            proc.Start();
            Id = proc.Id;
            wv.Source = new System.Uri($"http://127.0.0.1:{stport}/index.html");
            this.Content = grid;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11)
            {
                _currentStyle = _currentStyle == WindowStyle.None ? WindowStyle.SingleBorderWindow : WindowStyle.None;
                if (_currentStyle == WindowStyle.None)
                {
                    this.WindowState = WindowState.Maximized;
                }
                else
                {
                    this.WindowState = WindowState.Normal;
                }
            }

            if (e.Key == Key.Escape && _currentStyle == WindowStyle.None)
            {
                this.WindowState = WindowState.Normal;
                _currentStyle = WindowStyle.SingleBorderWindow;
            }

            this.WindowStyle = _currentStyle;

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
                if (process.Id == Id || process.Id == Id2)
                {
                    process.Kill();
                }
            }
        }

        private void Text_PreviewKeyDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("explorer.exe", "https://www.python.org");
        }
    }
}
