ccfehop cal_lmm2
      include '../gni_irk2.f'
      include '../gni_lmm2.f'
      include '../problem.f'
      IMPLICIT REAL*8 (A-H,O-Z)
      PARAMETER (NDIM=100)
      DIMENSION P(NDIM),Q(NDIM),PEX(NDIM),QEX(NDIM),IPAR(20),RPAR(20)
      REAL TIME0,TIME1
      EXTERNAL EQUA,SOLFIX
C --- OPEN DATA FILES ----
      OPEN(12,FILE='kep_lmm2.dat')
      OPEN(15,FILE='sol_lmm2.dat')
      
      DO IPROB=1,4,3
        IFILE=IPROB+11
        REWIND (IFILE)
        METH=803
        WRITE (6,*) '     METHOD, PROBLEM  ',METH,IPROB
        IPAR(11)=IPROB
        IF (IPROB.EQ.1) RPAR(11)=0.6D0
        H=200.0D0
        IF (IPROB.EQ.1) H=0.05D0
        DO IH=1,40
          CALL PDATA(X,XEND,NDIM,N,Q,P,QEX,PEX,RPAR,IPAR)
          IF (IPROB.EQ.1) XEND=200*XEND
          NSTEP=(XEND-X)/H
C --- CALL OF THE METHOD
          DO I=1,10
            RPAR(I)=0.0D0
            IPAR(I)=0
          END DO
          IPAR(12)=0
          IOUT=0
          CALL CPU_TIME(TIME0)
          CALL GNI_LMM2(N,EQUA,NSTEP,X,P,Q,XEND,
     &                  METH,SOLFIX,IOUT,RPAR,IPAR)
          CALL CPU_TIME(TIME1)
C --- STATISTICS
          NFCN=IPAR(12)
          ERR=0.0D0
          DO I=1,N
            ERR=ERR+(Q(I)-QEX(I))**2+(P(I)-PEX(I))**2
          END DO
          ERR=SQRT(ERR)
          WRITE (6,90) NFCN,TIME1-TIME0,ERR
 90       FORMAT (1X,I8,F7.2,1X,2E24.16)
c
          XDAT=LOG10(max(1.d-16,ERR))
          ANFCN=NFCN
          YDATF=LOG10(ANFCN)
          YDATT=LOG10(max(1.e-16,TIME1-TIME0))
          WRITE(IFILE,956)XDAT,YDATF,YDATT
c          WRITE(6,957)XDAT,YDATF,YDATT,NSTEP
 956      FORMAT(3F11.6)
 957      FORMAT(3F11.6,I10)
c
          H=H*0.9D0
        END DO
        CLOSE (IFILE)
      END DO
      STOP
      END
C
      SUBROUTINE SOLFIX (NR,XOLD,X,P,Q,N,IRTRN,RPAR,IPAR)
      IMPLICIT REAL*8 (A-H,O-Z)
      DIMENSION Q(N),P(N)
      DIMENSION IPAR(20),RPAR(20)
      CALL HAMILTON (N,X,Q,P,HAMIL,RPAR,IPAR)
      IF (NR.EQ.0) THEN
        RPAR(12)=HAMIL
        RPAR(13)=0.0D0
      ELSE
        RPAR(13)=MAX(RPAR(13),ABS(RPAR(12)-HAMIL))
      END IF
      RETURN
      END
