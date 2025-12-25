ccfeh twobdsph
      include '../gni_comp.f'
      IMPLICIT REAL*8 (A-H,O-Z)
      PARAMETER (NDIM=100)
      DIMENSION P(NDIM),Q(NDIM),PEX(NDIM),QEX(NDIM)
      DIMENSION RPAR(20),IPAR(20)
      EXTERNAL GRPOT,SOLFIX,RATTWO
      
      METHC=817
      WRITE (6,*) '     METHOD  ',METHC
      H=0.002D0
      XEND=2.0D1
      NSTEP=XEND/H
C --- INITIAL VALUES
      CALL INDATA(X,XEND,NDIM,NN,Q,P,QEX,PEX,RPAR,IPAR)
      IOUT=1
      CALL GNI_COMP(RATTWO,NN,GRPOT,NSTEP,X,P,Q,XEND,
     &                METHC,SOLFIX,IOUT,RPAR,IPAR)
      WRITE (6,*) ' ERROR IN HAMIL ',RPAR(12)
      STOP
      END
C
      SUBROUTINE SOLFIX (NR,XOLD,X,P,Q,N,IRTRN,RPAR,IPAR)
      IMPLICIT REAL*8 (A-H,O-Z)
      DIMENSION Q(N),P(N),RPAR(20),IPAR(20)
      COMMON /INTERN/HAMIL0
      CALL HAMILTON (N,X,Q,P,HAMIL,RPAR,IPAR)
      IF (NR.EQ.0) THEN
         RPAR(11)=HAMIL
         WRITE (6,*) ' HAMILTONIAN ',HAMIL
         RPAR(12)=0.0D0
      ELSE
         RPAR(12)=MAX(RPAR(12),ABS(HAMIL-RPAR(11)))
         PROD=Q(1)*Q(4)+Q(2)*Q(5)+Q(3)*Q(6)
C         WRITE (6,*) NR,HAMAX,PROD
      END IF
      RETURN
      END
C
C
      SUBROUTINE INDATA(X,XEND,NDIM,N,Q,P,QEX,PEX,RPAR,IPAR)
      IMPLICIT REAL*8 (A-H,O-Z)
      DIMENSION Q(NDIM),P(NDIM),QEX(NDIM),PEX(NDIM)
        N=6
        X=0.D0
        PHI1=1.3D0
        THE1=2.1D0
        Q(1)=COS(PHI1)*SIN(THE1)
        Q(2)=SIN(PHI1)*SIN(THE1)
        Q(3)=COS(THE1)
        PHI2=-2.1D0
        THE2=-1.1D0
        Q(4)=COS(PHI2)*SIN(THE2)
        Q(5)=SIN(PHI2)*SIN(THE2)
        Q(6)=COS(THE2)
        DPHI1=1.2D0
        DTHE1=0.1D0
        P(1)=-DPHI1*SIN(PHI1)*SIN(THE1)+DTHE1*COS(PHI1)*COS(THE1)
        P(2)=DPHI1*COS(PHI1)*SIN(THE1)+DTHE1*SIN(PHI1)*COS(THE1)
        P(3)=-DTHE1*SIN(THE1)
        DPHI2=0.1D0
        DTHE2=-0.5D0
        P(4)=-DPHI2*SIN(PHI2)*SIN(THE2)+DTHE2*COS(PHI2)*COS(THE2)
        P(5)=DPHI2*COS(PHI2)*SIN(THE2)+DTHE2*SIN(PHI2)*COS(THE2)
        P(6)=-DTHE2*SIN(THE2)
      RETURN
      END
C
      SUBROUTINE HAMILTON (N,X,Q,P,HAMIL,RPAR,IPAR)
      IMPLICIT DOUBLE PRECISION (A-H,O-Z) 
      DOUBLE PRECISION Q(N),P(N)
      HAMIL=0.0D0
      DO I=1,N
        HAMIL=HAMIL+P(I)**2
      END DO
      CALL POTEN (N,X,Q,POT,RPAR,IPAR)
      HAMIL=HAMIL*0.5D0+POT
      RETURN
      END 
C
      SUBROUTINE POTEN (N,X,Q,POT,RPAR,IPAR)
      IMPLICIT DOUBLE PRECISION (A-H,O-Z) 
      DOUBLE PRECISION Q(N)
      PROD=0.0D0
      DO I=1,3
        PROD=PROD+Q(I)*Q(I+3)
      END DO
      POT=-1.0D0/ACOS(PROD)
C      POT=Q(3)+Q(6)
      RETURN
      END 
C
      SUBROUTINE GRPOT(N,X,Q,F,RPAR,IPAR)
      IMPLICIT REAL*8 (A-H,O-Z) 
      DIMENSION Q(N),F(N)
      PROD=0.0D0
      DO I=1,3
        PROD=PROD+Q(I)*Q(I+3)
      END DO
      FAC=ACOS(PROD)**2*SQRT(1.0D0-PROD**2)
      DO I=1,3
        II=I+3
        F(I)=-Q(II)/FAC
        F(II)=-Q(I)/FAC
      END DO
      RETURN
      END 
C
      SUBROUTINE RATTWO (N,X,P,Q,EP,EQ,FCN,HA,HB,HC,
     &                   FIRST,LAST,RPAR,IPAR) 
C--- RATTLE FOR THE TWO-BODY PROBLEM ON THE SPHERE
C--- FCN(N,X,Q,F,...) COMPUTES THE GRADIENT OF THE POTENTIAL
      IMPLICIT REAL*8 (A-H,O-Z)
      DIMENSION P(N),Q(N),EP(N),EQ(N),F(100)
      LOGICAL FIRST,LAST
C
      CALL FCN(N,X,Q,F,RPAR,IPAR)
      DO I=1,N
        EP(I)=P(I)-HA*F(I)
        EQ(I)=Q(I)+HB*EP(I)
      END DO
      EE1=0.0D0
      EQ1=0.0D0
      EE2=0.0D0
      EQ2=0.0D0
      DO I=1,3
        EE1=EE1+EQ(I)**2
        EQ1=EQ1+EQ(I)*Q(I)
        II=I+3
        EE2=EE2+EQ(II)**2
        EQ2=EQ2+EQ(II)*Q(II)
      END DO
      BET1=1.0D0-EE1
      ALAM1=-BET1/(HB*(EQ1+SQRT(BET1+EQ1**2)))
      BET2=1.0D0-EE2
      ALAM2=-BET2/(HB*(EQ2+SQRT(BET2+EQ2**2)))
      DO I=1,3
        II=I+3
        P(I)=EP(I)-ALAM1*Q(I)
        P(II)=EP(II)-ALAM2*Q(II)
        Q(I)=Q(I)+HB*P(I)
        Q(II)=Q(II)+HB*P(II)
      END DO
C
      IF (LAST) THEN
        CALL FCN(N,X,Q,F,RPAR,IPAR)
        AMU1=0.0D0
        AMU2=0.0D0
        DO I=1,3
          II=I+3
          P(I)=P(I)-HC*F(I)
          P(II)=P(II)-HC*F(II)
          AMU1=AMU1+P(I)*Q(I)
          AMU2=AMU2+P(II)*Q(II)
        END DO
        DO I=1,3
          II=I+3
          P(I)=P(I)-AMU1*Q(I)
          P(II)=P(II)-AMU2*Q(II)
        END DO
      END IF
      RETURN
      END
