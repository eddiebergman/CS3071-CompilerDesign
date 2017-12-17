using System;
using System.Collections.Generic;

namespace Tastier {

    public class Obj { // properties of declared symbol
        public string name; // its name
        public int kind; // var, proc or scope
        public Type type; // its type if var (undef for proc)
        public int level; // lexic level: 0 = global; >= 1 local
        public int adr; // address (displacement) in scope 
        public Obj next; // ptr to next object in scope
        //for arrays
        public int[] dimensions; //dimensions of the array
        public int sizeRequired;
        //for constants
        public int compileTimeValue; //value to be stored at compile time
        // for scopes
        public Obj outer; // ptr to enclosing scope
        public Obj locals; // ptr to locally declared objectsBranch
        public int nextAdr; // next free address in scope
        public int scopeEndLabel;
        public List<String> defs;

        public int getTypeId() { return type.id; }
    }


    public class SymbolTable {

        

        //kinds
        const int var = 0, proc = 1, scope = 2, constant = 3, array = 4;

        //types
        private static int nextTypeId = 0;
        public Dictionary<int, Type> types = new Dictionary<int, Type>() 
        {
            { 0, new Type("undef", -1, nextTypeId++) },
            { 1, new Type("int", 1, nextTypeId++) },
            { 2, new Type("bool", 1, nextTypeId++) }
        };

        //struct definitions
        public Dictionary<String, StructDef> structDefs 
            = new Dictionary<String, StructDef>();

        public Obj topScope; // topmost procedure scope
        public int curLevel; // nesting level of current scope
        public Obj undefObj; // object node for erroneous symbols

        public bool mainPresent;
        public string programName;

        public Parser parser;

        public StructDef newStructDef(String name, List<Tuple<Type, String>> fields){
            if(structDefs.ContainsKey(name))
                parser.SemErr("Struct with name :" + name  + " already exists in current context");
            int id = nextTypeId++;
            StructDef sd = new StructDef(name, fields, id);
            structDefs.Add(name, sd);
            types.Add(id, new Type(name, sd.sizeRequired, id));
            topScope.defs.Add(name);
            writeStruct(sd);
            return sd;
        }

        public void writeStruct(StructDef sd){
            Console.WriteLine(";Struct : " + sd.name + "  size: " + sd.sizeRequired + "  {");
            foreach(Field f in sd.fields){
                Console.WriteLine(";" + f.type.name + ' ' + f.name + "|\tOffset : " + f.offset 
                    + " \tSize : " + f.type.sizeRequired);
            }
            Console.WriteLine(";}\n");
        }

        public bool hasStructDef(String name){
            return structDefs.ContainsKey(name);
        }

        public bool typeIsStruct(int typeId){
            return typeId > 2; //what a bad way to do this ...but time
        }

        public Type getType(int typeId){
            Type t;
            if(!types.TryGetValue(typeId,out t)) //will set obj.type if exists
                parser.SemErr("Could not find associated type id :" + typeId);
            return t;
        }

        public StructDef getStructDef(String name){
            StructDef sd;
            if(!structDefs.TryGetValue(name,out sd)) //will set obj.type if exists
                parser.SemErr("Could not find associated struct name :" + name);
            return sd;
        }

        public SymbolTable (Parser parser) {
            curLevel = -1;
            topScope = null;
            undefObj = new Obj ();
            undefObj.name = "undef";
            undefObj.kind = var;
            undefObj.type = null;
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
            topScope.defs = new List<String>();
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
            foreach(String def in topScope.defs){
                structDefs.Remove(def);
            }
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
            return String.Format (";[{0}]\t{1}\t\t: {2}", obj.adr, obj.name, obj.type.toString());
        }

        private string ConstToString (Obj obj) {
            return String.Format (";[{0}]\t{1}\t\t: const {2} -> Value : {3}", 
                obj.adr, obj.name, obj.type.toString(), obj.compileTimeValue);
        }

        private string ArrayToString (Obj obj) {
            String result = String.Format ("{0}\t\t: {1}", obj.name, obj.type.toString());
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

        // open new sub-scope and make it the current scope (topScope)
        public void OpenSubScope (int endLabel) {
            // lexic level remains unchanged
            Obj scop = new Obj ();
            scop.name = "";
            scop.kind = scope;
            scop.outer = topScope;
            scop.locals = null;
            // next available address in stack frame remains unchanged
            scop.nextAdr = topScope.nextAdr;
            scop.scopeEndLabel = endLabel;
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
        public Obj NewObj (string name, int kind, int type, int adr) {
            Obj p, last;
            Obj obj = new Obj ();
            obj.name = name;
            obj.kind = kind;
            if(!types.TryGetValue(type,out obj.type)) //will set obj.type if exists
                parser.SemErr("Could not find associated type id :" + type);
            obj.dimensions = null;
            obj.compileTimeValue = -1;
            obj.scopeEndLabel = -1;
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

            if (kind == var || kind == constant){
                obj.adr = adr;
                if(typeIsStruct(type)){ //What a bad way to tell if its a struct but...timeee
                    StructDef sd = getStructDef(getType(type).name);
                    foreach(Field f in sd.fields){
                        NewObj(name + '.' + f.name, var, f.type.id, obj.adr + f.offset);
                    }
                }else{
                    topScope.nextAdr += obj.type.sizeRequired;
                }
            
            
            }else if(kind == proc){
            }else if(kind == scope){
            }else{
                parser.SemErr("Is not of kind var, constant,scope or proc");                
            }
            return obj;
        }

        public Obj NewArray(string name, int type, int[] dimensions, int adr){
            Obj p, last;
            Obj obj = new Obj ();
            obj.name = name;
            obj.kind = array;
            if(!types.TryGetValue(type,out obj.type)) //will set obj.type if exists
                parser.SemErr("Could not find associated type id :" + type);
            obj.dimensions = null;
            obj.compileTimeValue = -1;
            obj.scopeEndLabel = -1;
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
            obj.dimensions = dimensions;
            obj.sizeRequired = 1;
            foreach(int dimension in dimensions)
                obj.sizeRequired *= dimension * getType(type).sizeRequired;
            obj.adr = adr;
            topScope.nextAdr += obj.sizeRequired;;
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