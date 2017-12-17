using System;
using System.Collections.Generic;

namespace Tastier{

  public class StructDef{
    
    //map from fieldname to type and offset
    public List<Field> fields;
    public String name;
    public int sizeRequired;
    public int nextOffset;
    public int typeId;

    public StructDef(String name, List<Tuple<Type, String>> fieldsToAdd, int typeId){
      this.name = name;
      this.fields = new List<Field>();
      this.typeId = typeId;
      sizeRequired = 0;
      foreach(Tuple<Type,String> t in fieldsToAdd){
        this.addField(t.Item1, t.Item2);
      }
    }

    private void addField(Type type, String fieldName){
      fields.Add(new Field(fieldName, nextOffset, type));
      sizeRequired += type.sizeRequired;
      nextOffset += type.sizeRequired;      
    }
  }

  public class Field{

    public int offset;
    public String name;
    public Type type;

    public Field(String name, int offset, Type type){
      this.offset = offset;
      this.name = name;
      this.type = type;
    }
  }

}