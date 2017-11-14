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
    LDR     R5, =100
 LDR R2, =0
 ADD R2, R4, R2, LSL #2
 STR R5, [R2] ; bufferSize
; Procedure dummy
dummyBody
    LDR     R5, =10
 LDR R2, =1
 ADD R2, R4, R2, LSL #2
 STR R5, [R2] ; i
    LDR     R6, =3
    MOV     R5, R6
;Bound check begin
    MOV     R7, R5
    LDR     R6, =0
    CMP     R7, R6
    MOVGE   R7, #1
    MOVLT   R7, #0
    MOVS    R7, R7          ; reset Z flag in CPSR
    BEQ     L2              ; jump on condition false
    LDR     R6, =10
    CMP     R7, R6
    MOVLT   R7, #1
    MOVGE   R7, #0
    MOVS    R7, R7          ; reset Z flag in CPSR
    BEQ     L2              ; jump on condition false
    BNE     L1              ; jump on condition true
L2
    ADD     R0, PC, #4      ; string address
    BL      TastierPrintString
    B       L3
    DCB     index must be between 0 & , 0
    ALIGN
L3
    MOV     R0, R6
    BL      TastierPrintIntLf
StopTest
    B       StopTest
;Bound check end
L1
    MOV     R3, R5
    LDR     R8, =4
    MOV     R5, R8
;Bound check begin
    MOV     R7, R5
    LDR     R6, =0
    CMP     R7, R6
    MOVGE   R7, #1
    MOVLT   R7, #0
    MOVS    R7, R7          ; reset Z flag in CPSR
    BEQ     L5              ; jump on condition false
    LDR     R6, =10
    CMP     R7, R6
    MOVLT   R7, #1
    MOVGE   R7, #0
    MOVS    R7, R7          ; reset Z flag in CPSR
    BEQ     L5              ; jump on condition false
    BNE     L4              ; jump on condition true
L5
    ADD     R0, PC, #4      ; string address
    BL      TastierPrintString
    B       L6
    DCB     index must be between 0 & , 0
    ALIGN
L6
    MOV     R0, R6
    BL      TastierPrintIntLf
StopTest
    B       StopTest
;Bound check end
L4
    MUL     R3, R6, R3
    ADD     R3, R3, R5
    LDR     R5, =5
    MOV     R5, R5
;Bound check begin
    MOV     R7, R5
    LDR     R6, =0
    CMP     R7, R6
    MOVGE   R7, #1
    MOVLT   R7, #0
    MOVS    R7, R7          ; reset Z flag in CPSR
    BEQ     L8              ; jump on condition false
    LDR     R6, =10
    CMP     R7, R6
    MOVLT   R7, #1
    MOVGE   R7, #0
    MOVS    R7, R7          ; reset Z flag in CPSR
    BEQ     L8              ; jump on condition false
    BNE     L7              ; jump on condition true
L8
    ADD     R0, PC, #4      ; string address
    BL      TastierPrintString
    B       L9
    DCB     index must be between 0 & , 0
    ALIGN
L9
    MOV     R0, R6
    BL      TastierPrintIntLf
StopTest
    B       StopTest
;Bound check end
L7
    MUL     R3, R6, R3
    ADD     R3, R3, R5
    MOV     R5, R3
;Store index on stack as is only safe placewhile Expr is being evaluated
    ADD     R2, BP, #16
    LDR     R1, =1001
    ADD     R2, R2, R1, LSL #2
    STR     R5, [R2]        ; Store in top of Stack
    LDR     R8, =2
    LDR     R10, =3
    ADD     R8, R8, R10
    MOV     R5, R8
;Bound check begin
    MOV     R7, R5
    LDR     R6, =0
    CMP     R7, R6
    MOVGE   R7, #1
    MOVLT   R7, #0
    MOVS    R7, R7          ; reset Z flag in CPSR
    BEQ     L11              ; jump on condition false
    LDR     R6, =10
    CMP     R7, R6
    MOVLT   R7, #1
    MOVGE   R7, #0
    MOVS    R7, R7          ; reset Z flag in CPSR
    BEQ     L11              ; jump on condition false
    BNE     L10              ; jump on condition true
L11
    ADD     R0, PC, #4      ; string address
    BL      TastierPrintString
    B       L12
    DCB     index must be between 0 & , 0
    ALIGN
L12
    MOV     R0, R6
    BL      TastierPrintIntLf
StopTest
    B       StopTest
;Bound check end
L10
    MOV     R3, R5
    LDR     R8, =3
    LDR     R10, =4
    CMP     R8, R10
    MOVLT   R8, #1
    MOVGE   R8, #0
    MOVS    R8, R8          ; reset Z flag in CPSR
    BEQ     L13              ; jump on condition false
    LDR     R5, =4
    B       L14
