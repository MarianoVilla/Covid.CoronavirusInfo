using Android.Util;
using System;
using System.Reflection;

namespace Covid.Droid.Helpers
{
    public class DebugHelper
    {
        public static void Debug(string Message) => Log.Debug("Covid Debug", Message);
        public static void Error(string Error) => Log.Error("Covid Error", Error);
        public static void Error(Exception ex) => Log.Error("Covid Error", $"{ex.Message} {ex.StackTrace}");
        public static void Info(string Message) => Log.Info("Covid Info", Message);
        public static void Method(MethodBase Method) => Log.Info("Covid Info", $"Entered method {Method.Name}");
    }
}