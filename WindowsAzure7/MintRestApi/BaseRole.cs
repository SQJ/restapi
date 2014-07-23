using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.Diagnostics.Management;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.IO;
using System.Text;
using Microsoft.WindowsAzure.Storage;

namespace MintRestApi
{
    /// <summary>
    /// A base class for all web and worker roles. Contains
    /// Initializes Azure Diagnostics configuration.
    /// </summary>
    /// <remarks>
    /// <para>Web roles should derive from <see cref="BaseWebRole"/></para>
    /// <para>This class can leverage several configuration settings on your cscfg file. The following 
    /// default configuration will expose all of the consumed values:
    /// <code>
    ///<ConfigurationSettings>
    ///  <Setting name="DiagnosticMonitorEnabled" value="true"/>
    ///  <!-- If not provided, the app will use a default of 1 minute -->
    ///  <Setting name="DiagTransferPeriodMinutes" value="5"/>
    ///  <!-- If not provided, the app will use a default of LogLevel.Error -->
    ///  <Setting name="DiagWindowsEventLogLevel" value="Error"/>
    ///  <!-- If not provided, the app will use a default of LogLevel.Error -->
    ///  <Setting name="DiagLogsLogLevel" value="Error"/>
    ///  <!-- If not provided, the app will use a default of LogLevel.Error -->
    ///  <Setting name="DiagInfrastuctureLogLevel" value="Error"/>
    ///  <!-- If not provided, the app will use a default of 10 seconds -->
    ///  <Setting name="DiagPerfSampleRateSeconds" value ="10"/>
    ///</ConfigurationSettings>
    /// </code>
    /// </para>
    /// </remarks>
    public class BaseRole : RoleEntryPoint
    {
        private const string DiagnosticsConnectionStringSetting = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";

        /// <summary>
        /// </summary>
        public override bool OnStart()
        {
            StartDiagnostics();

            return base.OnStart();
        }

        /// <summary>
        /// Starts the Azure Diagnostics Monitor based on the configuration setting DiagnosticMonitorEnabled
        /// </summary>
        private void StartDiagnostics()
        {

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(DiagnosticsConnectionStringSetting));

            var roleInstanceDiagnosticManager = CloudAccountDiagnosticMonitorExtensions.CreateRoleInstanceDiagnosticManager(
            RoleEnvironment.GetConfigurationSettingValue(DiagnosticsConnectionStringSetting),
            RoleEnvironment.DeploymentId,
            RoleEnvironment.CurrentRoleInstance.Role.Name,
            RoleEnvironment.CurrentRoleInstance.Id);

            DiagnosticMonitorConfiguration config = roleInstanceDiagnosticManager.GetCurrentConfiguration();

            if (config == null)
            {
                Trace.TraceError("RoleInstanceDiagnosticManager's configuration was null. Resorting to DiagnosticsMonitor's default configuration");
                config = DiagnosticMonitor.GetDefaultInitialConfiguration();
            }

            Trace.TraceInformation("Diagnostics Monitoring Enabled.  Setting Up Azure Diagnostics Config");
            ConfigureDiagnostics(config);
            Trace.TraceInformation("Diagnostics monitoring configured, starting DiagnosticsMonitor");
            roleInstanceDiagnosticManager.SetCurrentConfiguration(config);
            Trace.TraceInformation("DiagnosticsMonitor started");
        }

        /// <summary>
        /// Configures the Azure diagnosics monitor
        /// </summary>
        /// <param name="config"></param>
        protected virtual void ConfigureDiagnostics(DiagnosticMonitorConfiguration config)
        {
            Trace.TraceInformation("Diag: Transfer period set to {0} minutes.  Add setting to change transfer rate: DiagTransferPeriodMinutes", TransferPeriod.Minutes);

            //set up Logs configuration
            ConfigureTraceLogDiagnostics(config);

            //set up WindowsEventLog configuration
            ConfigureEventLogDiagnostics(config);

            //set up DiagnosticInfrastructureLogs
            ConfigureInfrastructureLogDiagnostics(config);

            //set up file-based log configuration (including IIS logs)
            ConfigureTextLogDiagnostics(config);

            // set up PerfomanceCounters
            ConfigurePerformanceCounters(config);
        }

