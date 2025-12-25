C-----------------------------------------------------------------------
      SUBROUTINE GNI_LMM2(N,FCN,NSTEP,X,P,Q,XEND,
     &                    METH,SOLFIX,IOUT,RPAR,IPAR)
C-----------------------------------------------------------------------
C                 VERSION OF NOVEMBER 3,2002  
C  E-MAIL CONTACT ADDRESS : Ernst.Hairer@math.unige.ch
C-----------------------------------------------------------------------
C  SOLVES SECOND ORDER ORDINARY DIFFERENTIAL EQUATIONS OF THE FORM
C                       Q'' = F(X,Q)
C  BASED ON SYMMETRIC LINEAR MULTISTEP METHODS
C  DESCRIBED IN CHAPTER XIV OF THE BOOK:
C
C      E. HAIRER, C. LUBICH, G. WANNER, GEOMETRIC NUMERICAL INTEGRATION,
C         STRUCTURE-PRESERVING ALGORITHMS FOR ODES.
C         SPRINGER SERIES IN COMPUT. MATH. 31, SPRINGER 2002.
C
C  AND IN THE PUBLICATION
C
C      E. HAIRER, M. HAIRER, GNI-CODES - MATLAB PROGRAMS FOR
C         GEOMETRIC NUMERICAL INTEGRATION.
C
C  INPUT..
C     N           DIMENSION OF Q AND F(X,Q) 
C
C     FCN         NAME (EXTERNAL) OF SUBROUTINE COMPUTING F(X,Q):
C                    SUBROUTINE FCN(N,X,Q,F,RPAR,IPAR)
C                    REAL*8 Q(N),F(N)
C                    F(1)=...   ETC.
C
C     NSTEP       NUMBER OF INTEGRATION STEPS
C                    CONSTANT STEP SIZE, H=(XEND-X)/NSTEP
C
C     X           INITIAL X-VALUE
C     P(N)        INITIAL VELOCITY VECTOR
C     Q(N)        INITIAL POSITION VECTOR
C     XEND        FINAL X-VALUE
C
C     METH        NUMBER OF METHOD (ORDER = METH/100)
C                    ORDER 2 : 201 (STOERMER/VERLET, MOD.INIT.VELOCITY)
C                    ORDER 4 : 401
C                    ORDER 8 : 801,802,803
C
C     SOLFIX      NAME (EXTERNAL) OF SUBROUTINE PROVIDING THE
C                 NUMERICAL SOLUTION DURING INTEGRATION. 
C                 IF IOUT=1, IT IS CALLED AFTER EVERY STEP.
C                 SUPPLY A DUMMY SUBROUTINE IF IOUT=0. 
C                    SUBROUTINE SOLFIX (NR,XOLD,X,P,Q,N,IRTRN,RPAR,IPAR)
C                    DOUBLE PRECISION X,P(N),Q(N)
C                      ....  
C                 SOLFIX FURNISHES THE SOLUTION "Q,P" AT THE NR-TH
C                    GRID-POINT "X" (INITIAL VALUE FOR NR=0).
C                 "XOLD" IS THE PRECEEDING GRID-POINT.
C                 "IRTRN" SERVES TO INTERRUPT THE INTEGRATION. IF IRTRN
C                    IS SET <0, RETURN TO THE CALLING PROGRAM.
C     IOUT        SWITCH FOR CALLING THE SUBROUTINE SOLFIX:
C                    IOUT=0: SUBROUTINE IS NEVER CALLED
C                    IOUT=1: SUBROUTINE IS AVAILABLE FOR OUTPUT.
C
C     RPAR(LR)    REAL PARAMETER ARRAY; LR MUST BE AT LEAST LR=10
C                    RPAR(1),...,RPAR(10) SERVE AS PARAMETERS FOR
C                    THE CODE. FURTHER VALUES CAN BE USED FOR DEFINING
C                    PARAMETERS IN THE PROBLEM
C     IPAR(LI)    INTEGER PARAMETER ARRAY; LI MUST BE AT LEAST LI=10
C                    IPAR(1),...,IPAR(10) SERVE AS PARAMETERS FOR
C                    THE CODE. FURTHER VALUES CAN BE USED FOR DEFINING
C                    PARAMETERS IN THE PROBLEM
C
C  OUTPUT..
C     P(N)        SOLUTION (VELOCITY) AT XEND
C     Q(N)        SOLUTION (POSITION) AT XEND
C-----------------------------------------------------------------------
C     SOPHISTICATED SETTING OF PARAMETERS 
C-----------------------------------------------------------------------
C     NONE
C-----------------------------------------------------------------------
      IMPLICIT REAL*8 (A-H,O-Z) 
      PARAMETER (ND=500)
      DOUBLE PRECISION Q0(ND),U(ND),Z0(ND),P(N),Q(N)
      DOUBLE PRECISION F1(ND),F2(ND),F3(ND),F4(ND),F5(ND)
      DOUBLE PRECISION F6(ND),F7(ND),F8(ND),F9(ND)
      DOUBLE PRECISION Q1(ND),Q2(ND),Q3(ND),Q4(ND),Q5(ND)
      DOUBLE PRECISION Q6(ND),Q7(ND),Q8(ND),Q9(ND)
      DOUBLE PRECISION Z1(ND),Z2(ND),Z3(ND),Z4(ND),Z5(ND)
      DOUBLE PRECISION Z6(ND),Z7(ND),Z8(ND),Z9(ND)
      DIMENSION IPAR(*),RPAR(*)
      EXTERNAL FCN,SOLFIX
      H=(XEND-X)/NSTEP
      KK=METH/100
      CALL SYCOE(METH,A1,A2,A3,A4,B1,B2,B3,B4,H,HM)
      IF (METH.LE.0) THEN
         METH=-METH
         WRITE (6,*) ' NO COEFFICIENTS AVAILABLE FOR METHOD ',METH
         RETURN
      END IF
      NR=0
      IRTRN=0
      DO I=1,N
        Q0(I)=Q(I)
      END DO
      IF (IOUT.GE.1) CALL SOLFIX (NR,X,X,P,Q,N,IRTRN,RPAR,IPAR)
      XRK=X
