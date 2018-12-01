using System;
using System.Runtime.InteropServices;

namespace LeanBrowser
{
    class WindowUtils
    {
        [DllImportAttribute("dwmapi.dll")]
        public static extern IntPtr DwmIsCompositionEnabled(ref bool pfEnabled);

        public static bool IsCompositionEnabled
        {
            get
            {
                bool result = false;

                try
                {
                    if (DwmIsCompositionEnabled(ref result) == IntPtr.Zero)
                    {
                        return result;
                    }

                    return false;
                }

                catch (DllNotFoundException)
                {
                    return false;
                }
            }
        }

        // Return true if Windows 7 or older, false otherwise
        public static bool IsLegacyWindows
        {
            get
            {
                if (Environment.OSVersion.Version < new Version(6, 2))
                {
                    return true;
                }

                return false;
            }
        }
    }
}
