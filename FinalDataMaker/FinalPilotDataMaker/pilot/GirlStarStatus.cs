
using System.Text.Json.Nodes;
using FinalHogen.json;

namespace FinalHogen.pilot
{
  class GIrlStarStatus : FinalJsonNode
  {
    public PilotCommonData common;
    public GIrlStarStatus(PilotCommonData commonData, Dictionary<string,string> fileNames)
    :base("GirlStarData",fileNames){
      common = commonData;
    }
    protected override void SetJsonData(JsonNode node)
    {
      string girlID = node.FetchPath("GirlId")!.ToString();
      JsonObject? target = null;
      if(jsonData.ContainsKey(girlID))target = jsonData[girlID].AsObject();
      else jsonData[girlID] = target = new JsonObject();
      string level = node.FetchPath("GirlStar")!.ToString();
      target[level] = node.Clone();
    }
    public JsonObject? ConvertStatus(string girlID, int starlevel){
      JsonNode? node = Get(girlID,starlevel);
      if(node==null)return null;
      JsonObject statusNode = common.propertyName.ConvertKeyValues(node.FetchPath("GirlStarProperty")!);
      return statusNode;
    }
    protected JsonNode? Get(string girlID, int starlevel){
      JsonObject? node = Get(girlID) as JsonObject;
      if(node==null)return null;
      return node[starlevel.ToString()];
    }
  }
}
