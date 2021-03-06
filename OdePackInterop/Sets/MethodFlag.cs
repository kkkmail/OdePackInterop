using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OdePackInterop.Sets
{
    public record MethodFlag : ClosedDualSetBase<MethodFlag, int, string>
    {
        private MethodFlag(int key, [CallerMemberName] string? value = null) : base(key, value!)
        {
        }

        public static MethodFlag Adams { get; } = new(1);
        public static MethodFlag Bdf { get; } = new(2);
    }
}