C
      DO IK=1,KK-1
        XDEND=XRK+H
        NSTRK=1
        METHRK=4
        IOUTRK=0
        CALL GNI_IRK2(N,FCN,NSTRK,XRK,P,Q,XDEND,
     &              METHRK,SOLFIX,IOUTRK,RPAR,IPAR)
        XRK=XDEND
        IF (IK.EQ.1) CALL PREPST(N,FCN,H,Q,Q0,Q1,Z0,F1,RPAR,IPAR)
        IF (IK.EQ.2) CALL PREPST(N,FCN,H,Q,Q1,Q2,Z1,F2,RPAR,IPAR)
        IF (IK.EQ.3) CALL PREPST(N,FCN,H,Q,Q2,Q3,Z2,F3,RPAR,IPAR)
        IF (IK.EQ.4) CALL PREPST(N,FCN,H,Q,Q3,Q4,Z3,F4,RPAR,IPAR)
        IF (IK.EQ.5) CALL PREPST(N,FCN,H,Q,Q4,Q5,Z4,F5,RPAR,IPAR)
        IF (IK.EQ.6) CALL PREPST(N,FCN,H,Q,Q5,Q6,Z5,F6,RPAR,IPAR)
        IF (IK.EQ.7) CALL PREPST(N,FCN,H,Q,Q6,Q7,Z6,F7,RPAR,IPAR)
        IF (IK.EQ.8) CALL PREPST(N,FCN,H,Q,Q7,Q8,Z7,F8,RPAR,IPAR)
        IF (IK.EQ.9) CALL PREPST(N,FCN,H,Q,Q8,Q9,Z8,F9,RPAR,IPAR)
        IF (IK.LT.KK/2) THEN
          NR=NR+1
          XOLD=X
          X=XDEND
          IF (IOUT.GE.1) CALL SOLFIX (NR,XOLD,X,P,Q,N,IRTRN,RPAR,IPAR)
        END IF
      END DO
