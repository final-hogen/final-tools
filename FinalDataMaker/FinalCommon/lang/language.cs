
using System.Text.Json.Nodes;
using FinalHogen.json;

namespace FinalHogen
{
  /// <summary>
  /// 言語IDから文字列を取り出すシングルトン
  /// </summary>
  public class JsonFinalLang
  {
    JsonObject langNode;
    public JsonFinalLang(Dictionary<string,string> fileNames, string targetName)
    {
      langNode = JsonFile.load(fileNames[targetName])!.AsObject();
    }
    public string GetLang(string Key)
    {
      int convertNumber = 0;
      if(!int.TryParse(Key,out convertNumber))return Key;
      JsonNode? node = langNode[Key];
      if(node==null)return "";
      return ((string?)node)??"";
    }
    public class JP
    {
       static JsonFinalLang? result = null;
      public static JsonFinalLang Get(Dictionary<string,string> fileNames)
      {
        if(result!=null)return result;
        result = new JsonFinalLang(fileNames,"LangData");
        return result;
      }
    }
    public class CN
    {
       static JsonFinalLang? result = null;
      public static JsonFinalLang Get(Dictionary<string,string> fileNames)
      {
        if(result!=null)return result;
        result = new JsonFinalLang(fileNames,"CNLangData");
        return result;
      }
    }
  }
}