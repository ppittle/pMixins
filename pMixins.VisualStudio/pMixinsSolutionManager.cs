using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;

namespace CopaceticSoftware.pMixins.VisualStudio
{
    public class pMixinsSolutionManager : SolutionManager
    {
        public pMixinsSolutionManager(Solution solution, IVisualStudioEventProxy visualStudioEventProxy) : 
            base(solution, visualStudioEventProxy)
        {
        }
    }
}
