ccfeh rigidbody
        include '../gni_comp.f'
        IMPLICIT DOUBLE PRECISION (A-H,O-Z)
        DIMENSION Q(4),AM(3),QI(4),PI(4),RPAR(20),IPAR(20)
        REAL TIME0,TIME1
        EXTERNAL QUATER,RATORI,POTENP,SOLFIX
C---
        AI1=0.5D0
        AI2=0.9D0
        AI3=1.0D0
        RPAR(11)=AI1
        RPAR(12)=AI2
        RPAR(13)=AI3
        IOUT=1
C---
        XEND=1.0D3
        H=0.6D0
        DO IH=1,5
          H=H/2.0d0
          NSTEP=XEND/H
          N=4
C --- INITIAL VALUES
          X=0.d0
          Q(1)=0.4D0
          Q(2)=0.2D0
          Q(3)=0.4D0
          Q(4)=SQRT(1.0D0-Q(1)**2-Q(2)**2-Q(3)**2)
          AM(1)=0.2D0
          AM(2)=1.0D0
          AM(3)=0.4D0
C ---
          METHC=45
C
          DO I=1,10
            RPAR(I)=0.0D0
            IPAR(I)=0
          END DO
          CALL CPU_TIME(TIME0)
C --- COMPOSITION WITH SPLITTING AS BSIC
C          CALL GNI_COMP(QUATER,N,POTENP,NSTEP,X,AM,Q,XEND,
C     &                  METHC,SOLFIX,IOUT,RPAR,IPAR)
C --- COMPOSITION WITH RATTLE AS BSIC
          CALL GNI_COMP(RATORI,N,POTENP,NSTEP,X,AM,Q,XEND,
     &                  METHC,SOLFIX,IOUT,RPAR,IPAR)
          CALL CPU_TIME(TIME1)
C ---
          WRITE (6,*) '  HAMILT = ',RPAR(14)
          WRITE (6,*) '  H = ',H,' ERR HAM = ',RPAR(15)/RPAR(14),
     &      '  TIME = ',TIME1-TIME0
C --- PRINT FINAL SOLUTION
        END DO
        STOP
        END
C
        SUBROUTINE HAMIL (Q,AM,HAM,RPAR,IPAR)
        IMPLICIT REAL*8 (A-H,O-Z) 
        DIMENSION Q(4),AM(3)
        DIMENSION IPAR(*),RPAR(*)
        HAM=AM(1)**2/RPAR(11)+AM(2)**2/RPAR(12)+AM(3)**2/RPAR(13)
        CALL POTEN(Q,POT,RPAR,IPAR)
        HAM=HAM/2.0D0+POT
        RETURN
        END 
c
        SUBROUTINE POTEN(Q,POT,RPAR,IPAR)
        IMPLICIT REAL*8 (A-H,O-Z) 
        DIMENSION Q(4)
        DIMENSION IPAR(*),RPAR(*)
        POT=Q(1)**2-Q(2)**2-Q(3)**2+Q(4)**2
C        POT=0.0D0
        RETURN
        END 
c
        SUBROUTINE POTENP(Q,POTP,RPAR,IPAR)
        IMPLICIT REAL*8 (A-H,O-Z) 
        DIMENSION Q(4),POTP(3)
        DIMENSION IPAR(*),RPAR(*)
        POTP(1)=-2*(Q(1)*Q(2)+Q(3)*Q(4))
        POTP(2)=-2*(Q(1)*Q(3)-Q(2)*Q(4))
C        POTP(1)=0.0d0
C        POTP(2)=0.0d0
        POTP(3)=0.0d0
        RETURN
        END 
C
      SUBROUTINE SOLFIX (NR,XOLD,X,AM,Q,N,IRTRN,RPAR,IPAR)
      IMPLICIT REAL*8 (A-H,O-Z)
      DIMENSION Q(N),AM(N)
      DIMENSION IPAR(20),RPAR(20)
        CALL HAMIL (Q,AM,HAM,RPAR,IPAR)
        IF(NR.EQ.0) THEN
          RPAR(14)=HAM
          RPAR(15)=0.0D0
        ELSE
          RPAR(15)=MAX(ABS(HAM-RPAR(14)),RPAR(15))
          IF (ABS(HAM-RPAR(14)).GE.1.D0) THEN
             WRITE (6,*) ' EXIT IN SOLFIX '
             IRTRN=-1
          END IF
        END IF
      RETURN
      END
