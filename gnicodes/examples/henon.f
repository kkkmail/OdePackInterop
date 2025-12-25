C * * * * * * * * * * * * * * * * * * * * * * * * *
C --- DRIVER FOR DOPRI5 ON VAN DER POL'S EQUATION
C * * * * * * * * * * * * * * * * * * * * * * * * *
ccfeh henon
       include '../gni_irk2.f'
       include '../gni_lmm2.f'
       include '../gni_comp.f'
       IMPLICIT REAL*8 (A-H,O-Z)
       PARAMETER (NDGL=2)
       DIMENSION P(NDGL),Q(NDGL),IPAR(20),RPAR(20)
       EXTERNAL HENON,SOLFIX,STVERL
       DO IGNI=1,3
C --- DIMENSION OF THE SYSTEM
         N=2
C --- INITIAL VALUES
         X=0.0D0
         VAL=0.18D0
         P(1)=VAL
         P(2)=VAL
         Q(1)=VAL
         Q(2)=VAL
         U=0.5D0*(Q(1)**2+Q(2)**2)+Q(1)**2*Q(2)-Q(2)**3/3.0D0
         T=0.5D0*(P(1)**2+P(2)**2)
         WRITE (6,*) ' HAMIL ',T+U
C --- ENDPOINT OF INTEGRATION
         X=0.0D0
         XEND=20.0D0
         IOUT=1
         IF (IGNI.EQ.1) H=0.22D0
         IF (IGNI.EQ.2) H=1.5D0
         IF (IGNI.EQ.3) H=1.2D0
         NSTEP=(XEND-X)/H
         DO I=1,20
           IPAR(I)=0
           RPAR(I)=0.0D0
         END DO
         WRITE (6,*) ' NSTEP ',NSTEP
         IF (IGNI.EQ.1) THEN
           METH=803
           CALL GNI_LMM2(N,HENON,NSTEP,X,P,Q,XEND,
     &                  METH,SOLFIX,IOUT,RPAR,IPAR)
         END IF
         IF (IGNI.EQ.2) THEN
           METH=4
           CALL GNI_IRK2(N,HENON,NSTEP,X,P,Q,XEND,
     &                  METH,SOLFIX,IOUT,RPAR,IPAR)
         END IF
         IF (IGNI.EQ.3) THEN
           METH=817
           CALL GNI_COMP(STVERL,N,HENON,NSTEP,X,P,Q,XEND,
     &                  METH,SOLFIX,IOUT,RPAR,IPAR)
         END IF
C --- PRINT FINAL SOLUTION
         WRITE (6,99) X,Q(1),Q(2)
 99      FORMAT(1X,'X =',F10.2,'    Q =',2E18.10)
         END DO
         STOP
         END
C
        SUBROUTINE SOLFIX (NR,XOLD,X,P,Q,N,IRTRN,RPAR,IPAR)
        IMPLICIT REAL*8 (A-H,O-Z)
        DIMENSION Q(N),P(N),IPAR(20),RPAR(20)
        REAL XD,YD
        COMMON /INTERN/XOUT,EVOLD,HAM0,HAMMAX,QOLD(10),POLD(10)
        IF (NR.EQ.0) EVOLD=Q(1)
        EVEN=Q(1)
        IF (EVOLD*EVEN.LT.0.0D0) THEN
           CALL ZEROEV(EVOLD,EVEN,EVENT,N,P,Q,POLD,QOLD,XOLD,X,XP)
           CALL POLINT(N,P,Q,POLD,QOLD,XOLD,X,XP,2,POLQ,POLP)
C --- (POLP,POLQ) IS THE POINT OF THE POINCARE SECTION
           WRITE (6,88) XP,POLP,POLQ
 88        FORMAT(1X,3F15.10)
        END IF
        EVOLD=EVEN
        DO I=1,N
          QOLD(I)=Q(I)
          POLD(I)=P(I)
        END DO
        RETURN
        END
C
        SUBROUTINE ZEROEV(EVOLD,EVEN,EVENT,N,P,Q,POLD,QOLD,X0,X1,XM)
C --- ROOT FINDING BY BISECTION
        IMPLICIT REAL*8 (A-H,O-Z)
        DIMENSION Q(N),P(N),QOLD(10),POLD(10)
        XM0=X0
        XM1=X1
 33     XM=(XM1+XM0)/2.0D0
        CALL POLINT(N,P,Q,POLD,QOLD,X0,X1,XM,1,POLQ,POLP)
        IF (EVOLD*POLQ.LT.0.0D0) THEN
           XM1=XM
        ELSE
           XM0=XM
        END IF
        IF (XM1-XM0.GE.1.0D-10) GOTO 33
        RETURN
        END
C
        SUBROUTINE POLINT(N,P,Q,POLD,QOLD,X0,X1,X,I,POLQ,POLP)
C --- HERMITE INTERPOLATION
        IMPLICIT REAL*8 (A-H,O-Z)
        DIMENSION Q(N),P(N),QOLD(10),POLD(10)
        H=X1-X0
        H2=H*H
        DIFF=(Q(I)-QOLD(I))/H
        XM0=X-X0
        XM1=X-X1
        P0DIF=POLD(I)-DIFF
        P1DIF=P(I)-DIFF
        POLQ=QOLD(I)+XM0*(DIFF+XM1*(P1DIF*XM0+P0DIF*XM1)/H2)
        POLP=DIFF+(XM0*(2*XM1+XM0)*P1DIF+XM1*(2*XM0+XM1)*P0DIF)/H2
        RETURN
        END
C
        SUBROUTINE HENON(N,X,Q,F,RPAR,IPAR)
C --- RIGHT-HAND SIDE OF THE HENON-HEILES PROBLEM
        IMPLICIT REAL*8 (A-H,O-Z)
        DIMENSION Q(N),F(N)
        F(1)=-Q(1)*(1+2*Q(2))
        F(2)=-Q(2)*(1-Q(2))-Q(1)**2
        RETURN
        END 