        private TimeSpan transferPeriod = TimeSpan.Zero;
        private TimeSpan TransferPeriod
        {
            get
            {
                if (transferPeriod == TimeSpan.Zero)
                {
                    // Use the same transfer period for all metrics
                    try
                    {
                        transferPeriod = TimeSpan.FromMinutes(SettingsFacade.GetInt32("DiagTransferPeriodMinutes"));
                    }
                    catch (SettingException)
                    {
                        transferPeriod = TimeSpan.FromMinutes(1);
                    }
                }
                return transferPeriod;
            }
        }

        /// <summary>
        /// Configures the performance counters.
        /// </summary>
        /// <param name="config">The diagnostic monitor config.</param>
        /// <remarks>
        /// <para>Sets up the following performance counters:</para>
        /// <ul>
        /// <li>\Processor(_Total)\% Processor Time</li>
        /// <li>\Memory\Available Mbytes</li>
        /// <li>\TCPv6\Connections Established</li>
        /// </ul></remarks>
        protected virtual void ConfigurePerformanceCounters(DiagnosticMonitorConfiguration config)
        {
            config.PerformanceCounters.ScheduledTransferPeriod = TransferPeriod;
            TimeSpan perfSampleRate;
            try
            {
                perfSampleRate = TimeSpan.FromSeconds(SettingsFacade.GetInt32("DiagPerfSampleRateSeconds"));
            }
            catch (SettingException)
            {
                perfSampleRate = TimeSpan.FromSeconds(10);
            }
            Trace.TraceInformation("Diag: Setting up Performance Counters.  Add setting to change sample rate: DiagPerfSampleRateSeconds.  Using transfer period of {0} seconds", perfSampleRate.Seconds);
            AddPerformanceCounters(config.PerformanceCounters.DataSources, perfSampleRate);
        }

        /// <summary>
        /// Adds the performance counters.
        /// </summary>
        /// <param name="dataSources">The data sources.</param>
        /// <param name="perfSampleRate">The perf sample rate.</param>
        protected virtual void AddPerformanceCounters(IList<PerformanceCounterConfiguration> dataSources, TimeSpan perfSampleRate)
        {
            Trace.TraceInformation("Diag: Adding Performance Counters");
            dataSources.Add(new PerformanceCounterConfiguration
            {
                CounterSpecifier = @"\Processor(_Total)\% Processor Time",
                SampleRate = perfSampleRate
            });
            dataSources.Add(new PerformanceCounterConfiguration
            {
                CounterSpecifier = @"\Memory\Available Mbytes",
                SampleRate = perfSampleRate
            });
            dataSources.Add(new PerformanceCounterConfiguration
            {
                CounterSpecifier = @"\TCPv6\Connections Established",
                SampleRate = perfSampleRate
            });
        }

        /// <summary>
        /// Configures the infrastructure log diagnostics.
        /// </summary>
        /// <param name="config">The config.</param>
        protected virtual void ConfigureInfrastructureLogDiagnostics(DiagnosticMonitorConfiguration config)
        {
            var logLevel = GetDiagnosticLogLevelFromConfiguration("DiagInfrastuctureLogLevel");
            Trace.TraceInformation("Diag: Setting up Infrastructure Logs.  Add Setting DiagInfrastuctureLogLevel to adjust log level filter.  Currently using level {0}", logLevel);
            //config.DiagnosticInfrastructureLogs.BufferQuotaInMB = 100;
            config.DiagnosticInfrastructureLogs.ScheduledTransferLogLevelFilter = logLevel;
            config.DiagnosticInfrastructureLogs.ScheduledTransferPeriod = TransferPeriod;
        }

