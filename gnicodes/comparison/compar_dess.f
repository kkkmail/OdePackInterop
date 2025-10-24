cfggg compar_dess
C  this program uses GGG.f
      DIMENSION YDES0(1000),XDESF0(1000),XDEST0(1000)
      DIMENSION YDES4(1000),XDESF4(1000),XDEST4(1000)
      DIMENSION YDES6(1000),XDESF6(1000),XDEST6(1000)
      common/sizes/dim(4)
      data dim/5.1,5.0,0.8,0.6/
c
      call begin_ggg('compar_dess')
C
C --- OPEN DATA FILES ----
      OPEN(11,FILE='kep_comp.dat')
      OPEN(12,FILE='kep_irk2.dat')
      OPEN(13,FILE='kep_lmm2.dat')
      OPEN(14,FILE='sol_comp.dat')
      OPEN(15,FILE='sol_irk2.dat')
      OPEN(16,FILE='sol_lmm2.dat')

c --- READ DATA -----
      DO IPROB=1,4,3
        write(6,*)'  iprob=',iprob
        IFILE=IPROB+11
        DO I=1,500
          READ(IFILE,*,END=21)YDES0(I),XDESF0(I),XDEST0(I)
          NPT0=I
        END DO
  21    CONTINUE
        IF (IPROB.EQ.1) NPT0=NPT0
        IF (IPROB.EQ.4) NPT0=NPT0-10
        write (6,*) 'npt0 ',npt0
        IFILE=IPROB+12
        DO I=1,500
          READ(IFILE,*,END=22)YDES4(I),XDESF4(I),XDEST4(I)
          NPT4=I
        END DO
  22    CONTINUE
        IF (IPROB.EQ.1) NPT4=NPT4
        IF (IPROB.EQ.4) NPT4=NPT4-11
        write (6,*) 'npt4 ',npt4
        IFILE=IPROB+10
        DO I=1,500
          READ(IFILE,*,END=23)YDES6(I),XDESF6(I),XDEST6(I)
          NPT6=I
        END DO
 23     CONTINUE
        IF (IPROB.EQ.1) NPT6=NPT6
        IF (IPROB.EQ.4) NPT6=NPT6
        write (6,*) 'npt6 ',npt6
C ---- LOOP PICTURES -----
        DO IPIC=1,2
          call scale_char(0.75)
          call thick_pixel(3)
C --- KEPLER ---
          IF(IPROB.EQ.1)THEN
            ymax=1.0
            ymin=-12.2
            IF (IPIC.EQ.1) THEN
              xmin=4.3
              xmax=6.9
            END IF
            IF (IPIC.EQ.2) THEN
              xmin=-1.5
              xmax=0.5
            END IF
          END IF
C ---- SOLAR ----
          IF(IPROB.EQ.4)THEN
            ymax=-1.
            ymin=-11.9
            IF (IPIC.EQ.1) THEN
              xmin=3.5
              xmax=5.5
            END IF
            IF (IPIC.EQ.2) THEN
              xmin=-1.1
              xmax= 0.6
            END IF
          END IF
          call masog(xmin,xmax,ymin,ymax)
          call framg
          call logax_x(0.,YMIN,xmin,xmax,1,1)
          call logax_y(xmin,0.,ymin,ymax,3,1)