C
      IF (KK.EQ.2) THEN
 22     CONTINUE
        CALL FCN(N,X,Q1,F1,RPAR,IPAR)
        DO I=1,N
          Z0(I)=Z0(I)+H*F1(I)
          Q2(I)=Q1(I)+H*Z0(I)
        END DO
        DO I=1,N
          Q(I)=Q1(I)
          P(I)=(Q2(I)-Q0(I))/(2*H)
          Q0(I)=Q1(I)
          Q1(I)=Q2(I)
        END DO
        XOLD=X
        X=X+H
        NR=NR+1
        IF (IOUT.GE.1) CALL SOLFIX (NR,XOLD,X,P,Q,N,IRTRN,RPAR,IPAR)
        IF (NR.LT.NSTEP.AND.IRTRN.GE.0) GOTO 22
      END IF
C
      IF (KK.EQ.4) THEN
        DO I=1,N
          U(I)=A1*(Z0(I)+Z2(I))+A2*Z1(I)
        END DO
 44     CONTINUE
        CALL FCN(N,X,Q3,F3,RPAR,IPAR)
        DO I=1,N
          U(I)=U(I)+(B1*(F1(I)+F3(I))+B2*F2(I))
          SUM=A1*Z1(I)+A2*Z2(I)
          Z3(I)=U(I)-SUM
          Q4(I)=Q3(I)+H*Z3(I)
        END DO
        DO I=1,N
          Q(I)=Q2(I)
          P(I)=(8*(Q3(I)-Q1(I))-(Q4(I)-Q0(I)))/(12*H)
          F1(I)=F2(I)
          F2(I)=F3(I)
          Z1(I)=Z2(I)
          Z2(I)=Z3(I)
          Q0(I)=Q1(I)
          Q1(I)=Q2(I)
          Q2(I)=Q3(I)
          Q3(I)=Q4(I)
        END DO
        XOLD=X
        X=X+H
        NR=NR+1
        IF (IOUT.GE.1) CALL SOLFIX (NR,XOLD,X,P,Q,N,IRTRN,RPAR,IPAR)
        IF (NR.LT.NSTEP.AND.IRTRN.GE.0) GOTO 44
      END IF
C
      IF (KK.EQ.6) THEN
        DO I=1,N
          U(I)=A1*(Z0(I)+Z4(I))+A2*(Z1(I)+Z3(I))+A3*Z2(I)
        END DO
 66     CONTINUE
        CALL FCN(N,X,Q5,F5,RPAR,IPAR)
        DO I=1,N
          U(I)=U(I)+(B1*(F1(I)+F5(I))+B2*(F2(I)+F4(I))+B3*F3(I))
          SUM=A1*Z1(I)+A2*(Z2(I)+Z4(I))+A3*Z3(I)
          Z5(I)=U(I)-SUM
          Q6(I)=Q5(I)+H*Z5(I)
        END DO
        DO I=1,N
          Q(I)=Q3(I)
          P(I)=(45*(Q4(I)-Q2(I))-9*(Q5(I)-Q1(I))+(Q6(I)-Q0(I)))/(60*H)
          F1(I)=F2(I)
          F2(I)=F3(I)
          F3(I)=F4(I)
          F4(I)=F5(I)
          Z1(I)=Z2(I)
          Z2(I)=Z3(I)
          Z3(I)=Z4(I)
          Z4(I)=Z5(I)
          Q0(I)=Q1(I)
          Q1(I)=Q2(I)
          Q2(I)=Q3(I)
          Q3(I)=Q4(I)
          Q4(I)=Q5(I)
          Q5(I)=Q6(I)
        END DO
        XOLD=X
        X=X+H
        NR=NR+1
        IF (IOUT.GE.1) CALL SOLFIX (NR,XOLD,X,P,Q,N,IRTRN,RPAR,IPAR)
        IF (NR.LT.NSTEP.AND.IRTRN.GE.0) GOTO 66
      END IF
