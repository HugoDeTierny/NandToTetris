// Fill the screen with black when a key is pressed
// Fill the screen with whitre when no key is pressed

// Idea
// 2 Portion 
// 	 - First check if a key is pressed
//   - Fill the screen  white = 0 or black = -1

// Check if key is pressed


// Pseudo Code
// 		1.
//			


//		2. Remplissage de l'écran
//			for(int i = 0; i<=8191 ; i++)
//				RAM[SCREEN+i] = -1;
//      3. Vidage de l'écran
//			for(int i = 0; i<=8191 ; i++)
//				RAM[SCREEN+i] = 0;


@ispressed
M=0

@8191
D=A
@screenSize
M=D

@ISPRESSED
0;JMP

(FILLSCREEN)
	@i
	M=0
	(STARTFILLLOOP)
	// jump si i > screeSize (8192)
	@i
	D=M
	@screenSize
	D=D-M
	@ISPRESSED
	D;JGT
	
	// FillScreen 
	@i
	D=M
	@SCREEN
	D=D+A
	// A est l'adresse de l'écran actuel
	A=D
	// -1 = 11111111 8 black pixel
	M=-1
	//Incrémente i
	@i
	M=M+1
	// retourne au début de la loop
	@STARTFILLLOOP
	0;JMP
	
(EMPTYSCREEN)
	@i
	M=0
	(STARTBLANCKLOOP)
	// jump si i > screeSize (8192)
	@i
	D=M
	@screenSize
	D=D-M
	@ISPRESSED
	D;JGT
	
	// FillScreen 
	@i
	D=M
	@SCREEN
	D=D+A
	// A est l'adresse de l'écran actuel
	A=D
	// 0 = 00000000 8 white pixel
	M=0
	//Incrémente i
	@i
	M=M+1
	// retourne au début de la loop
	@STARTBLANCKLOOP
	0;JMP


(ISPRESSED)
	@KBD
	D=M
	@tmpkeyboard
	M=D
	@lastvalue
	D=D-M
	@SAVEVALUE
	D;JNE
	@ISPRESSED
	0;JMP
		
(SAVEVALUE)
	@tmpkeyboard
	D=M
	@lastvalue
	M=D
	@FILLSCREEN
	D;JGT
	@EMPTYSCREEN
	D;JEQ
	
	
