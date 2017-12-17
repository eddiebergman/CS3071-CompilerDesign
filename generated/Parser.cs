/*----------------------------------------------------------------------
Compiler Generator Coco/R,
Copyright (c) 1990, 2004 Hanspeter Moessenboeck, University of Linz
extended by M. Loeberbauer & A. Woess, Univ. of Linz
with improvements by Pat Terry, Rhodes University

This program is free software; you can redistribute it and/or modify it 
under the terms of the GNU General Public License as published by the 
Free Software Foundation; either version 2, or (at your option) any 
later version.

This program is distributed in the hope that it will be useful, but 
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License 
for more details.

You should have received a copy of the GNU General Public License along 
with this program; if not, write to the Free Software Foundation, Inc., 
59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.

As an exception, it is allowed to write an extension of Coco/R that is
used as a plugin in non-free software.

If not otherwise stated, any source code generated by Coco/R (other than 
Coco/R itself) does not fall under the GNU General Public License.
-----------------------------------------------------------------------*/
using System.Collections.Generic;



using System;

namespace Tastier {



public class Parser {
	public const int _EOF = 0;
	public const int _number = 1;
	public const int _ident = 2;
	public const int _string = 3;
	public const int maxT = 49;

	const bool T = true;
	const bool x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

const int // object kinds
      var = 0, proc = 1, scope = 2, constant = 3, array = 4;

  const int // types
      undef = 0, integer = 1, boolean = 2;

  public SymbolTable tab;
  public CodeGenerator gen;

/*-------------------------------------------------------------------------------------------*/



	public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void Ident(out string name) {
		Expect(2);
		name = t.val; 
	}

	void String(out string text) {
		Expect(3);
		text = t.val; 
	}

	void AddOp(out Op op) {
		op = Op.ADD; 
		if (la.kind == 4) {
			Get();
		} else if (la.kind == 5) {
			Get();
			op = Op.SUB; 
		} else SynErr(50);
	}

	void MulOp(out Op op) {
		op = Op.MUL; 
		if (la.kind == 6) {
			Get();
		} else if (la.kind == 7 || la.kind == 8) {
			if (la.kind == 7) {
				Get();
			} else {
				Get();
			}
			op = Op.DIV; 
		} else if (la.kind == 9 || la.kind == 10) {
			if (la.kind == 9) {
				Get();
			} else {
				Get();
			}
			op = Op.MOD; 
		} else SynErr(51);
	}

	void RelOp(out Op op) {
		op = Op.EQU; 
		switch (la.kind) {
		case 11: {
			Get();
			break;
		}
		case 12: {
			Get();
			op = Op.LSS; 
			break;
		}
		case 13: {
			Get();
			op = Op.GTR; 
			break;
		}
		case 14: {
			Get();
			op = Op.NEQ; 
			break;
		}
		case 15: {
			Get();
			op = Op.LEQ; 
			break;
		}
		case 16: {
			Get();
			op = Op.GEQ; 
			break;
		}
		default: SynErr(52); break;
		}
	}

	void Type(out int type) {
		type = undef; 
		if (la.kind == 17) {
			Get();
			type = integer; 
		} else if (la.kind == 18) {
			Get();
			type = boolean; 
		} else if (la.kind == 19) {
			Get();
			String structType; 
			Ident(out structType);
			type = tab.getStructDef(structType).typeId; 
		} else SynErr(53);
	}

