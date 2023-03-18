
namespace FinalHogen.serialize
{
  /// <summary>
  /// ジェネリックコンテナに入れた　AnonymousSerialData<T>　をキャスト変換できなかったので、
  /// コンテナに含まれる場合は ジェネリックでない AnonymousData を強制使用。
  /// </summary>
  class FinalBinder : AnonymousBinder{
    List<string> abstructNames = new List<string>();
    FinalTypeParser paser = new FinalTypeParser();
    public override Type BindToType(string assemblyName, string typeName)
    {
      setDummyAbstruct(typeName);
      if(isDummyAbstruct(typeName)){
        return typeof(AnonymousData);
      }
      return base.BindToType(assemblyName,typeName);
    }
    public bool isDummyAbstruct(string typeName){
      return abstructNames.Contains(typeName);
    }
    public bool setDummyAbstruct(string typeName){
      if(Type.GetType(typeName)!=null)return false;
      FinalTypeParser.ClassInfo? info = paser.parseType(typeName);
      if(info==null)return false;
      if(!info.isGeneric)return false;
      bool result = false;
      foreach(FinalTypeParser.ClassInfo generic in info.generics){
        if(isDummyAbstruct(generic.className))continue;
        if(Type.GetType(generic.className)!=null)continue;
        abstructNames.Add(generic.className);
        result = true;
      }
      return result;
    }
  }
}
