using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using TFT_Overlay.Properties;
using TFT_Overlay.Utilities;
using Application = System.Windows.Forms.Application;
using Timer = System.Timers.Timer;

namespace TFT_Overlay
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Cursor loLHover = CustomCursor.FromByteArray(Properties.Resources.LoLHover);
        private readonly Cursor loLNormal = CustomCursor.FromByteArray(Properties.Resources.LoLNormal);
        private readonly Cursor loLPointer = CustomCursor.FromByteArray(Properties.Resources.LoLPointer);
        private readonly Timer tTop;
        /// <summary>
        /// My handle
        /// </summary>
        public IntPtr myHandle;

        private bool canDrag;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.LoadStringResource(Settings.Default.Language);
            this.Cursor = this.loLNormal;

            this.WindowState = WindowState.Normal;
            this.ShowInTaskbar = true;
            this.Topmost = this.OnTop;
            this.myHandle = new WindowInteropHelper(this).Handle;

            this.tTop = new Timer(15000);
            this.tTop.Elapsed += this.theout;
            this.tTop.AutoReset = true;
            this.tTop.Enabled = true;

            this.CanDrag = !Settings.Default.Lock;

            if (Settings.Default.AutoDim)
            {
                Thread t = new Thread(this.AutoDim)
                {
                    IsBackground = true
                };

                t.Start();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can drag.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can drag; otherwise, <c>false</c>.
        /// </value>
        public bool CanDrag
        {
            get => this.canDrag;
            set
            {
                this.canDrag = value;
                Settings.FindAndUpdate("Lock", !value);
            }
        }

        private string CurrentVersion { get; } = Utilities.Version.version;
        private bool OnTop { get; set; } = true;

        /// <summary>
        /// Gets the foreground window.
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Sets the foreground window.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// Theouts the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        public void theout(object source, ElapsedEventArgs e)
        {
            if (this.OnTop)
            {
                if (this.myHandle != GetForegroundWindow()) //let the win always on focus
                {
                    SetForegroundWindow(this.myHandle);
                }

                try
                {
                    this.Dispatcher.Invoke(
                        delegate
                        {
                            this.Topmost = false;
                            this.Topmost = true;
                        }
                    );
                }
                catch (TaskCanceledException)
                {
                    this.tTop.Stop();
                }
            }
        }

        private void AutoDim()
        {
            while (true)
            {
                this.Dispatcher.BeginInvoke(this.IsLeagueOrOverlayActive()
                    ? new ThreadStart(() => System.Windows.Application.Current.MainWindow.Opacity = 1)
                    : new ThreadStart(() => System.Windows.Application.Current.MainWindow.Opacity = .2));

                Thread.Sleep(500);
            }
        }

        private void AutoDim_Click(object sender, RoutedEventArgs e)
        {
            string state = Settings.Default.AutoDim ? "OFF" : "ON";

            MessageBoxResult result = MessageBox.Show($"Would you like to turn {state} Auto-Dim? This will restart the program.", "Auto-Dim", MessageBoxButton.OKCancel);

            if (result != MessageBoxResult.OK)
            {
                return;
            }

            Settings.FindAndUpdate("AutoDim", !Settings.Default.AutoDim);

            Application.Restart();
            System.Windows.Application.Current.Shutdown();
        }

        private void IconOpacityHandler_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem menuItem))
            {
                return;
            }

            string header = menuItem.Header.ToString();
            double opacity = double.Parse(header.Substring(0, header.Length - 1)) / 100;

            Settings.FindAndUpdate("IconOpacity", opacity);
        }

        private bool IsLeagueOrOverlayActive()
        {
            string currentActiveProcessName = ProcessHelper.GetActiveProcessName();
            return currentActiveProcessName.Contains("League of Legends") || currentActiveProcessName.Contains("TFT Overlay");
        }

        /// <summary>
        ///     Removes previous ItemStrings.xaml from MergedDictionaries and adds the one matching locale tag
        /// </summary>
        /// <param name="locale">localization tag</param>
        private void LoadStringResource(string locale)
        {
            try
            {
                ResourceDictionary resources = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/Resource/Localization/ItemStrings_" + locale + ".xaml", UriKind.Absolute)
                };


                ResourceDictionary current = System.Windows.Application.Current.Resources.MergedDictionaries.FirstOrDefault(
                    m => m.Source.OriginalString.EndsWith("ItemStrings_" + locale + ".xaml"));

                if (current != null)
                {
                    System.Windows.Application.Current.Resources.MergedDictionaries.Remove(current);
                }

                System.Windows.Application.Current.Resources.MergedDictionaries.Add(resources);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                this.LoadStringResource("en-US");
            }
        }

        private void Localization_Credits(object sender, RoutedEventArgs e)
        {
            StringBuilder msgBoxText = new StringBuilder();
            msgBoxText.AppendLine("de-DE: Revyn#0969");
            msgBoxText.AppendLine("es-AR: Oscarinom");
            msgBoxText.AppendLine("es-MX: Jukai#3434");
            msgBoxText.AppendLine("fr-FR: Darkneight");
            msgBoxText.AppendLine("HU: Edizone#6157");
            msgBoxText.AppendLine("it-IT: BlackTYWhite#0943");
            msgBoxText.AppendLine("JA: つかぽん＠PKMotion#8731");
            msgBoxText.AppendLine("PL: Czapson#9774");
            msgBoxText.AppendLine("pt-BR: Bigg#4019");
            msgBoxText.AppendLine("RU: Jeremy Buttson#2586");
            msgBoxText.AppendLine("SL: Shokugeki#0012");
            msgBoxText.AppendLine("vi-VN: GodV759");
            msgBoxText.AppendLine("zh-CN: nevex#4441");
            msgBoxText.AppendLine("zh-TW: noheart#6977");

            MessageBox.Show(msgBoxText.ToString(), "Localization Credits");
        }

        /// <summary>
        ///     Takes MenuItem, and passes its Header into LoadStringResource()
        /// </summary>
        /// <param name="sender">Should be of type MenuItem</param>
        /// <param name="e"></param>
        private void Localization_Handler(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem menuItem))
            {
                return;
            }

            string tag = menuItem.Header.ToString();
            this.LoadStringResource(tag);

            Settings.FindAndUpdate("Language", tag);
        }

        private void LocalizationHelp_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/Just2good/TFT-Overlay/blob/master/Localization.md");
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            Settings.Default.Save();
        }

        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((Control)sender).Cursor = this.loLPointer;
            if (this.CanDrag)
            {
                this.DragMove();
            }
        }

        private void MainWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((Control)sender).Cursor = this.loLNormal;
        }

        private void MainWindow_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((Control)sender).Cursor = this.loLHover;
        }

        private void MainWindow_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((Control)sender).Cursor = this.loLNormal;
        }

        private void MenuItem_AutoUpdate(object sender, RoutedEventArgs e)
        {
            string state = Settings.Default.AutoUpdate ? "OFF" : "ON";

            MessageBoxResult result = MessageBox.Show($"Would you like to turn {state} Auto-Update? This will restart the program.", "Auto-Updater", MessageBoxButton.OKCancel);

            if (result != MessageBoxResult.OK)
            {
                return;
            }

            Settings.FindAndUpdate("AutoUpdate", !Settings.Default.AutoUpdate);

            Application.Restart();
            System.Windows.Application.Current.Shutdown();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void MenuItem_Click_About(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "TFT Information Overlay V" + this.CurrentVersion + " by J2GKaze/Jinsoku#4019\n\nDM me on Discord if you have any questions\n\nLast Updated: August 11th, 2019 @ 2:06AM PDT", "About");
        }

        private void MenuItem_Click_Credits(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Big thanks to:\nChaoticoz: Lock Window, Always on Top, and Mouseover\nAsemco/Asemco#7390: Adding Origins and Classes\nAthenyx#9406: Designs\nTenebris: Auto-Updater\nOBJECT#3031: Items/Origins/Classes Strings Base\nJpgdev: Readme format\nKbphan\nEerilai\nꙅꙅɘᴎTqAbɘbᴎɘld#1175: Window Position/Size Saving, CPU Threading Fix\nNarcolic#6374: Item Builder\nIzoyo: Fullscreen\n\nShoutout to:\nAlexander321#7153 for the Discord Nitro Gift!\nAnonymous for Reddit Gold\nu/test01011 for Reddit Gold\n\nmac#0001 & bNatural#0001(Feel free to bug these 2 on Discord) ;)\nShamish#4895 (Make sure you bug this guy a lot)\nDekinosai#7053 (Buy this man tons of drinks)",
                "Credits");
        }

        private void MenuItem_Click_Lock(object sender, RoutedEventArgs e)
        {
            this.CanDrag = !this.CanDrag;
        }

        private void MenuItem_Click_OnTop(object sender, RoutedEventArgs e)
        {
            if (this.OnTop)
            {
                this.Topmost = false;
                this.OnTop = false;
            }
            else
            {
                this.Topmost = true;
                this.OnTop = true;
            }
        }

        private void OpenChangelog_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/Just2good/TFT-Overlay/blob/master/README.md#version-history");
        }

        private void ResetToDefault_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Reset();
            this.CanDrag = true;
            this.LoadStringResource(Settings.Default.Language);
        }
    }
}