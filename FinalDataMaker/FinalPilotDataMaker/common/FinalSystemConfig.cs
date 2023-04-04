
using System.Text.Json.Nodes;
using FinalHogen.json;
using FinalHogen.property;

namespace FinalHogen.pilot
{
  public class FinalSystemConfig : JsonDictionary<string>{
    JsonFinalLang JPLang;
    public FinalSystemConfig(Dictionary<string,string> fileNames)
    :base("SystemConfigData",fileNames)
    {
      JPLang = JsonFinalLang.JP.Get(fileNames);
    }
    protected override void SetJsonData(JsonNode node)
    {
      string key = node.FetchPath("Key")!.ToString();
      string value = node.FetchPath("Value")!.ToString();
      jsonData[key] = value;
    }
  }
}
