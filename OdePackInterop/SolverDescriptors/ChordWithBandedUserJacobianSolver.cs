using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OdePackInterop.Sets;

namespace OdePackInterop.SolverDescriptors
{
    public record ChordWithBandedUserJacobianSolver : SolverDescriptorBase
    {
        public ChordWithBandedUserJacobianSolver(
            int numberOfEquations,
            MethodFlag methodFlag)
            : base(numberOfEquations, methodFlag, CorrectorIteratorMethod.ChordWithBandedUserJacobian)
        {

        }
    }
}