	void Primary(out int reg, out int type) {
		Obj obj; string name;
		int indexReg = gen.GetRegister();
		reg = gen.GetRegister(); 
		type = undef;
		
		switch (la.kind) {
		case 2: {
			Ident(out name);
			while (la.kind == 36) {
				StructField(name, out name);
			}
			obj = tab.Find(name);
			type = obj.getTypeId();
			bool arraySyntax = false;
			
			if (la.kind == 33) {
				ArrayIndex(name,out indexReg);
				arraySyntax = true; 
				if(obj.kind != array) SemErr("Can only Index into array");
				if(obj.level == 0){
				 gen.LoadIndexedGlobal(reg, obj.adr, indexReg, name);
				}else{
				 gen.LoadIndexedLocal(reg,tab.curLevel-obj.level, obj.adr, indexReg, name);
				}
				
			}
			if(!arraySyntax){  //var const stuff
			 if (obj.kind != var && obj.kind != constant) SemErr("variable expected, got : " + obj.kind); 
			 if (obj.level == 0){ 
			   gen.LoadGlobal(reg, obj.adr, name);
			 }else{ 
			   gen.LoadLocal(reg, tab.curLevel-obj.level, obj.adr, name);
			 }
			 if (type == boolean) gen.ResetZ(reg);
			}
			
			break;
		}
		case 5: {
			Get();
			Primary(out reg, out type);
			if (type != integer) 
			 SemErr("integer type expected");
			else 
			 gen.NegateValue(reg);
			
			break;
		}
		case 1: {
			Get();
			type = integer; gen.LoadConstant(reg, Convert.ToInt32(t.val)); 
			break;
		}
		case 20: {
			Get();
			type = boolean; gen.LoadTrue(reg);  
			break;
		}
		case 21: {
			Get();
			type = boolean; gen.LoadFalse(reg); 
			break;
		}
		case 22: {
			Get();
			Expr(out reg, out type);
			Expect(23);
			break;
		}
		default: SynErr(54); break;
		}
	}

	void StructField(string pName, out string name) {
		String fieldName; 
		Expect(36);
		Ident(out fieldName);
		name = tab.Find(pName + '.' + fieldName).name; 
	}

	void ArrayIndex(string name ,out int reg) {
		int type;
		int accReg = 3;
		int dCount = 0;
		reg = gen.GetRegister();
		
		Obj arr = tab.Find(name); 
		Expect(33);
		Expr(out reg, out type);
		Expect(34);
		int exprReg = gen.GetRegister();
		int boundReg = gen.GetRegister();
		int compareReg = gen.GetRegister();
		gen.MoveRegister(exprReg, reg);
		if(type == integer){
		 int l1 = gen.NewLabel();
		 int l2 = gen.NewLabel();
		 Console.WriteLine(";Bound check begin");
		 gen.MoveRegister(compareReg, exprReg);
		 gen.LoadConstant(boundReg, 0);
		 gen.RelOp(Op.GEQ, compareReg, boundReg);
		 gen.BranchFalse(l2);
		
		 gen.LoadConstant(boundReg, arr.dimensions[dCount++]);
		 gen.RelOp(Op.LSS, compareReg, boundReg);
		 gen.BranchFalse(l2);
		
		 gen.BranchTrue(l1);
		
		 gen.Label(l2);
		 gen.WriteString("index must be between 0 & ");
		 gen.WriteInteger(boundReg, true);
		 gen.StopProgram(tab.programName);
		 Console.WriteLine(";Bound check end");
		 gen.Label(l1);
		 gen.MoveRegister(accReg, exprReg);
		}
		else SemErr("Array index must be an integer");
		
		while (la.kind == 33) {
			Get();
			Expr(out reg, out type);
			Expect(34);
			gen.MoveRegister(exprReg, reg);
			if(dCount + 1 > arr.dimensions.Length){
			 SemErr("Specifying too many indices, " + arr.name
			   + " has only " + arr.dimensions.Length + " dimenensions" );
			}else if(type == integer){
			 int l1 = gen.NewLabel();
			 int l2 = gen.NewLabel();
			 Console.WriteLine(";Bound check begin");
			 gen.MoveRegister(compareReg, exprReg);
			 gen.LoadConstant(boundReg, 0);
			 gen.RelOp(Op.GEQ, compareReg, boundReg);
			 gen.BranchFalse(l2);
			
			 gen.LoadConstant(boundReg, arr.dimensions[dCount++]);
			 gen.RelOp(Op.LSS, compareReg, boundReg);
			 gen.BranchFalse(l2);
			
			 gen.BranchTrue(l1);
			
			 gen.Label(l2);
			 gen.WriteString("index must be between 0 & ");
			 gen.WriteInteger(boundReg, true);
			 gen.StopProgram(tab.programName);
			
			 Console.WriteLine(";Bound check end");
			 gen.Label(l1);
			 gen.MulOp(Op.MUL, accReg, boundReg);
			 gen.AddOp(Op.ADD, accReg, exprReg);
			}else SemErr("Array index must be an integer");
			
		}
		if(dCount != arr.dimensions.Length)
		 SemErr("Must specifiy all " + arr.dimensions.Length
		   + " indices in array \'" + arr.name + '\'');
		else
		 gen.MoveRegister(reg, accReg);
		
	}

