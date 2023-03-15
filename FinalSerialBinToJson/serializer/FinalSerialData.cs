
using System.Runtime.Serialization;
/// <summary>
/// ファイナルギアのシリアライズデータを問答無用で受け取る
/// </summary>
namespace FinalHogen.serialize
{
  /// <summary>
  /// 不明なデータを横取りするバインダ　SetFormatterでセット
  /// </summary>
  class AnonymousBinder : SerializationBinder
  {
    protected FinalTypeBuilder typeBuilder;
    public string firstConvertClassName = "";
    public AnonymousBinder(){
      typeBuilder = FinalTypeBuilder.GetBuilder();
    }
    public void SetFormatter(IFormatter formatter)
    {
      formatter.Binder = this;
      StreamingContext oldContext = formatter.Context;
      StreamingContext newContext = new StreamingContext(oldContext.State,this);
      formatter.Context = newContext;
    }
    public override Type BindToType(string assemblyName, string typeName)
    {
      if( Type.GetType(typeName) == null )
      {
        if(String.IsNullOrEmpty(firstConvertClassName))firstConvertClassName = typeName;
        return MakeAnonymousType(typeName);
      }
      Type? typeToDeserialize = Type.GetType(String.Format("{0}, {1}",
        typeName, assemblyName));
      if(typeToDeserialize==null)
      {
        throw new ArgumentNullException(String.Format("Type.GetType fail.{0}, {1}",
         typeName, assemblyName));
      }
      return typeToDeserialize;
    }
    public Type MakeAnonymousType(string typeName){
      Type gType = typeof(AnonymousSerialData<>);
      Type anonymousType = typeBuilder.MakeType(typeName);
      return  gType.MakeGenericType(anonymousType);
    }
  }
  /// <summary>
  /// よく分からんデータを格納する
  /// </summary>
  [Serializable]
  public class AnonymousSerialData<T> : Dictionary<string,object?>, ISerializable
  {
    private static string classnameKey = "@@ClassName";
    public string? finalClassName {get{return typeof(T).FullName;}}
    public AnonymousSerialData(){}  // JSONSerializer用、使わない
    public AnonymousSerialData(Dictionary<string,object?> src)
    :base(src){}
    public AnonymousSerialData(SerializationInfo info, StreamingContext context)
    {
      SerializationInfoEnumerator iterator = info.GetEnumerator();
      while(iterator.MoveNext()){
        if( iterator.Value!=null&&iterator.Value.GetType().IsArray ){
          this[iterator.Name] = ConvertMultiArray((Array)iterator.Value);
        }else{
          this[iterator.Name] = iterator.Value;
        }
      }
      if(!ContainsKey(classnameKey))this.Add(classnameKey,finalClassName);
    }
    /// <summary>
    /// JsonSerializer では int[,]などの多次元配列は未サポートなので変換
    /// </summary>
    public static Array ConvertMultiArray(Array source)
    {
      if(source.Rank<=1)return source;
      int[] address = new int[source.Rank];
      return (Array)MakeConverArray(source,0,address);
    }
    private static object MakeConverArray(Array source, int rank, int[] addres)
    {
      if(rank>=source.Rank)return source.GetValue(addres)??new object();
      int myRankLength = source.GetLength(rank);
      object[] result = new object[myRankLength];
      for(int i=0;i<myRankLength;++i)
      {
        addres[rank] = i;
        result[i] = MakeConverArray(source,rank+1,addres);
      }
      return result;
    }
  }
}
