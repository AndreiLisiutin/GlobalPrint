﻿using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.CommonUtils
{
    public class MimeTypeUtility
    {
        private static ConcurrentDictionary<string, string> MimeTypeToExtension = new ConcurrentDictionary<string, string>();
        private static ConcurrentDictionary<string, string> ExtensionToMimeType = new ConcurrentDictionary<string, string>();

        public string ConvertMimeTypeToExtension(string mimeType)
        {
            if (string.IsNullOrWhiteSpace(mimeType))
                throw new ArgumentNullException("mimeType");

            string key = string.Format(@"MIME\Database\Content Type\{0}", mimeType);
            string result;
            if (MimeTypeToExtension.TryGetValue(key, out result))
                return result;

            RegistryKey regKey;
            object value;

            regKey = Registry.ClassesRoot.OpenSubKey(key, false);
            value = regKey != null ? regKey.GetValue("Extension", null) : null;
            result = value != null ? value.ToString() : string.Empty;

            MimeTypeToExtension[key] = result;
            return result;
        }

        public string ConvertExtensionToMimeType(string extension)
        {

            if (string.IsNullOrWhiteSpace(extension))
                throw new ArgumentNullException("extension");

            if (!extension.StartsWith("."))
                extension = "." + extension;

            string result;
            if (ExtensionToMimeType.TryGetValue(extension, out result))
                return result;

            RegistryKey regKey;
            object value;

            regKey = Registry.ClassesRoot.OpenSubKey(extension, false);
            value = regKey != null ? regKey.GetValue("Content Type", null) : null;
            result = value != null ? value.ToString() : string.Empty;

            ExtensionToMimeType[extension] = result;
            return result;
        }
    }
}
