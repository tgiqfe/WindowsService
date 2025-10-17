﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace WindowsService.WindowsService.Functions
{
    public class RegistryFunctions
    {
        /// <summary>
        /// Get RegistryKey instance from full key path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isCreate"></param>
        /// <param name="writable"></param>
        /// <returns></returns>
        public static RegistryKey GetRegistryKey(string path, bool isCreate = false, bool writable = false)
        {
            string rootPath = path.Substring(0, path.IndexOf("\\"));
            string keyPath = path.Substring(path.IndexOf("\\") + 1);

            RegistryKey rootKey = rootPath.ToLower() switch
            {
                "hkcr" or "hkcr:" or "hkey_classes_root" => Registry.ClassesRoot,
                "hkcu" or "hkcu:" or "hkey_current_user" => Registry.CurrentUser,
                "hklm" or "hklm:" or "hkey_local_machine" => Registry.LocalMachine,
                "hku" or "hku:" or "hkey_users" => Registry.Users,
                "hkcc" or "hkcc:" or "hkey_current_config" => Registry.CurrentConfig,
                _ => null
            };
            if (rootKey == null) return null;

            return isCreate ?
                rootKey.CreateSubKey(keyPath, writable) :
                rootKey.OpenSubKey(keyPath, writable);
        }

        /// <summary>
        /// Convert string to RegistryValueKind.
        /// </summary>
        /// <param name="valueKindString"></param>
        /// <returns></returns>
        public static RegistryValueKind StringToValueKind(string valueKindString)
        {
            return valueKindString.ToLower() switch
            {
                "reg_sz" or "string" => RegistryValueKind.String,
                "reg_binary" or "binary" => RegistryValueKind.Binary,
                "reg_dword" or "dword" => RegistryValueKind.DWord,
                "reg_qword" or "qword" => RegistryValueKind.QWord,
                "reg_multi_sz" or "multistring" => RegistryValueKind.MultiString,
                "reg_expand_sz" or "expandstring" => RegistryValueKind.ExpandString,
                "reg_none" or "none" => RegistryValueKind.None,
                _ => RegistryValueKind.Unknown,
            };
        }

        public static string RegistryValueKindToString(RegistryValueKind valueKind)
        {
            return valueKind switch
            {
                RegistryValueKind.String => "REG_SZ",
                RegistryValueKind.Binary => "REG_BINARY",
                RegistryValueKind.DWord => "REG_DWORD",
                RegistryValueKind.QWord => "REG_QWORD",
                RegistryValueKind.MultiString => "REG_MULTI_SZ",
                RegistryValueKind.ExpandString => "REG_EXPAND_SZ",
                RegistryValueKind.None => "REG_NONE",
                _ => "REG_UNKNOWN"
            };
        }

        public static string RegistryValueToString(RegistryKey regKey, string name, bool noResolv = true)
        {
            RegistryValueKind valueKind = regKey.GetValueKind(name);
            return valueKind switch
            {
                RegistryValueKind.String => regKey.GetValue(name) as string,
                RegistryValueKind.DWord => regKey.GetValue(name).ToString(),
                RegistryValueKind.QWord => regKey.GetValue(name).ToString(),
                RegistryValueKind.ExpandString => noResolv ?
                    regKey.GetValue(name, "", RegistryValueOptions.DoNotExpandEnvironmentNames) as string :
                    regKey.GetValue(name) as string,
                RegistryValueKind.Binary =>
                    BitConverter.ToString(regKey.GetValue(name) as byte[]).Replace("-", "").ToUpper(),
                RegistryValueKind.MultiString => string.Join("\\0", regKey.GetValue(name) as string[]),
                RegistryValueKind.None => null,
                _ => null,
            };
        }
    }
}
