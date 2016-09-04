using System;
using System.Diagnostics;
using log4net;

namespace CCluster.Common
{
    public static class LogProvider
    {
        public static ILog GetCurrentClassLogger()
        {
            string loggerName;
            Type declaringType;
            int framesToSkip = 1;
            do
            {
                StackFrame frame = new StackFrame(framesToSkip, false);
                var method = frame.GetMethod();
                declaringType = method.DeclaringType;
                if (declaringType == null)
                {
                    loggerName = method.Name;
                    break;
                }
                framesToSkip++;
                loggerName = declaringType.FullName;
            } while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));
            return LogManager.GetLogger(loggerName);
        }
    }
}
