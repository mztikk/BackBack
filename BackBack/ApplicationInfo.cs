using System.Reflection;

namespace BackBack
{
    internal static class ApplicationInfo
    {
        internal static readonly string s_appName = Assembly.GetEntryAssembly().GetName().Name;
    }
}