L13
    LDR     R5, =3
L14
    MOV     R5, R5
;Bound check begin
    MOV     R7, R5
    LDR     R6, =0
    CMP     R7, R6
    MOVGE   R7, #1
    MOVLT   R7, #0
    MOVS    R7, R7          ; reset Z flag in CPSR
    BEQ     L16              ; jump on condition false
    LDR     R6, =10
    CMP     R7, R6
    MOVLT   R7, #1
    MOVGE   R7, #0
    MOVS    R7, R7          ; reset Z flag in CPSR
    BEQ     L16              ; jump on condition false
    BNE     L15              ; jump on condition true
L16
    ADD     R0, PC, #4      ; string address
    BL      TastierPrintString
    B       L17
    DCB     index must be between 0 & , 0
    ALIGN
L17
    MOV     R0, R6
    BL      TastierPrintIntLf
StopTest
    B       StopTest
;Bound check end
L15
    MUL     R3, R6, R3
    ADD     R3, R3, R5
 LDR R2, =1
 ADD R2, R4, R2, LSL #2
 LDR R5, [R2] ; i
    LDR     R7, =7
    SUB     R5, R5, R7
    MOV     R5, R5
;Bound check begin
    MOV     R7, R5
    LDR     R6, =0
    CMP     R7, R6
    MOVGE   R7, #1
    MOVLT   R7, #0
    MOVS    R7, R7          ; reset Z flag in CPSR
    BEQ     L19              ; jump on condition false
    LDR     R6, =10
    CMP     R7, R6
    MOVLT   R7, #1
    MOVGE   R7, #0
    MOVS    R7, R7          ; reset Z flag in CPSR
    BEQ     L19              ; jump on condition false
    BNE     L18              ; jump on condition true
L19
    ADD     R0, PC, #4      ; string address
    BL      TastierPrintString
    B       L20
    DCB     index must be between 0 & , 0
    ALIGN
L20
    MOV     R0, R6
    BL      TastierPrintIntLf
StopTest
    B       StopTest
