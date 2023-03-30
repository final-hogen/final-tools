
using System.Text.Json.Nodes;
using FinalHogen.json;

namespace FinalHogen.property
{
  public class PilotNameDictionary : JsonDictionary<string>{
    public PilotNameDictionary(Dictionary<string,string> fiileNames)
    :base("GirlData",fiileNames){
    }
    protected override void SetJsonData(JsonNode node){
      JsonNode? id = node.FetchPath("BaseData+<ID>k__BackingField");
      if(id==null)return;
      string? name = node.FetchPath("BaseData+<Name>k__BackingField").ToLang(Lang);
      if(name==null)return;
      jsonData[id.ToString()] = name;
    }
  }
}