C
      SUBROUTINE QUATER (N,X,AM,Q,EAM,EQ,POTENP,HA,HB,HC,
     &                   FIRST,LAST,RPAR,IPAR)
C--- SPLITTING METHOD FOR RIGID BODY SIMULATIONS
C--- ORTHOGONAL MATRICES ARE REPRESENTED BY QUATERNIONS
      IMPLICIT REAL*8 (A-H,O-Z) 
      DOUBLE PRECISION Q(4),AM(3),EAM(3),EQ(4),POTP(3)
      DIMENSION IPAR(20),RPAR(20)
      COMMON /INTERN/FAC01,FAC02,FAC03
      LOGICAL FIRST,LAST
      AI1=RPAR(11)
      AI2=RPAR(12)
      AI3=RPAR(13)
      IF (FIRST) THEN
        CALL POTENP(Q,POTP,RPAR,IPAR)
C ---            HALF-STEP POTENTIAL
        AM1=AM(1)-HA*POTP(1)
        AM2=AM(2)-HA*POTP(2)
        AM3=AM(3)-HA*POTP(3)
        FAC01=(AI2-AI3)/(AI2*AI3)
        FAC02=0.5D0*(AI2-AI1)/(AI2*AI1)
        FAC03=1.0D0/AI2
      ELSE
        AM1=AM(1)
        AM2=AM(2)
        AM3=AM(3)
      END IF
      HD2=HB/2.0D0
      FAC1=HD2*FAC01
      FAC2=HD2*FAC02
      FAC3=HD2*FAC03
C ---            HALF-STEP "R"
      ALPH=FAC2*AM1
      SA=SIN(ALPH)
      CA=COS(ALPH)
      CAP=1-2*SA**2
      SAP=2*CA*SA
      SAVE=CAP*AM2+SAP*AM3
      AM3=-SAP*AM2+CAP*AM3
      AM2=SAVE
C ---
      Q0=CA*Q(1)-SA*Q(2)
      Q1=CA*Q(2)+SA*Q(1)
      Q2=CA*Q(3)+SA*Q(4)
      Q3=CA*Q(4)-SA*Q(3)
C ---            STEP "S"
      BETA=FAC1*AM3
      ANORM=SQRT(AM1**2+AM2**2+AM3**2)
      GAM=ANORM*FAC3
      CA=COS(GAM)
      SA=SIN(GAM)
      CB=COS(BETA)
      SB=SIN(BETA)
      CBP=1-2*SB**2
      SBP=2*CB*SB
      FAC=SA/ANORM
      V1=AM1*FAC
      V2=AM2*FAC
      V3=AM3*FAC
C ---
      QP0=CA*Q0-V1*Q1-V2*Q2-V3*Q3
      QP1=CA*Q1+V1*Q0+V3*Q2-V2*Q3
      QP2=CA*Q2+V2*Q0+V1*Q3-V3*Q1
      QP3=CA*Q3+V3*Q0+V2*Q1-V1*Q2
C ---
      Q0=CB*QP0-SB*QP3
      Q1=CB*QP1+SB*QP2
      Q2=CB*QP2-SB*QP1
      Q3=CB*QP3+SB*QP0
C ---
      SAVE=CBP*AM1+SBP*AM2
      AM2=-SBP*AM1+CBP*AM2
      AM1=SAVE
C ---            HALF-STEP "R"
      ALPH=FAC2*AM1
      SA=SIN(ALPH)
      CA=COS(ALPH)
      CAP=1-2*SA**2
      SAP=2*CA*SA
      SAVE=CAP*AM2+SAP*AM3
      AM3=-SAP*AM2+CAP*AM3
      AM2=SAVE
C ---
      Q(2)=CA*Q1+SA*Q0
      Q(3)=CA*Q2+SA*Q3
      Q(4)=CA*Q3-SA*Q2
      Q1=CA*Q0-SA*Q1