C
      IF (KK.EQ.8) THEN
        DO I=1,N
          U(I)=A1*(Z0(I)+Z6(I))+A2*(Z1(I)+Z5(I))
     &        +A3*(Z2(I)+Z4(I))+A4*Z3(I)
        END DO
 88     CONTINUE
        CALL FCN(N,X,Q7,F7,RPAR,IPAR)
        DO I=1,N
          U(I)=U(I)+(B1*(F1(I)+F7(I))+B2*(F2(I)+F6(I))
     &        +B3*(F3(I)+F5(I))+B4*F4(I))
          SUM=A1*Z1(I)+A2*(Z2(I)+Z6(I))+A3*(Z3(I)+Z5(I))+A4*Z4(I)
          Z7(I)=U(I)-SUM
          Q8(I)=Q7(I)+H*Z7(I)
        END DO
        DO I=1,N
          Q(I)=Q4(I)
          P(I)=(672*(Q5(I)-Q3(I))-168*(Q6(I)-Q2(I))
     &        +32*(Q7(I)-Q1(I))-3*(Q8(I)-Q0(I)))/(840*H)
          F1(I)=F2(I)
          F2(I)=F3(I)
          F3(I)=F4(I)
          F4(I)=F5(I)
          F5(I)=F6(I)
          F6(I)=F7(I)
          Z1(I)=Z2(I)
          Z2(I)=Z3(I)
          Z3(I)=Z4(I)
          Z4(I)=Z5(I)
          Z5(I)=Z6(I)
          Z6(I)=Z7(I)
          Q0(I)=Q1(I)
          Q1(I)=Q2(I)
          Q2(I)=Q3(I)
          Q3(I)=Q4(I)
          Q4(I)=Q5(I)
          Q5(I)=Q6(I)
          Q6(I)=Q7(I)
          Q7(I)=Q8(I)
        END DO
        XOLD=X
        X=X+H
        NR=NR+1
        IF (IOUT.GE.1) CALL SOLFIX (NR,XOLD,X,P,Q,N,IRTRN,RPAR,IPAR)
        IF (NR.LT.NSTEP.AND.IRTRN.GE.0) GOTO 88
      END IF
      RETURN
      END
C
      SUBROUTINE PREPST(N,FCN,H,Q,QB,QE,Z,F,RPAR,IPAR)
      IMPLICIT REAL*8 (A-H,O-Z)
      DIMENSION Q(N),QB(N),QE(N),Z(N),F(N)
      DIMENSION IPAR(*),RPAR(*)
      DO I=1,N
        QE(I)=Q(I)
        Z(I)=(Q(I)-QB(I))/H
      END DO
      CALL FCN(N,X,QE,F,RPAR,IPAR)
      RETURN
      END
C
      SUBROUTINE SYCOE(METH,A1,A2,A3,A4,B1,B2,B3,B4,H,HM)
      IMPLICIT REAL*8 (A-H,O-Z)
      IF (METH.EQ.201) RETURN
      IF (METH.EQ.401) THEN
C --- METHOD ORDER 4
        A1=1.0D0
        A2=1.0D0
        HM=H/4.0D0
        B1=5*HM
        B2=2*HM
        RETURN
      END IF
      IF (METH.EQ.601) THEN
C --- METHOD ORDER 6
        A1=1.0D0
        A2=1.0D0
        A3=1.0D0
        HM=H/48.0D0
        B1=67*HM
        B2=-8*HM
        B3=122*HM
        RETURN
      END IF
      IF (METH.EQ.801) THEN
C --- METHOD SY8 (QUINLAN & TREMAINE 1990)
        A1=1.0D0
        A2=0.0D0
        A3=1.0D0
        A4=1.0D0
        HM=H/12096.0D0
        B1=17671*HM
        B2=-23622*HM
        B3=61449*HM
        B4=-50516*HM
        RETURN
      END IF
      IF (METH.EQ.802) THEN
C --- METHOD SY8B
        A1=1.0D0
        A2=2.0D0
        A3=3.0D0
        A4=3.5D0
        HM=H/120960.0D0
        B1=192481*HM
        B2=6582*HM
        B3=816783*HM
        B4=-156812*HM
        RETURN
      END IF
      IF (METH.EQ.803) THEN
C --- METHOD SY8C
        A1=1.0D0
        A2=1.0D0
        A3=1.0D0
        A4=1.0D0
        HM=H/8640.0D0
        B1=13207*HM
        B2=-8934*HM
        B3=42873*HM
        B4=-33812*HM
        RETURN
      END IF
      METH=-METH
      RETURN
      END
