using log4net;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Logging
{
    public static class Log4NetInitializer
    {
        private static bool _isInitialized;

        private static object _lock = new object();

        public static void Initialize(EnvDTE.OutputWindowPane outputWindow)
        {
            if (_isInitialized)
                return;

            lock (_lock)
            {
                if (_isInitialized)
                    return;

                log4net.Config.BasicConfigurator.Configure();

                var heirachy = (LogManager.GetRepository() as Hierarchy);
                if (null == heirachy)
                    return;

                var root = heirachy.Root as IAppenderAttachable;
                if (null == root)
                    return;

                root.AddAppender(new VisualStudioOutputWindowAppender(outputWindow));

                _isInitialized = true;
            }
        }
    }
}