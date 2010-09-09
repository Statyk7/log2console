//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Text;

namespace MS.WindowsAPICodePack.Internal
{
    /// <summary>
    /// Common Helper methods
    /// </summary>
    static public class CoreHelpers
    {
        /// <summary>
        /// Determines if the application is running on XP
        /// </summary>
        public static bool RunningOnXP
        {
            get
            {
                return Environment.OSVersion.Version.Major >= 5;
            }
        }

        /// <summary>
        /// Throws PlatformNotSupportedException if the application is not running on Windows XP
        /// </summary>
        public static void ThrowIfNotXP()
        {
            if (!CoreHelpers.RunningOnXP)
            {
                throw new PlatformNotSupportedException("Only supported on Windows XP or newer.");
            }
        }

        /// <summary>
        /// Determines if the application is running on Vista
        /// </summary>
        public static bool RunningOnVista
        {
            get
            {
                return Environment.OSVersion.Version.Major >= 6;
            }
        }

        /// <summary>
        /// Throws PlatformNotSupportedException if the application is not running on Windows Vista
        /// </summary>
        public static void ThrowIfNotVista()
        {
            if (!CoreHelpers.RunningOnVista)
            {
                throw new PlatformNotSupportedException("Only supported on Windows Vista or newer.");
            }
        }

        /// <summary>
        /// Determines if the application is running on Windows 7
        /// </summary>
        public static bool RunningOnWin7
        {
            get
            {
                return (Environment.OSVersion.Version.Major > 6) ||
                    (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor >= 1);
            }
        }

        /// <summary>
        /// Throws PlatformNotSupportedException if the application is not running on Windows 7
        /// </summary>
        public static void ThrowIfNotWin7()
        {
            if (!CoreHelpers.RunningOnWin7)
            {
                throw new PlatformNotSupportedException("Only supported on Windows 7 or newer.");
            }
        }

        /// <summary>
        /// Get a string resource given a resource Id
        /// </summary>
        /// <param name="resourceId">The resource Id</param>
        /// <returns>The string resource corresponding to the given resource Id. Returns null if the resource id
        /// is invalid or the string cannot be retrieved for any other reason.</returns>
        public static string GetStringResource(string resourceId)
        {
            string[] parts;
            string library;
            int index;

            if (String.IsNullOrEmpty(resourceId))
            {
                return String.Empty;
            }
            // Known folder "Recent" has a malformed resource id
            // for its tooltip. This causes the resource id to
            // parse into 3 parts instead of 2 parts if we don't fix.
            resourceId = resourceId.Replace("shell32,dll", "shell32.dll");
            parts = resourceId.Split(new char[] { ',' });

            library = parts[0];
            library = library.Replace(@"@", String.Empty);

            parts[1] = parts[1].Replace("-", String.Empty);
            index = Int32.Parse(parts[1]);

            library = Environment.ExpandEnvironmentVariables(library);
            IntPtr handle = CoreNativeMethods.LoadLibrary(library);
            StringBuilder stringValue = new StringBuilder(255);
            int retval = CoreNativeMethods.LoadString(
                handle, index, stringValue, 255);

            if (retval == 0)
                return null;
            else
                return stringValue.ToString();
        }
    }
}