	void Expr(out int reg,        // load value of Expr into register
out int type) {
		int typeR, regR; Op op; 
		SimExpr(out reg, out type);
		if (StartOf(1)) {
			RelOp(out op);
			SimExpr(out regR, out typeR);
			if (type != typeR) SemErr("Type mismatch");
			type = boolean;
			gen.RelOp(op, reg, regR);
			
			if (la.kind == 24) {
				TernaryOp(out reg, out type);
			}
		}
		gen.ClearRegisters(); 
	}

	void Term(out int reg,        // load value of Term into register
out int type) {
		int typeR, regR; Op op; 
		Primary(out reg, out type);
		while (StartOf(2)) {
			MulOp(out op);
			Primary(out regR,out typeR);
			if (type == integer && typeR == integer)
			 gen.MulOp(op, reg, regR);
			else SemErr("integer type expected");
			
		}
	}

	void SimExpr(out int reg, out int type) {
		int typeR, regR; Op op; 
		Term(out reg,out type);
		while (la.kind == 4 || la.kind == 5) {
			AddOp(out op);
			Term(out regR,out typeR);
			if (type == integer && typeR == integer)
			 gen.AddOp(op, reg, regR);
			else SemErr("integer type expected");
			
		}
	}

	void TernaryOp(out int reg,  //Loads value of ? x : y int register based on last RelOp
out int type) {
		int l1, l2; 
		Expect(24);
		gen.ClearRegisters();
		l1 = gen.NewLabel();
		l2 = gen.NewLabel();
		
		gen.BranchFalse(l1);
		Expr(out reg, out type);
		gen.Branch(l2); 
		Expect(25);
		gen.Label(l1); 
		Expr(out reg, out type);
		
		gen.Label(l2); 
	}

	void ConstExpr(out int type, out int value) {
		string name; type = undef; value = -1; 
		if (la.kind == 1) {
			Get();
			type = integer; value = Convert.ToInt32(t.val); 
		} else if (la.kind == 5) {
			Get();
			Expect(1);
			type = integer; value = -(Convert.ToInt32(t.val)); 
		} else if (la.kind == 20) {
			Get();
			type = boolean; value = 1; 
		} else if (la.kind == 21) {
			Get();
			type = boolean; value = 0; 
		} else if (la.kind == 2) {
			Ident(out name);
			Obj obj = tab.Find(name);
			if(obj.kind == constant){
			 type = obj.getTypeId();
			 value = obj.compileTimeValue;
			}
			else SemErr("Must be constant");
			
		} else SynErr(55);
	}

	void VarDecl() {
		string name; int type; int[] dimensions = null;  
		int kind = var;
		
		Type(out type);
		Ident(out name);
		if (la.kind == 33) {
			ArrayDecl(out dimensions);
			if(tab.typeIsStruct(type)) SemErr("Can not make arrays of struct types, namely " + type);  
			kind = array; 
		}
		if(kind == var)
		 tab.NewObj(name, kind, type, tab.topScope.nextAdr);
		else if(kind == array)
		 tab.NewArray(name, type, dimensions, tab.topScope.nextAdr);
		else SemErr("Obj " + name + " not of a correct kind");                                      
		
		while (la.kind == 26) {
			Get();
			Ident(out name);
			if (la.kind == 33) {
				ArrayDecl(out dimensions);
				kind = array; 
			}
			if(kind == var)
			 tab.NewObj(name, kind, type, tab.topScope.nextAdr);
			else if(kind == array)
			 tab.NewArray(name, type, dimensions, tab.topScope.nextAdr);
			else SemErr("Obj " + name + " not of a correct kind");
			
		}
		Expect(27);
	}

