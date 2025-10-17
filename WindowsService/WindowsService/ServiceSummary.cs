using System.Management;
using System.ServiceProcess;
using WindowsService.WindowsService.Functions;

namespace WindowsService.WindowsService
{
    public class ServiceSummary : BaseServiceSummary
    {
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

        public ServiceSummary(ServiceController sc, ManagementObject mo = null)
        {
            _sc = sc;
            _mo = mo ?? new ManagementClass("Win32_Service").
                GetInstances().
                OfType<ManagementObject>().
                FirstOrDefault(x => sc.ServiceName == x["Name"] as string);

            this.Name = sc.ServiceName;
            this.DisplayName = sc.DisplayName;
            this.Status = sc.Status;
            this.StartupType = sc.StartType;
            this.TriggerStart = IsTriggeredStart();
            this.TriggerStart = IsDelayedAutoStart();
            if (mo != null)
            {
                this.ExecutePath = mo["PathName"] as string;
                this.Description = mo["Description"] as string;
                this.LogonName = mo["StartName"] as string;
                this.ProcessId = (uint)mo["ProcessId"];
            }
        }

        public static ServiceSummary[] Load(string serviceName = null)
        {
            IEnumerable<ServiceController> services = null;
            if (serviceName == null)
            {
                services = ServiceController.GetServices();
            }
            else if (serviceName.Contains("*") || serviceName.Contains("?"))
            {
                var regPattern = TextFunctions.WildcardMatch(serviceName);
                services = ServiceController.GetServices().
                    Where(x =>
                        regPattern.IsMatch(x.ServiceName) || regPattern.IsMatch(x.DisplayName));
            }
            else
            {
                services = ServiceController.GetServices().
                    Where(x =>
                        x.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase) ||
                        x.DisplayName.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
            }
            var wmi_services = new ManagementClass("Win32_Service").
                GetInstances().
                OfType<ManagementObject>();

            return services.
                Select(sc => new ServiceSummary(sc, wmi_services.FirstOrDefault(mo => sc.ServiceName == mo["Name"] as string))).
                ToArray();
        }
    }
}
