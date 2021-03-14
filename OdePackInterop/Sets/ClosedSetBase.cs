using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Softellect.OdePackInterop.Sets
{
    public abstract record ClosedSetBase<T, TV> : SetBase<T, TV>
        where T : ClosedSetBase<T, TV>
        where TV : IComparable<TV>
    {
        protected ClosedSetBase(TV value) : base(value)
        {
        }

        private static ImmutableHashSet<T> GetAllValuesImpl(Type? t = null)
        {
            t ??= typeof(T);

            var values = t.GetNestedTypes(BindingFlags.Public | BindingFlags.Static)
                .SelectMany(GetAllValuesImpl)
                .Concat(t.GetProperties(BindingFlags.Public | BindingFlags.Static)
                    .Where(e => e.PropertyType == typeof(T))
                    .Select(e => e.GetValue(null) as T)
                    .Where(e => e != null!)
                    .Select(e => e!))
                .ToImmutableHashSet();

            return values;
        }

        private static readonly Lazy<ImmutableHashSet<T>> AllValues = new (() => GetAllValuesImpl());

        private static readonly Lazy<ImmutableDictionary<TV, T>> AllValuesDictionary =
            new(() => GetAllValues().ToImmutableDictionary(e => e.Value, e => e));

        public static ImmutableHashSet<T> GetAllValues() => AllValues.Value;
        public static ImmutableDictionary<TV, T> GetAllValuesDictionary() => AllValuesDictionary.Value;
        public static T? TryCreate(TV v) => GetAllValuesDictionary().TryGetValue(v, out var t) ? t : null;
    }
}
