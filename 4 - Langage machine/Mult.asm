// Multiply R0 and R1, set result to R2

// Idea
// We do not have a multiply operation so we need to add R1 R0 Time to itself
// Edge case negative value
// always loop at the end

// Pseudo Code
// if (R1 == 0 || R0 == 0)
// 		R2 = 0
//		Jmp to end
// n = R1;
// isNegative = (check if R1 is a positiv integer)
// For( i=0, i<R1 (n) ; i++){
// 		Si (Negatif)
//			sum = sum - R0
//		Si (Positif)
//			sum = sum + R0
// }
// R2 = sum

@R2
M=0

// R0 = 0 Or R1 == 0 Part
@R0
D=M
@END
D;JEQ
@R1
D=M
@END
D;JEQ



@i
M=0

// n = |R1|
@R1
D=M
@n
M=D
@LOOP
D;JGT
@n
M=-M

(LOOP)
	// Part exit if (i = n)
	@i
	D=M
	@n
	D=M-D
	@END
	D;JEQ
	
	@R1
	D=M
	@NEGATIVE
	D;JLT
	
	//Cas positif
	@R0
	D=M
	@R2
	M=D+M
	@INCREMENT
	0;JMP
	
(NEGATIVE)
	@R0
	D=M
	@R2
	M=M-D
	@INCREMENT
	0;JMP
	
	// i = i+1
(INCREMENT)
	@i
	M=M+1
	@LOOP
	0;JMP
	



(END)
	@END
	0;JMP