	void ArrayDecl(out int[] results) {
		int type; int value = -1;
		List<int> dimensions = new List<int>();
		
		Expect(33);
		ConstExpr(out type, out value);
		Expect(34);
		if(type == integer && value > 0)
		 dimensions.Add(value);
		else
		 SemErr("Array size must be integer > 0");
		
		while (la.kind == 33) {
			Get();
			ConstExpr(out type, out value);
			Expect(34);
			if(type == integer && value > 0)
			 dimensions.Add(value);
			else
			 SemErr("Array size must be integer > 0");
			
		}
		results = dimensions.ToArray(); 
	}

	void StructDecl() {
		string structName, name; int type; 
		Expect(28);
		Expect(19);
		Ident(out structName);
		Expect(29);
		List<Tuple<Type, String>> fields 
		  = new List<Tuple<Type, String>>(); 
		while (la.kind == 17 || la.kind == 18 || la.kind == 19) {
			Type(out type);
			Ident(out name);
			Type t = tab.getType(type);
			fields.Add( new Tuple<Type, String>(t, name));
			
			while (la.kind == 26) {
				Get();
				Ident(out name);
				fields.Add( new Tuple<Type, String>(t, name)); 
			}
			Expect(27);
		}
		tab.newStructDef(structName, fields); 
		Expect(30);
	}

	void ProcDecl() {
		string procName; 
		Expect(31);
		Ident(out procName);
		tab.NewObj(procName, proc, undef, 0);
		if (procName == "main")
		  if (tab.curLevel == 0)
		     tab.mainPresent = true;
		  else SemErr("main not at lexic level 0");
		tab.OpenScope();
		
		Expect(22);
		Expect(23);
		Expect(29);
		if (procName == "main")
		  gen.Label("Main", "Body");
		else {
		  gen.ProcNameComment(procName);
		  gen.Label(procName, "Body");
		}
		
		while (StartOf(3)) {
			Stat();
		}
		Expect(30);
		if (procName == "main") {
		  gen.StopProgram(tab.programName);
		  gen.Enter("Main", tab.curLevel, tab.topScope.nextAdr);
		} else {
		  gen.Return(procName);
		  gen.Enter(procName, tab.curLevel, tab.topScope.nextAdr);
		}
		tab.CloseScope();
		
	}

	void Stat() {
		if (StartOf(4)) {
			ComplexStat();
		} else if (StartOf(5)) {
			SimpleStat();
		} else SynErr(56);
	}

	void ConstDecl() {
		string name; int type, typeR; int reg; int value; 
		Expect(32);
		type = undef; typeR = undef; 
		Type(out type);
		reg = gen.GetRegister(); 
		Ident(out name);
		Obj obj = tab.NewObj(name, constant, type, tab.topScope.nextAdr); 
		Expect(11);
		ConstExpr(out typeR, out value);
		if(type == typeR)
		{
		 obj.compileTimeValue = value;
		 gen.LoadConstant(reg, value);
		 if (obj.level == 0)
		   gen.StoreGlobal(reg, obj.adr, name);
		 else
		   gen.StoreLocal(reg, tab.curLevel-obj.level, obj.adr, name);
		}
		else SemErr("Type Mismatch");
		
		Expect(27);
		gen.ClearRegisters(); 
	}

	void VarAssignment(string varName) {
		int type, reg; 
		Obj obj = tab.Find(varName);
		if(obj.kind != var) SemErr("Must assign to a variable");
		if(tab.typeIsStruct(obj.type.id)) SemErr("Can only assign to primitave fields");
		
		Expect(35);
		Expr(out reg, out type);
		if(type != obj.getTypeId()) SemErr("Must be equal types, " + obj.getTypeId() + ':' + type);
		
		if (obj.level == 0)
		 gen.StoreGlobal(reg, obj.adr, varName);
		else 
		 gen.StoreLocal(reg, tab.curLevel-obj.level, obj.adr, varName);
		
		
		Expect(27);
	}

	void ArrayAssignment(string arrayName,int indexReg) {
		int type; Obj obj; int reg; 
		Expect(35);
		gen.StoreLocal(indexReg, 0 ,tab.topScope.nextAdr++, "push to Stack"); 
		Expr(out reg, out type);
		obj = tab.Find(arrayName);
		if(type != obj.getTypeId()) 
		 SemErr("Type mismatch");
		
		int exprReg = gen.GetRegister();
		int idxReg = gen.GetRegister();
		gen.MoveRegister(exprReg, reg);
		gen.LoadLocal(idxReg, 0, --tab.topScope.nextAdr, "pop off Stack");
		
		if(obj.kind != array) SemErr("Can only Index into array");
		if(obj.level == 0){
		 gen.StoreIndexedGlobal(exprReg, obj.adr, idxReg, arrayName);
		}else{
		 gen.StoreIndexedLocal(exprReg,tab.curLevel-obj.level, obj.adr, idxReg,arrayName);
		}
		
		Expect(27);
	}

