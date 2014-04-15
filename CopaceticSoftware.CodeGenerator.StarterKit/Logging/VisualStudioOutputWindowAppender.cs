using log4net.Appender;
using log4net.Core;
using log4net.Layout;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Logging
{
    public class VisualStudioOutputWindowAppender : AppenderSkeleton
    {
        public EnvDTE.OutputWindowPane OutputWindow { get; set; }

        public VisualStudioOutputWindowAppender(EnvDTE.OutputWindowPane outputWindow)
        {
            OutputWindow = outputWindow;

            Layout = new PatternLayout("%-5level %logger - %message%newline");
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (null == OutputWindow)
                return;

            if (null == loggingEvent)
                return;

            OutputWindow.OutputString(RenderLoggingEvent(loggingEvent));
        }
    }
}
