using System;

namespace Tastier {

    public class Obj { // properties of declared symbol
        public string name; // its name
        public int kind; // var, proc or scope
        public int type; // its type if var (undef for proc)
        public int level; // lexic level: 0 = global; >= 1 local
        public int adr; // address (displacement) in scope 
        public Obj next; // ptr to next object in scope
        //for arrays
        public int[] dimensions; //dimensions of the array
        //for constants
        public int compileTimeValue; //value to be stored at compile time
        // for scopes
        public Obj outer; // ptr to enclosing scope
        public Obj locals; // ptr to locally declared objectsBranch
        public int nextAdr; // next free address in scope
    }

    public class SymbolTable {

        const int // object kinds
        var = 0, proc = 1, scope = 2, constant = 3, array = 4;

        const int // types
        undef = 0, integer = 1, boolean = 2;

        public Obj topScope; // topmost procedure scope
        public int curLevel; // nesting level of current scope
        public Obj undefObj; // object node for erroneous symbols

        public bool mainPresent;
        public string programName;

        Parser parser;

        public SymbolTable (Parser parser) {
            curLevel = -1;
            topScope = null;
            undefObj = new Obj ();
            undefObj.name = "undef";
            undefObj.kind = var;
            undefObj.type = undef;
            undefObj.level = 0;
            undefObj.adr = 0;
            undefObj.next = null;
            this.parser = parser;
            mainPresent = false;
        }

        // open new scope and make it the current scope (topScope)
        public void OpenScope () {
            Obj scop = new Obj ();
            scop.name = "";
            scop.kind = scope;
            scop.outer = topScope;
            scop.locals = null;
            scop.nextAdr = 0;
            topScope = scop;
            curLevel++;
        }

        // close current scope
        public void CloseScope () {
            Console.WriteLine ("\n;=======");
            Console.WriteLine (";Level : {0} - {1}", curLevel, curLevel == 0 ? ";global" : ";local");
            Console.WriteLine (";=======");
            for (Obj cur = topScope.locals; cur != null; cur = cur.next) {
                Console.WriteLine (ObjToString (cur));
            }
            Console.WriteLine (";=======\n");
            topScope = topScope.outer;
            curLevel--;
        }

        public string ObjToString (Obj obj) {
            switch (obj.kind) {
                case var:
                    return VarToString (obj);
                case proc:
                    return ProcToString (obj);
                case scope:
                    return ScopeToString (obj);
                case array:
                    return ArrayToString (obj);
                case constant:
                    return ConstToString (obj);
                default:
                    return "UNKOWN";
            }
        }

        private string VarToString (Obj obj) {
            return String.Format (";[{0}]\t{1}\t\t: {2}", obj.adr, obj.name, TypeToString (obj.type));
        }

        private string ConstToString (Obj obj) {
            return String.Format (";[{0}]\t{1}\t\t: const {2} -> {3}", 
                obj.adr, obj.name, TypeToString (obj.type), obj.compileTimeValue);
        }

        private string ArrayToString (Obj obj) {
            String result = String.Format ("{0}\t\t: {1}", obj.name, TypeToString (obj.type));
            int spaceRequired = 1;
            foreach (int dimension in obj.dimensions) {
                result += "[" + dimension + "]";
                spaceRequired *= dimension;
            }
            return String.Format(";[{0}-{1}]\t{2}",obj.adr, obj.adr+spaceRequired, result);
        }

        private string ProcToString (Obj obj) {
            return String.Format (";proc\t{0}", obj.name);
        }

        private string ScopeToString (Obj obj) {
            return String.Format (";scope\t{0}", obj.name);
        }

        private string TypeToString(int type){
            switch(type){
                case integer: return "int";
                case boolean: return "bool";
                default: return "undef";
            }
        }

        // open new sub-scope and make it the current scope (topScope)
        public void OpenSubScope () {
            // lexic level remains unchanged
            Obj scop = new Obj ();
            scop.name = "";
            scop.kind = scope;
            scop.outer = topScope;
            scop.locals = null;
            // next available address in stack frame remains unchanged
            scop.nextAdr = topScope.nextAdr;
            topScope = scop;
        }

        // close current sub-scope
        public void CloseSubScope () {
            // update next available address in enclosing scope
            topScope.outer.nextAdr = topScope.nextAdr;
            // lexic level remains unchanged
            topScope = topScope.outer;
        }

        // create new object node in current scope
        public Obj NewObj (string name, int kind, int type) {
            Obj p, last;
            Obj obj = new Obj ();
            obj.name = name;
            obj.kind = kind;
            obj.type = type;
            obj.dimensions = null;
            obj.compileTimeValue = -1;
            obj.level = curLevel;
            obj.next = null;
            p = topScope.locals;
            last = null;
            while (p != null) {
                if (p.name == name)
                    parser.SemErr ("name declared twice");
                last = p;
                p = p.next;
            }
            if (last == null)
                topScope.locals = obj;
            else last.next = obj;
            if (kind ==
                var || kind == constant)
                obj.adr = topScope.nextAdr++;
            return obj;
        }

        // search for name in open scopes and return its object node
        public Obj Find (string name) {
            Obj obj, scope;
            scope = topScope;
            while (scope != null) { // for all open scopes
                obj = scope.locals;
                while (obj != null) { // for all objects in this scope
                    if (obj.name == name) return obj;
                    obj = obj.next;
                }
                scope = scope.outer;
            }
            parser.SemErr (name + " is undeclared");
            return undefObj;
        }

    } // end SymbolTable

} // end namespace