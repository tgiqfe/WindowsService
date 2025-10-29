using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using WindowsService.Functions;

namespace WindowsService.WindowsService
{
    public class BaseServiceSummary
    {
        protected ServiceController _sc { get; set; }
        protected ManagementObject _mo { get; set; }

        /// <summary>
        /// Is the service a delayed auto start service?
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="mo"></param>
        /// <returns></returns>
        public bool IsDelayedAutoStart()
        {
            if (_sc == null) return false;
            if (_mo == null)
            {
                var keyPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services";
                using (var regKey = RegistryFunctions.GetRegistryKey(keyPath, false))
                {
                    if (regKey != null)
                    {
                        using (var subKey = regKey.OpenSubKey(_sc.ServiceName))
                        {
                            if (subKey != null)
                            {
                                var startValue = subKey.GetValue("Start");
                                var delayedAutoStartValue = subKey.GetValue("DelayedAutostart");
                                if (startValue != null && delayedAutoStartValue != null)
                                {
                                    return (int)startValue == 2 && (int)delayedAutoStartValue == 1;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                return _sc.StartType == ServiceStartMode.Automatic && _mo["DelayedAutoStart"] as bool? == true;
            }
            return false;
        }

        /// <summary>
        /// Is the service a trigger start service?
        /// </summary>
        /// <param name="sc"></param>
        /// <returns></returns>
        public bool IsTriggeredStart()
        {
            if (_sc == null) return false;
            var keyPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services";
            using (var regKey = RegistryFunctions.GetRegistryKey(keyPath, false))
            {
                if (regKey != null)
                {
                    using (var subKey = regKey.OpenSubKey(_sc.ServiceName))
                    {
                        if (subKey != null)
                        {
                            return subKey.GetSubKeyNames().Any(x =>
                                x.Equals("TriggerInfo", StringComparison.OrdinalIgnoreCase));
                        }
                    }
                }
            }
            return false;
        }
    }
}
