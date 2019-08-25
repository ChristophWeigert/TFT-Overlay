using System;
using System.ComponentModel;
using System.Configuration;

namespace TFT_Overlay.Properties
{

    /// <summary>
    /// This class allows you to handle specific events on the settings class:
    /// The SettingChanging event is raised before a setting's value is changed.
    /// The PropertyChanged event is raised after a setting's value is changed.
    /// The SettingsLoaded event is raised after the setting values are loaded.
    /// The SettingsSaving event is raised before the setting values are saved.
    /// </summary>
    /// <seealso cref="ApplicationSettingsBase" />
    public sealed partial class Settings
    {
        /// <summary>
        /// Finds the and update.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void FindAndUpdate<T>(string key, T value)
        {
            try
            {
                Default[key] = value;
                Default.Save();
            }
            catch (SettingsPropertyNotFoundException ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void SettingChangingEventHandler(object sender, SettingChangingEventArgs e)
        {
            // Add code to handle the SettingChangingEvent event here.
        }

        private void SettingsSavingEventHandler(object sender, CancelEventArgs e)
        {
            // Add code to handle the SettingsSaving event here.
        }
    }
}