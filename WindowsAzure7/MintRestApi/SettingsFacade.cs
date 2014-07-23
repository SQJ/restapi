using System;
using System.Configuration;
using System.Globalization;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace MintRestApi
{
    /// <summary>
    /// Exposes methods that can be used to interact with basic application configuration settings.
    /// </summary>
    /// <remarks>
    /// <para>The methods exposed in this facade can be used to interact with simple name-value pair configuration
    /// settings.  This class does not deal with complex or custom .Net configuration.</para>
    /// <para>The general pattern for this facade is to define the names as <c>public const string</c> for individual settings that your assembly 
    /// needs to access within a class in your assembly.  Then, call into the appropriate setting retrieval method in this facade.</para>
    /// </remarks>
    public static class SettingsFacade
    {
        /// <summary>
        /// Does the work for retrieval of the setting defined by <paramref name="key"/> from the application's configuration.
        /// </summary>
        /// <remarks>
        /// <para>Determines if we are working in Azure.  If we are, then this method will attempt to retrieve the 
        /// configuration setting from the Azure cscfg settings.  If the application is not running in Azure, then this method will 
        /// look at the AppSettings section of the application's default configuration (*.config).</para>
        /// </remarks>
        /// <returns>The configuration setting located by <paramref name="key"/>.  If the setting defined by <paramref name="key"/> cannot
        /// be found, then this method will return null.</returns>
        /// <exception cref="SettingException">thrown when the underlying configuration handlers throw an exception.</exception>
        /// <seealso cref="RoleEnvironment.GetConfigurationSettingValue"/>
        /// <seealso cref="ConfigurationManager.AppSettings" />
        private static string GetSettingValue(string key)
        {
            string retVal = null;

            try
            {
                if (RoleEnvironment.IsAvailable)
                {
                    try
                    {
                        retVal = RoleEnvironment.GetConfigurationSettingValue(key);
                    }
                    catch (RoleEnvironmentException)
                    {
                        retVal = null;
                    }
                }

                return retVal ?? (ConfigurationManager.AppSettings[key]);
            }
            catch (Exception)
            {
                //throw new SettingException(key, ex);
            }

            return retVal ?? (ConfigurationManager.AppSettings[key]);
        }

        /// <summary>
        /// Gets a configuration setting as a string found using the setting id of <paramref name="key"/>
        /// </summary>
        /// <returns>The configuration setting found at <paramref name="key"/></returns>
        /// <remarks><para>This method will first look in the Azure service config (cscfg), if we are running in Azure.  If we are not running in Azure,
        /// or if the configuration setting is not available in Azure, then we will look in the AppSettings section of the applications default configuration
        /// (defined in the app's *.config file).</para>
        /// <para>If the setting defined by <paramref name="key"/> cannot be found at all, this method will throw an exception.</para></remarks>
        /// <exception cref="SettingException">thrown if the value cannot be found in any of the configuration settings for the application.  For details 
        /// on what the underlying problem was, see the <see cref="Exception.InnerException"/>.</exception>
        public static string GetString(string key)
        {
            return GetSettingValue(key);
        }

        /// <summary>
        /// Used to get an <see cref="Int32"/> from the application configuration.
        /// </summary>
        /// <param name="key">The application configuration setting to retrieve</param>
        /// <returns>An integer parsed from the configuration setting found at <paramref name="key"/></returns>
        /// <exception cref="SettingException">thrown in the event that there is a problem retrieving the value from the configuration or if there is a problem
        /// parsing the value.  For details on what the underlying problem was, see the <see cref="Exception.InnerException"/>.</exception>
        public static int GetInt32(string key)
        {
            try
            {
                return Int32.Parse(GetSettingValue(key), NumberStyles.Integer, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new SettingException("There was a problem parsing an Int32 from the application setting", key, ex);
            }
        }

        /// <summary>
        /// Used to get an <see cref="Int64"/> from the application configuration.
        /// </summary>
        /// <param name="key">The application configuration setting to retrieve</param>
        /// <returns>An Int64 parsed from the configuration setting found at <paramref name="key"/></returns>
        /// <exception cref="SettingException">thrown in the event that there is a problem retrieving the value from the configuration or if there is a problem
        /// parsing the value.  For details on what the underlying problem was, see the <see cref="Exception.InnerException"/>.</exception>
        public static long GetInt64(string key)
        {
            try
            {
                return Int64.Parse(GetSettingValue(key), NumberStyles.Integer, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new SettingException("There was a problem parsing an Int64 from the application setting", key, ex);
            }
        }

        /// <summary>
        /// Used to get an <see cref="bool"/> from the application configuration.
        /// </summary>
        /// <param name="key">The application configuration setting to retrieve</param>
        /// <returns>A bool parsed from the configuration setting found at <paramref name="key"/></returns>
        /// <exception cref="SettingException">thrown in the event that there is a problem retrieving the value from the configuration or if there is a problem
        /// parsing the value.  For details on what the underlying problem was, see the <see cref="Exception.InnerException"/>.</exception>
        public static bool GetBoolean(string key)
        {
            try
            {
                return Boolean.Parse(GetSettingValue(key));
            }
            catch (Exception ex)
            {
                throw new SettingException("There was a problem parsing a boolean from the application setting", key, ex);
            }
        }

        /// <summary>
        /// Used to get a <see cref="Type"/> from the configuration file based on the string representation of the type's name.
        /// </summary>
        /// <param name="key">The configured property name</param>
        /// <returns>A <see cref="Type"/> based on the type's name found in the configuration file at the 
        /// <paramref name="key"/>.  If the setting does not exist then this will return null.</returns>
        /// <exception cref="SettingException">thrown when the string configured at <paramref name="key"/> is non-null, but 
        /// is not a valid type name.</exception>
        public static Type GetType(string key)
        {
            try
            {
                var typeName = GetSettingValue(key);
                return (null == typeName)
                        ? null
                        : Type.GetType(typeName);
            }
            catch (Exception ex)
            {
                var message = String.Format(CultureInfo.InvariantCulture, "The configuration setting {0} is not a valid type name.", key);
                throw new SettingException(message, key, ex);
            }
        }

        /// <summary>
        /// Used to get a <see cref="double"/> from the application configuration.
        /// </summary>
        /// <param name="key">The application configuration setting to retrieve</param>
        /// <returns>A double parsed from the configuration setting found at <paramref name="key"/></returns>
        /// <exception cref="SettingException">thrown in the event that there is a problem retrieving the value from the configuration or if there is a problem
        /// parsing the value.  For details on what the underlying problem was, see the <see cref="Exception.InnerException"/>.</exception>
        public static double GetDouble(string key)
        {
            try
            {
                return double.Parse(GetSettingValue(key));
            }
            catch (Exception ex)
            {
                throw new SettingException("There was a problem parsing a double from the application setting", key, ex);
            }
        }

        /// <summary>
        /// Used to get an Enum value of type T
        /// </summary>
        /// <typeparam name="T">An enumeration type</typeparam>
        /// <param name="key">The application configuration setting to retrieve</param>
        /// <returns>The parsed value of the configuration setting found using the <paramref name="key"/></returns>
        /// <exception cref="SettingException">thrown in the event that there is a problem retrieving the value from the configuration or if there is a problem
        /// parsing the value.  For details on what the underlying problem was, see the <see cref="Exception.InnerException"/> of the thrown exception.</exception>
        public static T GetEnum<T>(string key) where T : struct
        {
            try
            {
                return GetSettingValue(key).ParseEnum<T>();
            }
            catch (Exception ex)
            {
                var message = String.Format(CultureInfo.InvariantCulture, "There was a problem parsing an {0} from the application setting", typeof(T));
                throw new SettingException(message, key, ex);
            }
        }


        /// <summary>
        /// Parses the enum out of the string..
        /// </summary>
        /// <typeparam name="T">Type of enumeration to parse</typeparam>
        /// <param name="value">The value to parse.</param>
        /// <returns>The value parsed into an enum, if it is valid.  Otherwise, returns the default of the enum.</returns>
        public static T ParseEnum<T>(this string value) where T : struct
        {
            T retVal = Enum.TryParse(value, out retVal)
                        ? retVal
                        : default(T);
            return retVal;
        }
    }

    /// <summary>
    /// Wrapper exception used when configuration helpers experience a problem.
    /// </summary>
    [Serializable]
    public class SettingException : ConfigurationErrorsException
    {
        private const string DefaultMessage = "The setting key does not exist in the application configuration.";

        /// <summary>
        /// Returns the name of the configuration setting that was being retrieved.
        /// </summary>
        public String SettingKey { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingException"/> class.
        /// </summary>
        public SettingException() : this(String.Empty, null, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public SettingException(String message) : this(message, null, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingException"/> class.
        /// </summary>
        /// <param name="settingKey">The setting key.</param>
        /// <param name="inner">The inner exception.</param>
        public SettingException(String settingKey, Exception inner) : this(DefaultMessage, settingKey, inner) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="settingKey">The setting key.</param>
        /// <param name="inner">The inner exception.</param>
        public SettingException(String message, String settingKey, Exception inner)
            : base(message, inner)
        {
            SettingKey = settingKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingException"/> class.
        /// </summary>
        /// <param name="info">The object that holds the information to deserialize.</param>
        /// <param name="context">Contextual information about the source or destination.</param>
        /// <exception cref="T:System.InvalidOperationException">The current type is not a <see cref="T:System.Configuration.ConfigurationException"/> or a <see cref="T:System.Configuration.ConfigurationErrorsException"/>.</exception>
        protected SettingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