        /// <summary>
        /// Configures the IIS log diagnostics.
        /// </summary>
        /// <param name="config">The config.</param>
        protected virtual void ConfigureTextLogDiagnostics(DiagnosticMonitorConfiguration config)
        {
            Trace.TraceInformation("Diag: Setting up scheduled transfer of text-based logs.  This will include IIS logs in WebRoles");
            config.Directories.ScheduledTransferPeriod = TransferPeriod;
        }

        /// <summary>
        /// Configures the text log diagnostics.
        /// </summary>
        /// <param name="config">The config.</param>
        protected virtual void ConfigureTraceLogDiagnostics(DiagnosticMonitorConfiguration config)
        {
            var logLevel = GetDiagnosticLogLevelFromConfiguration("DiagLogsLogLevel");
            Trace.TraceInformation("Diag: Setting up Trace Logs.  Add Setting DiagLogsLogLevel to adjust log level filter.  Currently using {0}", logLevel);
            config.Logs.ScheduledTransferLogLevelFilter = logLevel;
            config.Logs.ScheduledTransferPeriod = TransferPeriod;
        }

        /// <summary>
        /// Configures the event log diagnostics.
        /// </summary>
        /// <param name="config">The config.</param>
        protected virtual void ConfigureEventLogDiagnostics(DiagnosticMonitorConfiguration config)
        {
            var logLevel = GetDiagnosticLogLevelFromConfiguration("DiagWindowsEventLogLevel");
            Trace.TraceInformation("Diag: Setting up Event Logs.  Add Setting DiagWindowsEventLogLevel to adjust log level filter.  Currently Using {0}", logLevel);
            config.WindowsEventLog.BufferQuotaInMB = 100;
            config.WindowsEventLog.ScheduledTransferLogLevelFilter = logLevel;
            config.WindowsEventLog.ScheduledTransferPeriod = TransferPeriod;
            try
            {
                Trace.TraceInformation("Diag: Setting up Event Log Sources.  If you would like to override the default of 'System!*', then add the configuration setting 'DiagWindowsEventLogSources' with a pipe-separated ('|') list of event sources such as 'System!*|Application!*'");
                String eventLogSources = SettingsFacade.GetString("DiagWindowsEventLogSources");
                String[] sources = eventLogSources.Split('|');
                foreach (var source in sources)
                    config.WindowsEventLog.DataSources.Add(source);
            }
            catch (Exception)
            {
                config.WindowsEventLog.DataSources.Add("System!*");
            }
        }

        /// <summary>
        /// Gets the diagnostic log level from configuration.  If the log level cannot be found, then 
        /// the method returns a default of <see cref="LogLevel.Error"/>
        /// </summary>
        /// <param name="logLevelKey">The log level key.</param>
        /// <returns></returns>
        protected Microsoft.WindowsAzure.Diagnostics.LogLevel GetDiagnosticLogLevelFromConfiguration(string logLevelKey)
        {
            return GetDiagnosticLogLevelFromConfiguration(logLevelKey, Microsoft.WindowsAzure.Diagnostics.LogLevel.Error);
        }

        #region Check Bec One Box Host Entry
        /// <summary>
        /// Gets the diagnostic log level from configuration.
        /// </summary>
        /// <param name="logLevelKey">The log level key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        protected Microsoft.WindowsAzure.Diagnostics.LogLevel GetDiagnosticLogLevelFromConfiguration(string logLevelKey, Microsoft.WindowsAzure.Diagnostics.LogLevel defaultValue)
        {
            try
            {
                return SettingsFacade.GetEnum<Microsoft.WindowsAzure.Diagnostics.LogLevel>(logLevelKey);
            }
            catch (SettingException)
            {
                Trace.TraceWarning(string.Format("Diagnostic transfer LogLevel configuration setting {0} is not set.  Using default value of {1}.", logLevelKey, defaultValue));
                return defaultValue;
            }
        }

        #endregion
    }
}
