# OdePackInterop
This is a simple C# NET5 interop with (FORTRAN ODE Solver DLSODE)[https://computing.llnl.gov/projects/odepack] aimed at solving very large systems of potentially stiff ODE (like chemical systems) where the number of variables is ![formula](https://render.githubusercontent.com/render/math?math=\sim%2010^5). An alternative could be to use (SUNDIALS)[https://computing.llnl.gov/projects/sundials], which is a newer C port of various FORTRAN solvers. However, since SUNDIALS is written in C an attempt to create a NET5 interop results in C++/CLI E0337 "Linkage specification is incompatible" error, which is a known issue but without a publicly available solution: https://developercommunity.visualstudio.com/t/ccli-e0337-linkage-specification-is-incompatible/919335 as of March 11, 2021.

Stiff chemical problems of this size pose several computational difficulties.
1. 


# References
FORTRAN source code: https://www.netlib.org/odepack/

https://www.codeproject.com/Articles/1065197/Introduction-to-FORTRAN-Interoperability-with-NET
https://community.intel.com/t5/Intel-Fortran-Compiler/Exporting-subroutines-from-a-Fortran-DLL/td-p/1129099
https://community.intel.com/t5/Intel-Fortran-Compiler/error-6633-The-type-of-the-actual-argument-differs-from-the-type/td-p/1010721
http://www.stochasticlifestyle.com/wp-content/uploads/2019/11/de_solver_software_comparsion.pdf

