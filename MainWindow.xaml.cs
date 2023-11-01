using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Grid_view_Engine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnStartProcess(object sender, RoutedEventArgs e)
        {
            OutputBox.Text += "Starting the process...\n";

            string directory = @"F:\Software_develop\image";
            string fileName = "test.png";
            string imagePath = Path.Combine(directory, fileName);

            OutputBox.Text += "Getting The Game List\n";
            string steamPath;
            SortedDictionary<string, string> apps;
            GetSteamInfo(out steamPath, out apps);

            if (steamPath == null)
            {
                OutputBox.Text += "Could not automatically detect Steam installation path. Please enter it manually:\n";
                // You can use an input dialog or a TextBox for this.
                steamPath = InputSteamPathBox.Text; // Assuming you have a TextBox named InputSteamPathBox for manual input
            }

            OutputBox.Text += "Steam installation path: " + steamPath + "\n";

            GamesList.Items.Clear();
            foreach (var app in apps)
            {
                GamesList.Items.Add(app.Key);
            }

            // The rest of the selection process can be handled in another event, e.g., GamesList_SelectionChanged
        }

        private void GamesList_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (GamesList.SelectedItem != null)
            {
                string appName = GamesList.SelectedItem.ToString();
                SortedDictionary<string, string> apps;
                GetSteamInfo(out _, out apps);
                string appId = apps[appName];

                string steamID = "896533048"; // Replace with your SteamID

                OutputBox.Text += "We are trying to get the Steam Path\n";
                ReplaceImage(steamID, appId, "YOUR_IMAGE_PATH_HERE", "YOUR_STEAM_PATH_HERE");
            }
        }


        static void GetSteamInfo(out string steamPath, out SortedDictionary<string, string> apps)
        {
            steamPath = null;
            apps = new SortedDictionary<string, string>();

            string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key))
            {
                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        if (subkey_name.StartsWith("Steam App"))
                        {
                            object appNameObject = subkey.GetValue("DisplayName");
                            object installLocationObject = subkey.GetValue("InstallLocation");

                            if (appNameObject == null || installLocationObject == null)
                            {
                                continue;
                            }

                            string appName = (string)appNameObject;
                            string installLocation = (string)installLocationObject;
                            string appId = subkey_name.Substring(10); // Remove "Steam App " prefix

                            apps[appName] = appId;

                            if (steamPath == null)
                            {
                                // Extract the Steam path from the install location
                                steamPath = installLocation.Substring(
                                    0,
                                    installLocation.IndexOf("\\steamapps")
                                );
                            }
                        }
                    }
                }
            }
        }

        static void ReplaceImage(string steamID, string appID, string imagePath, string steamPath)
        {
            string steamDirectory = Path.Combine(steamPath, "userdata", steamID, "config", "grid");
            string newImageFilename = Path.Combine(steamDirectory, $"{appID}p.png");

            try
            {
                File.Copy(imagePath, newImageFilename, true);
                Console.WriteLine($"Successfully replaced grid view image for app ID {appID}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        } // ... Your GetSteamInfo and ReplaceImage methods here ...

        private void OutputBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}