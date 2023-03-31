
using FinalHogen.gear;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace FinalHogen.json
{
  /// <summary>
  /// 色々機能を追加する
  /// おもにこのプロジェクトで使う便利キャスト
  /// </summary>
  public static class JsonFinalExtension{
    public static T ToEnum<T> (this JsonNode? node)where T : Enum{
      return (T)Enum.ToObject(typeof(T),(int)node!);
    }
    public static T FetchEnum<T> (this JsonNode? node, string path)where T : Enum{
      return node!.FetchPath(path+"/value__").ToEnum<T>();
    }
    public static string? ToLang(this JsonNode? node, JsonFinalLang lang){
      if(node==null)return null;
      string str = lang.GetLang(node.ToString());
      return str.FinalSafeString();
    }
    public static string FinalSafeString(this string str){
      str = Regex.Replace( str, "<size.*?>", string.Empty );
      str = Regex.Replace( str, "</size*?>", string.Empty );
      str = Regex.Replace( str, "<color.*?>", string.Empty );
      str = Regex.Replace( str, "</color.*?>", string.Empty );
      str = str.Replace("\"","”").Replace("&","＆").Replace("<","＜").Replace(">","＞");
      return str;
    }
    private static JsonNode? GetDataItem(JsonNode? item){
      JsonValue? value = item as JsonValue;
      if(value!=null){
        string? str;
        if(value.TryGetValue(out str)){
          item = JsonValue.Create(str.FinalSafeString());
        }
      }
      return item;
    }
    public static bool AddData(this JsonObject node, string key, JsonNode? item){
      item = GetDataItem(item);
      return node.AddContent(key,item);
    }
    public static bool WritePathData(this JsonObject node, string key, JsonNode? item){
      item = GetDataItem(item);
      return node.WritePathContent(key,item);
    }
    public static void save(string subdir, string path, JsonNode node){
     if(!subdir.EndsWith("/"))subdir+="/";
      path = GearFiles.OutputJsonDirectry+subdir+path;
      string? dir = Path.GetDirectoryName(path);
      if(dir!=null)Directory.CreateDirectory(dir);
      JsonFile.save(node,path);
      Console.WriteLine("save done. "+path);
    }
  }
}
