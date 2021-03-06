using System;
using System.IO;
using static OdePackInterop.Sets.SolutionMethod;
using static OdePackInterop.Sets.CorrectorIteratorMethod;

namespace OdePackInterop.Sets
{
    public static class SetExt
    {
        public static T Switch<T>(
            this SolutionMethod methodFlag,
            Func<T> onAdams,
            Func<T> onBdf
        ) =>
            methodFlag == Adams ? onAdams()
            : methodFlag == Bdf ? onBdf()
            : throw SolutionMethod.ToInvalidDataException(methodFlag);

        public static T Switch<T>(
            this CorrectorIteratorMethod iteratorMethod,
            Func<T> onFunctional,
            Func<T> onChordWithUserJacobian,
            Func<T> onChordWithGeneratedJacobian,
            Func<T> onChordWithDiagonalJacobian,
            Func<T> onChordWithBandedUserJacobian,
            Func<T> onChordWithBandedGeneratedJacobian
        ) =>
            iteratorMethod == Functional ? onFunctional()
            : iteratorMethod == ChordWithUserJacobian ? onChordWithUserJacobian()
            : iteratorMethod == ChordWithGeneratedJacobian ? onChordWithGeneratedJacobian()
            : iteratorMethod == ChordWithDiagonalJacobian ? onChordWithDiagonalJacobian()
            : iteratorMethod == ChordWithBandedUserJacobian ? onChordWithBandedUserJacobian()
            : iteratorMethod == ChordWithBandedGeneratedJacobian ? onChordWithBandedGeneratedJacobian()
            : throw CorrectorIteratorMethod.ToInvalidDataException(iteratorMethod);
    }
}
