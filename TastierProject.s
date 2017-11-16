	AREA	TastierProject, CODE, READONLY

    IMPORT  TastierDiv
	IMPORT	TastierMod
	IMPORT	TastierReadInt
	IMPORT	TastierPrintInt
	IMPORT	TastierPrintIntLf
	IMPORT	TastierPrintTrue
	IMPORT	TastierPrintTrueLf
	IMPORT	TastierPrintFalse
    IMPORT	TastierPrintFalseLf
    IMPORT  TastierPrintString
    
; Entry point called from C runtime __main
	EXPORT	main

; Preserve 8-byte stack alignment for external routines
	PRESERVE8

; Register names
BP  RN 10	; pointer to stack base
TOP RN 11	; pointer to top of stack

main
; Initialization
	LDR		R4, =globals
	LDR 	BP, =stack		; address of stack base
	LDR 	TOP, =stack+16	; address of top of stack frame
	B		Main
MainBody
    LDR     R5, =3
    ADD     R2, BP, #16
    LDR     R1, =0
    ADD     R2, R2, R1, LSL #2
    STR     R5, [R2]        ; Store in b
L2
;Load in the Case primary
    LDR     R5, =0
    MOV     R6, R5
;Load in Switch obj 
    ADD     R2, BP, #16
    LDR     R1, =0
    ADD     R2, R2, R1, LSL #2
    LDR     R5, [R2]        ; Load b
;Comapare Switch obj with Case primary
    CMP     R5, R6
    MOVEQ   R5, #1
    MOVNE   R5, #0
    MOVS    R5, R5          ; reset Z flag in CPSR
    BEQ     L4              ; jump on condition false
L3
    ADD     R2, BP, #16
    LDR     R1, =0
    ADD     R2, R2, R1, LSL #2
    LDR     R7, [R2]        ; Load b
    LDR     R9, =3
    ADD     R7, R7, R9
    ADD     R2, BP, #16
    LDR     R1, =0
    ADD     R2, R2, R1, LSL #2
    STR     R7, [R2]        ; Store in b
;Break: Jump to end of Switch statement
    B       L1
;Jump to next set of statements
    B       L5
L4
;Load in the Case primary
    LDR     R5, =1
    MOV     R6, R5
;Load in Switch obj 
    ADD     R2, BP, #16
    LDR     R1, =0
    ADD     R2, R2, R1, LSL #2
    LDR     R5, [R2]        ; Load b
;Comapare Switch obj with Case primary
    CMP     R5, R6
    MOVEQ   R5, #1
    MOVNE   R5, #0
    MOVS    R5, R5          ; reset Z flag in CPSR
    BEQ     L6              ; jump on condition false
L5
    ADD     R2, BP, #16
    LDR     R1, =0
    ADD     R2, R2, R1, LSL #2
    LDR     R7, [R2]        ; Load b
    LDR     R9, =2
    ADD     R7, R7, R9
    ADD     R2, BP, #16
    LDR     R1, =0
    ADD     R2, R2, R1, LSL #2
    STR     R7, [R2]        ; Store in b
;Break: Jump to end of Switch statement
    B       L1
;Jump to next set of statements
    B       L7
L6
;Load in the Case primary
    LDR     R5, =2
    MOV     R6, R5
;Load in Switch obj 
    ADD     R2, BP, #16
    LDR     R1, =0
    ADD     R2, R2, R1, LSL #2
    LDR     R5, [R2]        ; Load b
;Comapare Switch obj with Case primary
    CMP     R5, R6
    MOVEQ   R5, #1
    MOVNE   R5, #0
    MOVS    R5, R5          ; reset Z flag in CPSR
    BEQ     L8              ; jump on condition false
L7
    ADD     R2, BP, #16
    LDR     R1, =0
    ADD     R2, R2, R1, LSL #2
    LDR     R7, [R2]        ; Load b
    LDR     R9, =1
    ADD     R7, R7, R9
    ADD     R2, BP, #16
    LDR     R1, =0
    ADD     R2, R2, R1, LSL #2
    STR     R7, [R2]        ; Store in b
;Break: Jump to end of Switch statement
    B       L1
;Jump to next set of statements
    B       L9
;Default labels whether default present or not
L8
L9
    LDR     R5, =0
    ADD     R2, BP, #16
    LDR     R1, =0
    ADD     R2, R2, R1, LSL #2
    STR     R5, [R2]        ; Store in b
L1
StopTest
    B       StopTest
Main
    LDR     R0, =1          ; current lexic level
    LDR     R1, =1          ; number of local variables
    BL      enter           ; build new stack frame
    B       MainBody

;=======
;Level : 1 - ;local
;=======
;[0]	b		: int
;=======


;=======
;Level : 0 - ;global
;=======
;proc	main
;=======


; Subroutine enter
; Construct stack frame for procedure
; Input: R0 - lexic level (LL)
;		 R1 - number of local variables
; Output: new stack frame

enter
	STR		R0, [TOP,#4]			; set lexic level
	STR		BP, [TOP,#12]			; and dynamic link
	; if called procedure is at the same lexic level as
	; calling procedure then its static link is a copy of
	; the calling procedure's static link, otherwise called
 	; procedure's static link is a copy of the static link 
	; found LL delta levels down the static link chain
    LDR		R2, [BP,#4]				; check if called LL (R0) and
	SUBS	R0, R2					; calling LL (R2) are the same
	BGT		enter1
	LDR		R0, [BP,#8]				; store calling procedure's static
	STR		R0, [TOP,#8]			; link in called procedure's frame
	B		enter2
enter1
	MOV		R3, BP					; load current base pointer
	SUBS	R0, R0, #1				; and step down static link chain
    BEQ     enter2-4                ; until LL delta has been reduced
	LDR		R3, [R3,#8]				; to zero
	B		enter1+4				;
	STR		R3, [TOP,#8]			; store computed static link
enter2
	MOV		BP, TOP					; reset base and top registers to
	ADD		TOP, TOP, #16			; point to new stack frame adding
	ADD		TOP, TOP, R1, LSL #2	; four bytes per local variable
	BX		LR						; return
	
	AREA	Memory, DATA, READWRITE
globals     SPACE 4096
stack      	SPACE 16384

	END