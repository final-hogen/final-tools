
using System.Text.Json.Nodes;
using FinalHogen.json;
using FinalHogen.gear;

namespace FinalHogen.pilot
{
  class GirlAwakeStaus : FinalJsonNode
  {
    public PilotCommonData common;
    public GirlAwakeStaus(PilotCommonData commonData, Dictionary<string,string> fileNames)
    :base("GirlAwakeData",fileNames){
      common = commonData;
    }
    protected override void SetJsonData(JsonNode node)
    {
      string girlID = node.FetchPath("GirlId")!.ToString();
      JsonObject? target = null;
      if(jsonData.ContainsKey(girlID))target = jsonData[girlID].AsObject();
      else jsonData[girlID] = target = new JsonObject();
      GirlQualityType rare = node.FetchEnum<GirlQualityType>("GirlQualityType");
      string level = node.FetchPath("Level")!.ToString();
      target[rare.ToString()+"/"+level] = node.Clone();
    }
    public JsonObject? ConvertStatus(string girlID,string rare, int level=-1){
      JsonNode? node = Get(girlID,rare,level);
      if(node==null)return null;
      JsonObject statusNode = common.propertyName.ConvertKeyValues(node.FetchPath("AwakeProperty")!);
      return statusNode;
    }
    protected JsonNode? Get(string girlID,string rare, int level=-1){
      JsonObject? node = Get(girlID) as JsonObject;
      if(node==null)return null;
      if(level>=0)return node[rare+"/"+level.ToString()];
      foreach(string rareLevel in node.GetKeys()){
        string[] split = rareLevel.Split("/");
        if(rare!=split[0])continue;
        int check = int.Parse(split[1]);
        if(check <= level)continue;
        level = check;
      }
      return node[rare+"/"+level.ToString()];
    }
  }
}
