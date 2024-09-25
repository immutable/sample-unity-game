using UnityEngine;

namespace Xsolla.Core
{
    public static class XDebug
    {
        private const string LOG_PREFIX = "[Xsolla SDK]";

        public static void Log(object message, bool ignoreLogLevel = false)
        {
            Debug.Log($"{LOG_PREFIX} {message}");
        }

        public static void LogWarning(object message, bool ignoreLogLevel = false)
        {
            Debug.LogWarning($"{LOG_PREFIX} {message}");
        }

        public static void LogError(object message)
        {
            Debug.LogError($"{LOG_PREFIX} {message}");
        }
    }
}