	void ProcCall(string name) {
		Expect(22);
		Expect(23);
		Expect(27);
		Obj obj = tab.Find(name);
		if (obj.kind == proc)
		  gen.Call(name);
		else SemErr("object is not a procedure");
		
	}

	void ReadStat() {
		string name; Obj obj; 
		Expect(37);
		Ident(out name);
		Expect(27);
		obj = tab.Find(name);
		if (obj.getTypeId() == integer) {
		  gen.ReadInteger();
		  if (obj.level == 0)
		     gen.StoreGlobal(0, obj.adr, name);
		  else gen.StoreLocal(0, tab.curLevel-obj.level, obj.adr, name);
		}
		else SemErr("integer type expected");
		
	}

	void IfStat() {
		int type, reg; 
		Expect(38);
		int l1, l2; l1 = 0; 
		Expr(out reg, out type);
		if (type == boolean) {
		 l1 = gen.NewLabel();
		 gen.BranchFalse(l1);
		}
		else SemErr("boolean type expected");
		
		Stat();
		l2 = gen.NewLabel();
		gen.Branch(l2);
		gen.Label(l1);
		
		if (la.kind == 39) {
			Get();
			Stat();
		}
		gen.Label(l2); 
	}

	void WhileStat() {
		int type, reg, condLabel, endLabel; 
		Expect(40);
		condLabel = gen.NewLabel();
		endLabel = gen.NewLabel();
		gen.Label(condLabel);
		
		Expr(out reg, out type);
		if (type == boolean) {
		 gen.BranchFalse(endLabel);
		}
		else SemErr("boolean type expected");
		
		Expect(29);
		tab.OpenSubScope(endLabel); 
		while (StartOf(3)) {
			Stat();
		}
		Expect(30);
		gen.Branch(condLabel);
		gen.Label(endLabel);
		tab.CloseScope();
		
	}

	void ScopeStat() {
		int endLabel = gen.NewLabel(); 
		Expect(29);
		tab.OpenSubScope(endLabel); 
		while (StartOf(3)) {
			Stat();
		}
		Expect(30);
		tab.CloseSubScope(); 
	}

	void WriteStat() {
		string text; int type, reg; 
		Expect(41);
		if (StartOf(6)) {
			Expr(out reg, out type);
			switch (type) {
			case integer: gen.WriteInteger(reg, false); break;
			case boolean: gen.WriteBoolean(false); break;
			}
			
		} else if (la.kind == 3) {
			String(out text);
			gen.WriteString(text); 
		} else SynErr(57);
		Expect(27);
	}

	void WriteLnStat() {
		int type, reg; 
		Expect(42);
		Expr(out reg, out type);
		Expect(27);
		switch (type) {
		 case integer: gen.WriteInteger(reg, true); break;
		 case boolean: gen.WriteBoolean(true); break;
		}
		
	}

