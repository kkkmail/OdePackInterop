
This folder contains some programs that are used for computations
and illustrations of the monograph


                 GEOMETRIC NUMERICAL INTEGRATION
 Structure-Preserving Algorithms for Ordinary Differential Equations
               E. Hairer, Ch. Lubich and G. Wanner 
Springer Series in Computational Mathematics 31, Springer-Verlag, 2002
                    ISBN 3-540-43003-2 

A MATLAB version of these codes is also available. Its description
is given in the publication:

E. Hairer and M. Hairer, GNICODE - Matlab Programs for
                         Geometric Numerical Integration
    in: Frontiers in Numerical Analysis (Durham 2002),
        Springer, Berlin, 2003.

REMARK:
     Some programs use the GGG graphics library. The Fortran program GGG.f
     is available from "http://www.unige.ch/math/folks/hairer"

CONTENTS:

  gni_irk2.f   implicit Runge-Kutta code (Gauss methods) for
               second order differential equations
  gni_comp.f   symmetric composition methods based on
               second order symmetric methods
  gni_lmm2.f   symmetric linear multistep methods for
               second order differential equations

  dr_irk2.f    driver for gni_irk2.f
  dr_comp.f    driver for gni_comp.f
  dr_lmm2.f    driver for gni_lmm2.f

  problem.f    data and equations for the problems:
               Kepler problem, harmonic oscillator, pendulum, and
               outer solar system

  FOLDER comparison   
           for Kepler problem with 200 periods, outer solar system
           all three methods are of order 8

       cal_irk2.f     computation for Gauss methot
       cal_comp.f     computation for composition method
       cal_lmm2.f     computation for linear multistep method

       compar_dess.f  makes the figure (uses GGG library)
       
  FOLDER examples

       rigidbody.f    gives an application of composition methods
                      two basic methods are presented:
                      "quater" splitting and
                      "ratori" rattle
                      
       twobdsph.f     two body problem on the sphere
                      rattle as basic method for composition
                      use of "last" is illustrated

       henon.f        computation of the Poincaré section for
                      Henon-Heiles problem
