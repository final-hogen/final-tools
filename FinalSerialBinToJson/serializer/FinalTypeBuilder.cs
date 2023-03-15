
using System.Reflection;
using System.Reflection.Emit;

namespace FinalHogen.serialize
{
  /// <summary>
  /// ダミーのタイプを生成するクラス
  /// </summary>
  class FinalTypeBuilder{
    protected static FinalTypeBuilder? singleton = null;
    protected ModuleBuilder moduleBuilder;
    protected Dictionary<string,Type> createdType = new Dictionary<string, Type>();
    protected Dictionary<string,string> extendsInfo = new Dictionary<string, string>();
    protected FinalTypeBuilder(){
      moduleBuilder = MakeModuleBuilder();
    }
    /// <summary>
    /// 継承情報を追加
    /// </summary>
    /// <param name="addInfo">key 継承した子クラス名 value継承する親クラス名</param>
    public void addExtends(Dictionary<string,string> addInfo){
      Dictionary<string,string>.Enumerator itr = addInfo.GetEnumerator();
      while(itr.MoveNext()){
        extendsInfo[itr.Current.Key] = itr.Current.Value;
      }
    }
    public static FinalTypeBuilder GetBuilder(){
      if(singleton!=null)return singleton;
      singleton = new FinalTypeBuilder();
      return singleton;
    }
    /// <summary>
    /// Type を生成
    /// </summary>
    /// <param name="name">ダミークラスの名前</param>
    public Type MakeType(string name)
    {
      string? parentName = null;
      extendsInfo.TryGetValue(name,out parentName);
      if(parentName==null)return MakeType(name,null);
      Type parent = MakeType(parentName);
      return MakeType(parentName,parent);
    }
    protected Type MakeType(string name, Type? parent)
    {
      if(createdType.ContainsKey(name))return createdType[name];
      if(moduleBuilder==null)throw new InvalidProgramException();
      TypeBuilder typebuild = moduleBuilder.DefineType(name,TypeAttributes.Class,parent);
      Type  makedType = typebuild.CreateType();
      createdType[name] = makedType;
      return makedType;
    }
    public static ModuleBuilder MakeModuleBuilder(){
        AssemblyName asmName = new AssemblyName{ Name = "FinalAssem" };
        AssemblyBuilder asmBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
        ModuleBuilder builder = asmBuilder.DefineDynamicModule("FinalModule");
        return builder;
    }
  }
}