	void SwitchStat() {
		string name; int reg, oReg, cReg; int type;
		int caseLabel, statementLabel; 
		int endLabel = gen.NewLabel();                                        
		
		Expect(43);
		Ident(out name);
		while (la.kind == 36) {
			StructField(name, out name);
		}
		Obj obj = tab.Find(name); 
		tab.OpenSubScope(endLabel);
		
		Expect(29);
		while (la.kind == 44) {
			caseLabel = gen.NewLabel();
			gen.Label(caseLabel);
			
			Console.WriteLine(";Load in the Case primary");
			
			Get();
			Primary(out reg, out type);
			Expect(25);
			gen.ClearRegisters();
			oReg = gen.GetRegister();
			cReg = gen.GetRegister();
			
			//move generated primary value to cReg
			gen.MoveRegister(cReg, reg); 
			
			//load obj value into oReg
			Console.WriteLine(";Load in Switch obj ");
			if (obj.level == 0)
			 gen.LoadGlobal(oReg, obj.adr, name);
			else
			 gen.LoadLocal(oReg, tab.curLevel-obj.level, obj.adr, name);
			
			
			if(type != obj.getTypeId()) 
			 SemErr("Yo, don't mix n' match the types");
			
			//Compare oReg to cReg
			Console.WriteLine(";Comapare Switch obj with Case primary");
			gen.RelOp(Op.EQU, oReg, cReg);
			gen.BranchFalse(caseLabel + 2); //jump to next case, +2 to skip statementLabel
			//Gen statement labels so optional break can skip the comparisons
			
			statementLabel = gen.NewLabel();
			gen.Label(statementLabel);
			
			//....statements
			
			while (StartOf(3)) {
				Stat();
			}
			Console.WriteLine(";Jump to next set of statements"); 
			gen.Branch(statementLabel + 2); 
		}
		Console.WriteLine(";Default labels whether default present or not");                                      
		caseLabel = gen.NewLabel();
		gen.Label(caseLabel);
		statementLabel = gen.NewLabel();
		gen.Label(statementLabel);
		
		if (la.kind == 45) {
			Get();
			Expect(25);
			while (StartOf(3)) {
				Stat();
			}
		}
		Expect(30);
		gen.Label(endLabel); 
		tab.CloseSubScope();
		
	}

	void ForStat() {
		string name; int reg, type; 
		Expect(46);
		int condLabel = gen.NewLabel();
		int postLabel = gen.NewLabel();
		int statLabel = gen.NewLabel(); 
		int endLabel = gen.NewLabel();
		tab.OpenSubScope(endLabel);
		
		Expect(22);
		if (la.kind == 2) {
			Ident(out name);
			while (la.kind == 36) {
				StructField(name, out name);
			}
			VarAssignment(name);
		}
		gen.Label(condLabel);  
		gen.LoadTrue(gen.GetRegister()); //updates flag, even if CondExpr not there
		
		if (StartOf(6)) {
			Expr(out reg, out type);
			if(type != boolean) SemErr("For condition must be a boolean expression"); 
			Expect(27);
		}
		gen.BranchFalse(endLabel);
		gen.Branch(statLabel);
		
		gen.Label(postLabel); 
		if (StartOf(5)) {
			SimpleStat();
		}
		gen.Branch(condLabel); 
		Expect(23);
		Expect(29);
		gen.Label(statLabel); 
		while (StartOf(3)) {
			Stat();
		}
		Expect(30);
		gen.Label(endLabel); 
		tab.CloseSubScope();
		
	}

	void SimpleStat() {
		string name; int reg; 
		if (la.kind == 2) {
			Ident(out name);
			while (la.kind == 36) {
				StructField(name, out name);
			}
			if (la.kind == 35) {
				VarAssignment(name);
			} else if (la.kind == 33) {
				ArrayIndex(name, out reg);
				ArrayAssignment(name, reg);
			} else if (la.kind == 22) {
				ProcCall(name);
			} else SynErr(58);
		} else if (la.kind == 37) {
			ReadStat();
		} else if (la.kind == 41) {
			WriteStat();
		} else if (la.kind == 42) {
			WriteLnStat();
		} else SynErr(59);
	}

	void BreakStat() {
		Expect(47);
		Expect(27);
		int endLabel = tab.topScope.scopeEndLabel;
		if(endLabel == -1) SemErr("Break was not used in a subscope");
		gen.Branch(endLabel); 
		Console.WriteLine(";Branch to L"+ endLabel); 
	}

	void ComplexStat() {
		switch (la.kind) {
		case 43: {
			SwitchStat();
			break;
		}
		case 38: {
			IfStat();
			break;
		}
		case 40: {
			WhileStat();
			break;
		}
		case 29: {
			ScopeStat();
			break;
		}
		case 46: {
			ForStat();
			break;
		}
		case 47: {
			BreakStat();
			break;
		}
		case 17: case 18: case 19: {
			VarDecl();
			break;
		}
		case 32: {
			ConstDecl();
			break;
		}
		case 28: {
			StructDecl();
			break;
		}
		default: SynErr(60); break;
		}
	}

