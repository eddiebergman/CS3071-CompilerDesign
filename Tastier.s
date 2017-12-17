    LDR     R5, =1
 LDR R2, =0
 ADD R2, R4, R2, LSL #2
 STR R5, [R2] ; single
MainBody
;Struct : Foo  size: 2  {
;int v|	Offset : 0 	Size : 1
;bool b|	Offset : 1 	Size : 1
;}

;Struct : Bar  size: 2  {
;Foo lala|	Offset : 0 	Size : 2
;}

    LDR     R6, =9
    ADD     R2, BP, #16
    LDR     R1, =0
    ADD     R2, R2, R1, LSL #2
    STR     R6, [R2]        ; Store in foo1.v
L1
    MOVS    R5, #1          ; true
    ADD     R2, BP, #16
    LDR     R1, =0
    ADD     R2, R2, R1, LSL #2
    LDR     R7, [R2]        ; Load foo1.v
    LDR     R9, =0
    CMP     R7, R9
    MOVGE   R7, #1
    MOVLT   R7, #0
    MOVS    R7, R7          ; reset Z flag in CPSR
    BEQ     L4              ; jump on condition false
    B       L3
L2
    ADD     R2, BP, #16
    LDR     R1, =0
    ADD     R2, R2, R1, LSL #2
    LDR     R6, [R2]        ; Load foo1.v
 LDR R2, =0
 ADD R2, R4, R2, LSL #2
 LDR R8, [R2] ; single
    SUB     R6, R6, R8
    ADD     R2, BP, #16
    LDR     R1, =0
    ADD     R2, R2, R1, LSL #2
    STR     R6, [R2]        ; Store in foo1.v
    B       L1
L3
    ADD     R2, BP, #16
    LDR     R1, =0
    ADD     R2, R2, R1, LSL #2
    LDR     R9, [R2]        ; Load foo1.v
    MOV     R5, R9
;Bound check begin
    MOV     R7, R5
    LDR     R6, =0
    CMP     R7, R6
    MOVGE   R7, #1
    MOVLT   R7, #0
    MOVS    R7, R7          ; reset Z flag in CPSR
    BEQ     L6              ; jump on condition false
    LDR     R6, =10
    CMP     R7, R6
    MOVLT   R7, #1
    MOVGE   R7, #0
    MOVS    R7, R7          ; reset Z flag in CPSR
    BEQ     L6              ; jump on condition false
    BNE     L5              ; jump on condition true
L6
    ADD     R0, PC, #4      ; string address
    BL      TastierPrintString
    B       L7
    DCB     index must be between 0 & , 0
    ALIGN
L7
    MOV     R0, R6
    BL      TastierPrintIntLf
StopTest
    B       StopTest
;Bound check end
L5
    MOV     R3, R5
    MOV     R9, R3
    ADD     R2, BP, #16
    LDR     R1, =8
    ADD     R2, R2, R1, LSL #2
    LDR     R6, [R2, R9, LSL #2] ; value of pineapples[]
    ADD     R2, BP, #16
    LDR     R1, =4
    ADD     R2, R2, R1, LSL #2
    STR     R6, [R2]        ; Store in bar.lala.v
    ADD     R2, BP, #16
    LDR     R1, =4
    ADD     R2, R2, R1, LSL #2
    LDR     R6, [R2]        ; Load bar.lala.v
    LDR     R8, =4
    CMP     R6, R8
    MOVEQ   R6, #1
    MOVNE   R6, #0
    MOVS    R6, R6          ; reset Z flag in CPSR
    BEQ     L8              ; jump on condition false
    B       L4
;Branch to L4
    B       L9
L8
L9
L4
StopTest
    B       StopTest
Main
    LDR     R0, =1          ; current lexic level
    LDR     R1, =18          ; number of local variables
    BL      enter           ; build new stack frame
    B       MainBody

;=======
;Level : 1 - ;local
;=======
;[0]	foo1		: Type: Foo , size: 2
;[0]	foo1.v		: Type: int , size: 1
;[1]	foo1.b		: Type: bool , size: 1
;[2]	foo2		: Type: Foo , size: 2
;[2]	foo2.v		: Type: int , size: 1
;[3]	foo2.b		: Type: bool , size: 1
;[4]	bar		: Type: Bar , size: 2
;[4]	bar.lala		: Type: Foo , size: 2
;[4]	bar.lala.v		: Type: int , size: 1
;[5]	bar.lala.b		: Type: bool , size: 1
;[6]	bar2		: Type: Bar , size: 2
;[6]	bar2.lala		: Type: Foo , size: 2
;[6]	bar2.lala.v		: Type: int , size: 1
;[7]	bar2.lala.b		: Type: bool , size: 1
;[8-18]	pineapples		: Type: int , size: 1[10]
;=======


;=======
;Level : 0 - ;global
;=======
;[0]	single		: const Type: int , size: 1 -> Value : 1
;proc	main
;=======

