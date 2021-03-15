# OdePackInterop
This is a simple C# NET5 interop with [FORTRAN ODE Solver DLSODE](https://computing.llnl.gov/projects/odepack) aimed at solving very large systems of potentially stiff ODE (like chemical systems) where the number of variables is ![formula](https://render.githubusercontent.com/render/math?math=\sim%2010^5). 

An alternative could be to use [SUNDIALS](https://computing.llnl.gov/projects/sundials), which is a newer C port of various FORTRAN solvers. However, since SUNDIALS is written in C, an attempt to create a NET5 interop results in **_C++/CLI E0337 "Linkage specification is incompatible"_** error, which is [a known issue but without a publicly available solution](https://developercommunity.visualstudio.com/t/ccli-e0337-linkage-specification-is-incompatible/919335) as of March 11, 2021.

Stiff chemical problems of this size pose several computational difficulties.
1. To handle stiffness, forward methods often decrease the step to a very small value. This results in extremely large solution time. Basically, the solver just never comes back. This _seems_ to happen when some of the variables start to approach zero. The solver then overshoots zero, stiffness kicks in, and that, in turn, makes solver algorithm decrease the step to a very small value.
2. Backward methods require either inverting very large matrices or using full or banded Jacobian. That results in ![formula](https://render.githubusercontent.com/render/math?math=\sim%20N^2) memory increase and ![formula](https://render.githubusercontent.com/render/math?math=\sim%20N^3) number of operations increase as the number of equations ![formula](https://render.githubusercontent.com/render/math?math=N) increases.
3. Chemical systems often have an integral of motion: the "matter" is conserved in some way and this is usually written that some linear combination of variables must stay constant. Some solvers may break such integral of motion thus rendering the results questionable. Symplectic integrators cannot be used to remedy the situation as they are designed to handle completely different tasks.

# Test setup
The tests use a chemical-like system of equations based on a simple set of "reactions":

![formula](https://render.githubusercontent.com/render/math?math=y_0%20\rightleftharpoons%20y_1%20%2B%20y_2)

![formula](https://render.githubusercontent.com/render/math?math=y_2%20\rightleftharpoons%20y_3%20%2B%20y_4)

![formula](https://render.githubusercontent.com/render/math?math=y_4%20\rightleftharpoons%20y_5%20%2B%20y_6)

…

![formula](https://render.githubusercontent.com/render/math?math=y_{2n-2}%20\rightleftharpoons%20y_{2n-1}%20%2B%20y_{2n})

The number of variables was **_100,001_** (**_n = 50,000_**).

All forward and backward coefficients were the same:

![formula](https://render.githubusercontent.com/render/math?math=k_f%20=%201.0)

![formula](https://render.githubusercontent.com/render/math?math=k_b%20=%200.1)

The initial conditions set

![formula](https://render.githubusercontent.com/render/math?math=y_0%20=%2010)

and all other to zeros. 

The system was solved from ![formula](https://render.githubusercontent.com/render/math?math=t%20=%200) to ![formula](https://render.githubusercontent.com/render/math?math=t%20=%2010^6). The system has an integral of motion: sum of all **_y_** must be constant.

Five variants were tested under two different setups. The first setup treated all negative values of **_y_** as exact zeros when calculating the derivatives and the second one just used them as is without any corrections. Five tested variants were as follows:
1. MF = 23 (`SolutionMethod.Bdf`, `CorrectorIteratorMethod.ChordWithDiagonalJacobian`).
2. MF = 13 (`SolutionMethod.Adams`, `CorrectorIteratorMethod.ChordWithDiagonalJacobian`).
3. MF = 20 (`SolutionMethod.Bdf`, `CorrectorIteratorMethod.Functional`).
4. MF = 10 (`SolutionMethod.Adams`, `CorrectorIteratorMethod.Functional`).
5. AlgLib Cash-Carp method.

These variants were chosen as they were the only ones, which did not require time and memory expensive Jacobian calculations. All other combinations from DLSODE solver and all other solvers from ODEPACK do require full or banded Jacobian in some form and this was ruled out due to its size. Corrector iterator method `ChordWithDiagonalJacobian` calculates diagonal Jacobian and this is only one extra call to the derivative function per step.

# Test results
When non-negativity was used (all negative values of **_y_** were treated as zeros when calculating
the derivative) then the results are as follows (`No. f-s` is the number of derivative evaluations and `No. J-s` is the number of Jacobian evaluations):

1. MF = 23 (`SolutionMethod.Bdf`, `CorrectorIteratorMethod.ChordWithDiagonalJacobian`).
```
       Integral of motion: 10.0 -> 10.301689191032535 or OVER 3% discrepancy.
       No. steps = 40,104, No. f-s = 132,533, No. J-s = 37,380
       Elapsed: 00:02:37.3532643
```

2. MF = 13 (`SolutionMethod.Adams`, `CorrectorIteratorMethod.ChordWithDiagonalJacobian`).
```
       Integral of motion: 10.0 -> 10.380914193130206 or OVER 3% discrepancy.
       No. steps = 39,955, No. f-s = 100,769, No. J-s = 20,207
       Elapsed: 00:01:49.2829071
```

3. MF = 20 (`SolutionMethod.Bdf`, `CorrectorIteratorMethod.Functional`).
```
       Integral of motion: 10.0 -> 9.999999999999996.
       No. steps = 49,067, No. f-s = 89,820, No. J-s = 0
       Elapsed: 00:01:42.9414014
```

4. MF = 10 (`SolutionMethod.Adams`, `CorrectorIteratorMethod.Functional`).
```
       Integral of motion: 10.0 -> 9.999999999999936.
       No. steps = 48,266, No. f-s = 87,707, No. J-s = 0
       Elapsed: 00:01:39.7107217
```

5. AlgLib `Cash-Carp` method.
```
       The solver did not come back.
```

When non-negativity was turned off, then the following happened:

1. MF = 23 (`SolutionMethod.Bdf`, `CorrectorIteratorMethod.ChordWithDiagonalJacobian`).
```
       Integral of motion is nearly conserved: 10.0 -> 9.994361679959828
       No. steps = 18,176, No. f-s = 64,101, No. J-s = 20,098
       Elapsed: 00:00:46.7831109
```

2. MF = 13 (`SolutionMethod.Adams`, `CorrectorIteratorMethod.ChordWithDiagonalJacobian`).
```

       Integral of motion has nearly 2% discrepancy: 10.0 -> 9.98345003326132
       No. steps = 185,378, No. f-s = 649,958, No. J-s = 184,053
       Elapsed: 00:08:43.6774383
```

3. MF = 20 (`SolutionMethod.Bdf`, `CorrectorIteratorMethod.Functional`).
```
       The solver did not come back.
```

4. MF = 10 (`SolutionMethod.Adams`, `CorrectorIteratorMethod.Functional`).
```
       The solver did not come back.
```

5. AlgLib `Cash-Carp` method.
```
       The solver did not come back.
```

This makes the following combinations as clear winners:
1. Do not use non-negativity (treat the variables exactly as they are), then use `SolutionMethod.Bdf` and `CorrectorIteratorMethod.ChordWithDiagonalJacobian`. This seems to be the fastest method with the least number of derivative function evaluations. It was at least twice faster in the test, but the integral of motion started to deviate from the expected value and this may or may not have a significant impact for real tasks.
2. Use non-negativity (treat all negative values as exact zeros) when calculating the derivative, use `CorrectorIteratorMethod.Functional`, and then use either `SolutionMethod.Adams` (seems to be slightly faster) or `SolutionMethod.Bdf`. The integral of motion was conserved with a very high precision. However, the calculation was at least twice slower than for the combination above, and the number of derivative function evaluation was about 50% larger. This may change for real tasks.

# References
[ODEPACK FORTRAN Source Code](https://www.netlib.org/odepack/)

[FORTRAN Interoperability with NET](https://www.codeproject.com/Articles/1065197/Introduction-to-FORTRAN-Interoperability-with-NET)

[Exporting subroutines from a FORTRAN DLL](https://community.intel.com/t5/Intel-Fortran-Compiler/Exporting-subroutines-from-a-Fortran-DLL/td-p/1129099)

[FORTAN compiler issue due to usage of array and scalar variable interchangeably](https://community.intel.com/t5/Intel-Fortran-Compiler/error-6633-The-type-of-the-actual-argument-differs-from-the-type/td-p/1010721)

[Comparison of ODE Solvers](http://www.stochasticlifestyle.com/wp-content/uploads/2019/11/de_solver_software_comparsion.pdf)

[Discussion about positivity constraints in ODEs](https://mathematica.stackexchange.com/questions/45727/constraining-function-found-by-ndsolve-to-stay-positive/)

# Compiler and build
[Intel FORTRAN compiler](https://software.intel.com/content/www/us/en/develop/tools/oneapi/components/fortran-compiler.html) was used to compile FORTRAN code. File `ODEPACK\dependencies.txt` contains dependencies of compiled DLL and folder `DLLs` contains some of them. Solution was tested only in `x64` mode and Release mode is recommended as it is not possible to debug in FORTRAN code from C# interop anyway.