	void Tastier() {
		string progName; 
		Expect(48);
		Ident(out progName);
		tab.programName = progName; tab.OpenScope(); 
		Expect(29);
		while (StartOf(7)) {
			if (la.kind == 17 || la.kind == 18 || la.kind == 19) {
				VarDecl();
			} else {
				ConstDecl();
			}
		}
		while (la.kind == 31) {
			ProcDecl();
		}
		tab.CloseScope(); 
		Expect(30);
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		Tastier();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,x, x,x,x,x, T,T,x,x, T,x,x,x, x,T,T,x, T,T,T,T, x,x,T,T, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,x, x,x,x,x, T,T,x,x, T,x,x,x, x,x,T,x, T,x,x,T, x,x,T,T, x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,T,x, x,x,x,x, x,x,x},
		{x,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x}

	};
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
    public System.IO.TextWriter errorStream = Console.Error; // error messages go to this stream - was Console.Out DMA
    public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

	public virtual void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "number expected"; break;
			case 2: s = "ident expected"; break;
			case 3: s = "string expected"; break;
			case 4: s = "\"+\" expected"; break;
			case 5: s = "\"-\" expected"; break;
			case 6: s = "\"*\" expected"; break;
			case 7: s = "\"div\" expected"; break;
			case 8: s = "\"DIV\" expected"; break;
			case 9: s = "\"mod\" expected"; break;
			case 10: s = "\"MOD\" expected"; break;
			case 11: s = "\"=\" expected"; break;
			case 12: s = "\"<\" expected"; break;
			case 13: s = "\">\" expected"; break;
			case 14: s = "\"!=\" expected"; break;
			case 15: s = "\"<=\" expected"; break;
			case 16: s = "\">=\" expected"; break;
			case 17: s = "\"int\" expected"; break;
			case 18: s = "\"bool\" expected"; break;
			case 19: s = "\"struct\" expected"; break;
			case 20: s = "\"true\" expected"; break;
			case 21: s = "\"false\" expected"; break;
			case 22: s = "\"(\" expected"; break;
			case 23: s = "\")\" expected"; break;
			case 24: s = "\"?\" expected"; break;
			case 25: s = "\":\" expected"; break;
			case 26: s = "\",\" expected"; break;
			case 27: s = "\";\" expected"; break;
			case 28: s = "\"def\" expected"; break;
			case 29: s = "\"{\" expected"; break;
			case 30: s = "\"}\" expected"; break;
			case 31: s = "\"void\" expected"; break;
			case 32: s = "\"const\" expected"; break;
			case 33: s = "\"[\" expected"; break;
			case 34: s = "\"]\" expected"; break;
			case 35: s = "\":=\" expected"; break;
			case 36: s = "\".\" expected"; break;
			case 37: s = "\"read\" expected"; break;
			case 38: s = "\"if\" expected"; break;
			case 39: s = "\"else\" expected"; break;
			case 40: s = "\"while\" expected"; break;
			case 41: s = "\"write\" expected"; break;
			case 42: s = "\"writeln\" expected"; break;
			case 43: s = "\"switch\" expected"; break;
			case 44: s = "\"case\" expected"; break;
			case 45: s = "\"default\" expected"; break;
			case 46: s = "\"for\" expected"; break;
			case 47: s = "\"break\" expected"; break;
			case 48: s = "\"program\" expected"; break;
			case 49: s = "??? expected"; break;
			case 50: s = "invalid AddOp"; break;
			case 51: s = "invalid MulOp"; break;
			case 52: s = "invalid RelOp"; break;
			case 53: s = "invalid Type"; break;
			case 54: s = "invalid Primary"; break;
			case 55: s = "invalid ConstExpr"; break;
			case 56: s = "invalid Stat"; break;
			case 57: s = "invalid WriteStat"; break;
			case 58: s = "invalid SimpleStat"; break;
			case 59: s = "invalid SimpleStat"; break;
			case 60: s = "invalid ComplexStat"; break;

			default: s = "error " + n; break;
		}
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public virtual void SemErr (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public virtual void SemErr (string s) {
		errorStream.WriteLine(s);
		count++;
	}
	
	public virtual void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public virtual void Warning(string s) {
		errorStream.WriteLine(s);
	}
} // Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}
}