;Bound check end
L18
    MUL     R3, R6, R3
    ADD     R3, R3, R5
    MOV     R5, R3
    ADD     R2, BP, #16
    LDR     R5, [R2, R5, LSL #2] ; value of arr[]
    MOV     R5, R5
    ADD     R2, BP, #16
    LDR     R1, =1001
    ADD     R2, R2, R1, LSL #2
    LDR     R6, [R2]        ; Load top of Stack
    ADD     R2, BP, #16
    STR     R5, [R2, R6, LSL #2] ; value of arr[]
    MOV     TOP, BP         ; reset top of stack
    LDR     BP, [TOP,#12]   ; and stack base pointers
    LDR     PC, [TOP]       ; return from dummy
dummy
    LDR     R0, =1          ; current lexic level
    LDR     R1, =1001          ; number of local variables
    BL      enter           ; build new stack frame
    B       dummyBody

;=======
;Level : 1 - ;local
;=======
;[0-1000]	arr		: int[10][10][10]
;=======

    LDR     R7, =10
    ADD     R2, BP, #16
    LDR     R1, =0
    ADD     R2, R2, R1, LSL #2
    STR     R7, [R2]        ; Store in size
; Procedure dummy2
dummy2Body
    LDR     R5, =0
 LDR R2, =1
 ADD R2, R4, R2, LSL #2
 STR R5, [R2] ; i
    LDR     R5, =0
    ADD     R2, BP, #16
    LDR     R1, =1
    ADD     R2, R2, R1, LSL #2
    STR     R5, [R2]        ; Store in sum
L21
 LDR R2, =1
 ADD R2, R4, R2, LSL #2
 LDR R5, [R2] ; i
    ADD     R2, BP, #16
    LDR     R1, =0
    ADD     R2, R2, R1, LSL #2
    LDR     R7, [R2]        ; Load size
    CMP     R5, R7
    MOVLT   R5, #1
    MOVGE   R5, #0
    MOVS    R5, R5          ; reset Z flag in CPSR
    BEQ     L22              ; jump on condition false
 LDR R2, =1
 ADD R2, R4, R2, LSL #2
 LDR R6, [R2] ; i
    MOV     R5, R6
;Bound check begin
    MOV     R7, R5
    LDR     R6, =0
    CMP     R7, R6
    MOVGE   R7, #1
    MOVLT   R7, #0
    MOVS    R7, R7          ; reset Z flag in CPSR
    BEQ     L24              ; jump on condition false
    LDR     R6, =10
    CMP     R7, R6
    MOVLT   R7, #1
    MOVGE   R7, #0
    MOVS    R7, R7          ; reset Z flag in CPSR
    BEQ     L24              ; jump on condition false
    BNE     L23              ; jump on condition true
L24
    ADD     R0, PC, #4      ; string address
    BL      TastierPrintString
    B       L25
    DCB     index must be between 0 & , 0
    ALIGN
L25
    MOV     R0, R6
    BL      TastierPrintIntLf
StopTest
    B       StopTest
;Bound check end
L23
    MOV     R3, R5
    MOV     R6, R3
;Store index on stack as is only safe placewhile Expr is being evaluated
    ADD     R2, BP, #16
    LDR     R1, =13
    ADD     R2, R2, R1, LSL #2
    STR     R6, [R2]        ; Store in top of Stack
 LDR R2, =1
 ADD R2, R4, R2, LSL #2
 LDR R8, [R2] ; i
    MOV     R5, R8
    ADD     R2, BP, #16
    LDR     R1, =13
    ADD     R2, R2, R1, LSL #2
    LDR     R6, [R2]        ; Load top of Stack
    ADD     R2, BP, #16
    LDR     R1, =2
    ADD     R2, R2, R1, LSL #2
    STR     R5, [R2, R6, LSL #2] ; value of arr[]
    ADD     R2, BP, #16
    LDR     R1, =1
    ADD     R2, R2, R1, LSL #2
    LDR     R7, [R2]        ; Load sum
 LDR R2, =1
 ADD R2, R4, R2, LSL #2
 LDR R12, [R2] ; i
    MOV     R5, R12
;Bound check begin
    MOV     R7, R5
    LDR     R6, =0
    CMP     R7, R6
    MOVGE   R7, #1
    MOVLT   R7, #0
    MOVS    R7, R7          ; reset Z flag in CPSR
    BEQ     L27              ; jump on condition false
    LDR     R6, =10
    CMP     R7, R6
    MOVLT   R7, #1
    MOVGE   R7, #0
    MOVS    R7, R7          ; reset Z flag in CPSR
    BEQ     L27              ; jump on condition false
    BNE     L26              ; jump on condition true
L27
    ADD     R0, PC, #4      ; string address
    BL      TastierPrintString
    B       L28
    DCB     index must be between 0 & , 0
    ALIGN
L28
    MOV     R0, R6
    BL      TastierPrintIntLf
StopTest
    B       StopTest
;Bound check end
L26
    MOV     R3, R5
    MOV     R12, R3
    ADD     R2, BP, #16
    LDR     R1, =2
    ADD     R2, R2, R1, LSL #2
    LDR     R9, [R2, R12, LSL #2] ; value of arr[]
    ADD     R7, R7, R9
    ADD     R2, BP, #16
    LDR     R1, =1
    ADD     R2, R2, R1, LSL #2
    STR     R7, [R2]        ; Store in sum
    B       L21
L22
    MOV     TOP, BP         ; reset top of stack
    LDR     BP, [TOP,#12]   ; and stack base pointers
    LDR     PC, [TOP]       ; return from dummy2
dummy2
    LDR     R0, =1          ; current lexic level
    LDR     R1, =13          ; number of local variables
    BL      enter           ; build new stack frame
    B       dummy2Body

;=======
;Level : 1 - ;local
;=======
;[0]	size		: const int -> 10
;[1]	sum		: int
;[2-12]	arr		: int[10]
;=======

    LDR     R5, =100
    ADD     R2, BP, #16
    LDR     R1, =0
    ADD     R2, R2, R1, LSL #2
    STR     R5, [R2]        ; Store in constTest
    LDR     R5, =0
    ADD     R2, BP, #16
    LDR     R1, =1
    ADD     R2, R2, R1, LSL #2
    STR     R5, [R2]        ; Store in myBool
MainBody
    ADD     R0, PC, #4      ; store return address
    STR     R0, [TOP]       ; in new stack frame
    B       dummy
    ADD     R2, BP, #16
    LDR     R1, =1
    ADD     R2, R2, R1, LSL #2
    LDR     R5, [R2]        ; Load myBool
    MOVS    R5, R5          ; reset Z flag in CPSR
    ADD     R2, BP, #16
    LDR     R1, =2
    ADD     R2, R2, R1, LSL #2
    STR     R5, [R2]        ; Store in b
StopTest
    B       StopTest
Main
    LDR     R0, =1          ; current lexic level
    LDR     R1, =3          ; number of local variables
    BL      enter           ; build new stack frame
    B       MainBody

;=======
;Level : 1 - ;local
;=======
;[0]	constTest		: const int -> 100
;[1]	myBool		: const bool -> 0
;[2]	b		: bool
;=======


;=======
;Level : 0 - ;global
;=======
;[0]	bufferSize		: const int -> 100
;[1]	i		: int
;[2]	count		: int
;[3]	total		: int
;[4-104]	buffer		: int[100]
;proc	dummy
;proc	dummy2
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