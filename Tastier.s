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