C ---- DRAW LINES ----
          call thick_pixel(7)
          IF(IPIC.EQ.1)THEN
            call select_line_type(2)
            CALL LING(XDESF0,YDES0,NPT0)
            call select_line_type(7)
            CALL LING(XDESF4,YDES4,NPT4)
            CALL LING(XDESF6,YDES6,NPT6)
            call thick_pixel(3)
            AMM=0.6
            DO I=1,NPT0
              CALL BUBBLE(XDESF0(I),YDES0(I),AMM,0.9,'circle')
            END DO
            DO I=1,NPT4
              CALL BUBBLE(XDESF4(I),YDES4(I),AMM,0.5,'square')
            END DO
            DO I=1,NPT6
              CALL BUBBLE(XDESF6(I),YDES6(I),AMM,0.5,'mhex')
            END DO
          END IF
          IF(IPIC.EQ.2)THEN
            call select_line_type(2)
            CALL LING(XDEST0,YDES0,NPT0)
            call select_line_type(7)
            CALL LING(XDEST4,YDES4,NPT4)
            CALL LING(XDEST6,YDES6,NPT6)
            call thick_pixel(3)
            AMM=0.6
            DO I=1,NPT0
              CALL BUBBLE(XDEST0(I),YDES0(I),AMM,0.9,'circle')
            END DO
            DO I=1,NPT4
              CALL BUBBLE(XDEST4(I),YDES4(I),AMM,0.5,'square')
            END DO
            DO I=1,NPT6
              CALL BUBBLE(XDEST6(I),YDES6(I),AMM,0.5,'mhex')
            END DO
          END IF
C ---- TEXTES ------
          CALL TEXT_LATEX_RG(XMIN,(YMAX+2*YMIN)/3.,0.5,3.0,90.,1.
     &,'error')
          IF(IPROB.EQ.1)THEN
            CALL TEXT_LATEX((XMIN+XMAX)/2.,YMAX,0.3,1.4
     &,'\normalsize Kepler')
          END IF
          IF(IPROB.EQ.4)THEN
            CALL TEXT_LATEX((XMIN+XMAX)/2.,YMAX,-0.4,1.4
     &,'\normalsize Solar')
          END IF
          IF(IPIC.EQ.1)THEN
            CALL TEXT_LATEX((2*XMIN+XMAX)/3.,YMIN,0.8,-1.1,
     &'fcn.\,eval.')
          END IF
          IF(IPIC.EQ.2)THEN
            CALL TEXT_LATEX((2*XMIN+XMAX)/3.,YMIN,0.8,-0.6,
     &'cpu time')
          END IF
C ----- FIRST PICTURE ---
          IF(IPROB.EQ.1.AND.IPIC.EQ.1)THEN
            CALL TEXT_RELAD_LATEX(0.66,0.33,0.5,0.5,'comp')
            CALL TEXT_RELAD_LATEX(0.4,0.2,0.5,0.5,'irk2')
            CALL TEXT_RELAD_LATEX(0.25,0.45,0.5,0.5,'lmm2')
          END IF 
C ----- SECOND PICTURE ---
          IF(IPROB.EQ.1.AND.IPIC.EQ.2)THEN
            CALL TEXT_RELAD_LATEX(0.6,0.56,0.5,0.5,'comp')
            CALL TEXT_RELAD_LATEX(0.4,0.36,0.5,0.5,'irk2')
            CALL TEXT_RELAD_LATEX(0.15,0.6,0.5,0.5,'lmm2')
          END IF 
C ----- THIRD PICTURE ---
          IF(IPROB.EQ.4.AND.IPIC.EQ.1)THEN
            CALL TEXT_RELAD_LATEX(0.82,0.40,0.5,0.5,'comp')
            CALL TEXT_RELAD_LATEX(0.52,0.36,0.5,0.5,'irk2')
            CALL TEXT_RELAD_LATEX(0.32,0.6,0.5,0.5,'lmm2')
          END IF 
C ----- FOURTH PICTURE ---
          IF(IPROB.EQ.4.AND.IPIC.EQ.2)THEN
            CALL TEXT_RELAD_LATEX(0.44,0.58,0.5,0.5,'comp')
            CALL TEXT_RELAD_LATEX(0.68,0.75,0.5,0.5,'irk2')
            CALL TEXT_RELAD_LATEX(0.32,0.25,0.5,0.5,'lmm2')
          END IF 
        END DO
      END DO
      call greater_boundingbox (0.7,-0.1,0.3,0.)
      call end_ggg
      STOP
      END
