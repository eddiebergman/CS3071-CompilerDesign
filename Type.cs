using System;
using System.Collections.Generic;

namespace Tastier {

  public class Type{

    public int id;
    public int sizeRequired;
    public String name;

    public Type(String name, int sizeRequired, int id){
      this.id = id;
      this.name = name;
      this.sizeRequired = sizeRequired;
    }

    public String toString(){
      return "Type: " + name + " , size: " + sizeRequired;
    }

    public static bool operator==(Type t1, Type t2){
      return t1.id == t2.id;
    }

    public static bool operator!=(Type t1, Type t2){
      return !(t1 == t2);
    }

  }

}

/*

        public class StructDefinition{
            public string name;
            //String key: field name, Tuple Value: (int: kind, int: type, int: offset)
            public Dictionary<String, Tuple<int, int>> fields = new Dictionary<String, Tuple<int, int>>();
            public int structSize = 0;
        }
       
        public class StructDefinitionBuilder{
            StructDefinition sd;
            public StructDefinitionBuilder(string name){
                sd = new StructDefinition();
                sd.name = name;
                return this;
            }
            
            public add(String fieldName, int type){
                
            }
            
            public add(String fieldName, int type, int[] dimensions){

            }

            private static getTypeSize(int type){

            }


        }

 */