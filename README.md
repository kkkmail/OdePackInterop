# OdePackInterop
This is a simple C# NET5 interop with [FORTRAN ODE Solver DLSODE]([https://computing.llnl.gov/projects/odepack) aimed at solving very large systems of potentially stiff ODE (like chemical systems) where the number of variables is ![formula](https://render.githubusercontent.com/render/math?math=\sim%2010^5). 

An alternative could be to use [SUNDIALS](https://computing.llnl.gov/projects/sundials), which is a newer C port of various FORTRAN solvers. However, since SUNDIALS is written in C an attempt to create a NET5 interop results in C++/CLI E0337 "Linkage specification is incompatible" error, which is [a known issue but without a publicly available solution](https://developercommunity.visualstudio.com/t/ccli-e0337-linkage-specification-is-incompatible/919335) as of March 11, 2021.

Stiff chemical problems of this size pose several computational difficulties.
1. To handle stiffness, forward methods often decrease the step to a very small value. This results in extremely large solution time. Basically, the solver just never comes back. This _seems_ to happen when some of the variables start to approach zero. The solver then overshoots zero, stiffness kicks in and that, in turn, makes solver algorithm decrease the step to a very small value.
2. Backward methods require either inverting very large matrices or using (full or banded) Jacobian. That results in ![formula](https://render.githubusercontent.com/render/math?math=\sim%20N^2) memory increase and ![formula](https://render.githubusercontent.com/render/math?math=\sim%20N^3) number of operations increase as the number of equations **_N_** increases.
3. Chemical systems often have an integral of motion: the "matter" is conserved in some way and this is usually written that some linear combination of variables must stay constant. Some solvers may break such integral of motion. This is different from symplectic integrators, which are designed to handle completely different task.


# References
[ODEPACK FORTRAN Source Code](https://www.netlib.org/odepack/)
[FORTRAN Interoperability with NET](https://www.codeproject.com/Articles/1065197/Introduction-to-FORTRAN-Interoperability-with-NET)
[Exporting subroutines from a FORTRAN DLL](https://community.intel.com/t5/Intel-Fortran-Compiler/Exporting-subroutines-from-a-Fortran-DLL/td-p/1129099)
[FORTAN compiler issue due to usage of array and scalar variable interchangeably](https://community.intel.com/t5/Intel-Fortran-Compiler/error-6633-The-type-of-the-actual-argument-differs-from-the-type/td-p/1010721)
[Comparison of ODE Solvers](http://www.stochasticlifestyle.com/wp-content/uploads/2019/11/de_solver_software_comparsion.pdf)
[Discussion about positivity constraints in ODEs](https://mathematica.stackexchange.com/questions/45727/constraining-function-found-by-ndsolve-to-stay-positive/)
