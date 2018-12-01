using System;
using System.IO;

namespace LeanBrowser
{
    class StaticDeclarations
    {
        public static string Bookmarkspath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "LeanBrowser\\user data\\bookmarks.json");

        public static string Historypath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "LeanBrowser\\user data\\history.json");

        public static bool IsIncognito;
        public static bool IsFullscreen;
    }
}
