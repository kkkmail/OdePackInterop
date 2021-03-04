using System;
using static OdePackInterop.Interop;

namespace OdePackInteropTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            double a = 10.0;
            double b = 10.0;
            double c = 0.0;

            DUMSUM(ref a, ref b, ref c);

            Console.WriteLine($"a = {a}, b = {b}, c = {c}.");
            Console.ReadLine();
        }
    }
}
