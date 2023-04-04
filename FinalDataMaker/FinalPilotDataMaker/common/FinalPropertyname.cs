
using System.Text.Json.Nodes;
using FinalHogen.json;
using FinalHogen.property;

namespace FinalHogen.pilot
{
  public class FinalPropertyName : JsonDictionary<string>{
    JsonFinalLang JPLang;
    public FinalPropertyName(Dictionary<string,string> fileNames)
    :base("PropertyData",fileNames)
    {
      JPLang = JsonFinalLang.JP.Get(fileNames);
    }
    protected override void SetJsonData(JsonNode node)
    {
      string key = node.FetchPath("BaseData\u002B\u003CID\u003Ek__BackingField")!.ToString();
      string? value = node.FetchPath("UiDesc")!.ToLang(Lang);
      jsonData[key] = GetPropatyName(key,node)!;
    }
    protected string? GetPropatyName(string key, JsonNode node){
        switch(key){
          case "83":return "強固";
          case "84":return "操縦";
          case "85":return "忍耐";
          case "86":return "反応";
          case "87":return "技術";
        }
     return node.FetchPath("UiDesc").ToLang(Lang);
    }
    public JsonObject ConvertKeyValues(JsonNode sourceNode)
    {
      JsonObject result = new JsonObject();
      JsonNode? keys = sourceNode["keys"];
      JsonNode? values = sourceNode["values"];
      if(keys==null || values==null)return result;
      int count = keys.AsArray().Count;
      for( int i = 0; i < count; ++i ){
        JsonNode? key = keys[i];
        JsonNode? value = values[i];
        if(key==null || value==null)return result;
        string? newKey = Get(key);
        if(newKey==null)continue;
        result[newKey] = value;
      }
      return result;
    }
  }
}
