using System.ServiceProcess;
using WindowsService.WindowsService.Functions;
using WinSettingManager.Functions;

namespace WindowsService.WindowsService
{
    public class ServiceSimpleSummary : BaseServiceSummary
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Status { get; set; }
        public string StartupType { get; set; }
        
        public ServiceSimpleSummary(ServiceController sc)
        {
            _sc = sc;

            this.Name = sc.ServiceName;
            this.DisplayName = sc.DisplayName;
            this.Status = sc.Status switch
            {
                ServiceControllerStatus.Running => "実行中",
                ServiceControllerStatus.Stopped => "停止",
                ServiceControllerStatus.Paused => "一時中断",
                ServiceControllerStatus.StartPending => "開始中",
                ServiceControllerStatus.StopPending => "停止中",
                ServiceControllerStatus.PausePending => "一時中断保留中",
                ServiceControllerStatus.ContinuePending => "継続保留中",
                _ => "不明"
            };
            this.StartupType = sc.StartType switch
            {
                ServiceStartMode.Automatic => "自動",
                ServiceStartMode.Manual => "手動",
                ServiceStartMode.Disabled => "無効",
                _ => "不明"
            };

            var delay = IsDelayedAutoStart();
            var trigger = IsTriggeredStart();
            if (delay || trigger)
            {
                List<string> list = new();
                if (delay) list.Add("遅延自動");
                if (trigger) list.Add("トリガー開始");
                this.StartupType += " (" + string.Join(",", list) + ")";
            }
        }

        public static ServiceSimpleSummary[] Load(string serviceName = null)
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

            return services.Select(x => new ServiceSimpleSummary(x)).ToArray();
        }
    }
}
