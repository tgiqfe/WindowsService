using ItemStorageManager.ItemStorage;
using System.Management;
using System.ServiceProcess;

namespace WindowsService.WindowsService
{
    public class ServiceItem
    {
        #region Public parameter

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public ServiceControllerStatus Status { get; set; }
        public ServiceStartMode StartupType { get; set; }
        public bool TriggerStart { get; set; }
        public bool DelayedAutoStart { get; set; }
        public string ExecutePath { get; set; }
        public string Description { get; set; }
        public string LogonName { get; set; }
        public long ProcessId { get; set; }

        #endregion

        const string _log_target = "ServiceItem";

        public ServiceItem(ServiceController sc, ManagementObject mo = null)
        {
            mo ??= new ManagementClass("Win32_Service").
               GetInstances().
               OfType<ManagementObject>().
               FirstOrDefault(x => sc.ServiceName == x["Name"] as string);

            this.Name = sc.ServiceName;
            this.DisplayName = sc.DisplayName;
            this.Status = sc.Status;
            this.StartupType = sc.StartType;
            this.TriggerStart = StartupChecker.IsTriggeredStart(sc);
            this.TriggerStart = StartupChecker.IsDelayedAutoStart(sc, mo);
            if (mo != null)
            {
                this.ExecutePath = mo["PathName"] as string;
                this.Description = mo["Description"] as string;
                this.LogonName = mo["StartName"] as string;
                this.ProcessId = (uint)mo["ProcessId"];
            }
        }

        public static ServiceItem[] Load(string serviceName = null)
        {
            var services = serviceName == null ?
                ServiceController.GetServices() :
                ServiceController.GetServices().
                    Where(x =>
                        x.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase) ||
                        x.DisplayName.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
            var wmi_services = new ManagementClass("Win32_Service").
                GetInstances().
                OfType<ManagementObject>();

            return services.
                Select(sc => new ServiceItem(sc, wmi_services.FirstOrDefault(mo => sc.ServiceName == mo["Name"] as string))).
                ToArray();
        }

        public static bool Exists(string name)
        {
            Logger.WriteLine("Info", $"Checking existence of {_log_target}: {name}");

            //  Check by loading the service

            return false;
        }

        public bool ToStart()
        {
            Logger.WriteLine("Info", $"Starting {_log_target}: {this.Name}");

            //  Start the service

            return false;
        }

        public bool ToStop()
        {
            Logger.WriteLine("Info", $"Stopping {_log_target}: {this.Name}");

            //  Stop the service

            return false;
        }

        public bool ToRestart()
        {
            Logger.WriteLine("Info", $"Restarting {_log_target}: {this.Name}");

            //  Restart the service

            return false;
        }

        public bool ChangeStartupType(string mode)
        {
            Logger.WriteLine("Info", $"Changing startup type of {_log_target}: {this.Name} to {mode}");

            //  Change the startup type of the service

            return false;
        }
    }
}