C ---       PROJECTION
      SUM=Q(2)**2+Q(3)**2+Q(4)**2
      Q(1)=(Q1**2-SUM+1.0D0)/(2*Q1)
C ---            HALF-STEP POTENTIAL
      CALL POTENP(Q,POTP,RPAR,IPAR)
      AM(1)=AM1-HC*POTP(1)
      AM(2)=AM2-HC*POTP(2)
      AM(3)=AM3-HC*POTP(3)
      RETURN
      END
C
      SUBROUTINE RATORI (N,X,AM,Q,EAM,EQ,POTENP,HA,HB,HC,
     &                   FIRST,LAST,RPAR,IPAR)
C--- RATTLE APPLIED TO FORMULATION WITH P AND Q (MATRICES)
C--- ORTHOGONAL MATRICES ARE REPRESENTED BY QUATERNIONS
      IMPLICIT REAL*8 (A-H,O-Z) 
      DOUBLE PRECISION Q(4),AM(3),EAM(3),EQ(4),POTP(3)
      DIMENSION IPAR(20),RPAR(20)
      LOGICAL FIRST,LAST
      AI1=RPAR(11)
      AI2=RPAR(12)
      AI3=RPAR(13)
      IF (FIRST) THEN
        CALL POTENP(Q,POTP,RPAR,IPAR)
        AM1=AM(1)-HA*POTP(1)
        AM2=AM(2)-HA*POTP(2)
        AM3=AM(3)-HA*POTP(3)
      ELSE
        AM1=AM(1)
        AM2=AM(2)
        AM3=AM(3)
      END IF
      EPS=1.D-16/HB
      HD2=HB/2.0D0
      VDH=4.0D0/HB
      FAC1=(AI2-AI3)/AI1
      FAC2=(AI3-AI1)/AI2
      FAC3=(AI1-AI2)/AI3
      FAD1=(AI2-AI3)*VDH
      FAD2=(AI3-AI1)*VDH
      FAD3=(AI1-AI2)*VDH
C ---      SOLVE FOR INTERNAL STAGE
      BM1=AM1*HD2/AI1
      BM2=AM2*HD2/AI2
      BM3=AM3*HD2/AI3
      SUM=BM1**2+BM2**2+BM3**2
      CM0=1.0D0-SUM/2.0D0
      CM1=(BM1+FAC1*BM2*BM3)/CM0
      CM2=(BM2+FAC2*CM1*BM3)/CM0
      CM3=(BM3+FAC3*CM1*CM2)/CM0
      DO I=1,10
        SUM=CM1**2+CM2**2+CM3**2
        STE=SUM+CM0**2-1.0D0
        CM0=(CM0**2-SUM+1.0D0)/(2*CM0)
        CM1=(BM1+FAC1*CM2*CM3)/CM0
        CM2=(BM2+FAC2*CM1*CM3)/CM0
        CM3=(BM3+FAC3*CM1*CM2)/CM0
        IF (ABS(STE).LT.EPS) GOTO 22
      END DO
 22   CONTINUE
C ---      UPDATE Q
      Q1=Q(1)
      Q2=Q(2)
      Q3=Q(3)
      Q4=Q(4)
      Q(2)=CM0*Q2+CM1*Q1+CM3*Q3-CM2*Q4
      Q(3)=CM0*Q3+CM2*Q1+CM1*Q4-CM3*Q2
      Q(4)=CM0*Q4+CM3*Q1+CM2*Q2-CM1*Q3
      Q1=CM0*Q1-CM1*Q2-CM2*Q3-CM3*Q4
C ---       PROJECTION
      SUM=Q(2)**2+Q(3)**2+Q(4)**2
      Q(1)=(Q1**2-SUM+1.0D0)/(2*Q1)
C ---       UPDATE M
      CALL POTENP(Q,POTP,RPAR,IPAR)
      AM(1)=AM1-HC*POTP(1)+FAD1*CM2*CM3
      AM(2)=AM2-HC*POTP(2)+FAD2*CM1*CM3
      AM(3)=AM3-HC*POTP(3)+FAD3*CM1*CM2
      RETURN
      END
