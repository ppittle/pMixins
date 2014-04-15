using System;
using EnvDTE80;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure
{
    /*
    public interface IVisualStudioEventProxyFactory
    {
        IVisualStudioEventProxy BuildVisualStudioEventProxy(Func<DTE2> deferredVisualStudioLoader);
    }

    public abstract class VisualStudioEventProxyFactoryBase : IVisualStudioEventProxyFactory
    {
        private static object _lock = new object();

        private static IVisualStudioEventProxy _visualStudioEventProxy;

        public IVisualStudioEventProxy BuildVisualStudioEventProxy(Func<DTE2> deferredVisualStudioLoader)
        {
            if (null == _visualStudioEventProxy)
            {
                lock (_lock)
                {
                    if (null == _visualStudioEventProxy)
                    {
                        _visualStudioEventProxy = new VisualStudioEventProxy(deferredVisualStudioLoader());
                    }
                }
            }

            return _visualStudioEventProxy;
        }
    }
     */
}
