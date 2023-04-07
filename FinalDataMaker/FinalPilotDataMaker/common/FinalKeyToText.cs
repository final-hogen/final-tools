
using System.Text.Json.Nodes;
using FinalHogen.json;
using FinalHogen.property;

namespace FinalHogen.pilot
{
  public class FinalKeyToText : JsonDictionary<string>{
    JsonFinalLang JPLang;
    public FinalKeyToText(Dictionary<string,string> fileNames)
    :base("KeyToText",fileNames)
    {
      JPLang = JsonFinalLang.JP.Get(fileNames);
    }
    protected override void SetJsonData(JsonNode node)
    {
      string key = node.FetchPath("BaseData\u002B\u003CName\u003Ek__BackingField")!.ToString();
      string? value = node.FetchPath("ChineseShow")!.ToLang(Lang);
      jsonData[key] = value!;
    }

  }
}
