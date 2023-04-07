
using System.Text.Json.Nodes;
using FinalHogen.json;

namespace FinalHogen.pilot
{
  public class LevelupStatus : FinalJsonNode
  {
    public class status{
      public int UpgradeExp;
      public JsonObject statusObject = new JsonObject();
    }
    public PilotCommonData common;
    public LevelupStatus(PilotCommonData commonData, Dictionary<string,string> fileNames)
    :base("LevelUpData",fileNames){
      common = commonData;
    }
    protected override void SetJsonData(JsonNode node)
    {
      string girlID = node.FetchPath("GirlId")!.ToString();
      JsonObject? target = null;
      if(jsonData.ContainsKey(girlID))target = jsonData[girlID].AsObject();
      else jsonData[girlID] = target = new JsonObject();
      string level = node.FetchPath("Level")!.ToString();
      target[level] = node.Clone();
    }
    public status? ConvertStatus(string girlID, int level){
      JsonNode? node = Get(girlID,level);
      if(node==null)return null;
      int needExp = (int)node.FetchPath("UpgradeExp")!;
      JsonObject statusNode = common.propertyName.ConvertKeyValues(node.FetchPath("GirlProperty")!);
      return new status{UpgradeExp=needExp,statusObject=statusNode};
    }
    protected JsonNode? Get(string girlID, int level){
      JsonObject? node = Get(girlID) as JsonObject;
      if(node==null)return null;
      return node[level.ToString()];
    }
  }
}
