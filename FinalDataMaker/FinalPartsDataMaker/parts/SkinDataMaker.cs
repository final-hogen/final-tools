
using System.Text.Json.Nodes;
using FinalHogen.json;

namespace FinalHogen.parts
{
  /// <summary>
  /// スキン情報
  /// </summary>
  public class SkinDataMaker : FinalParts{
    public SkinDataMaker(Dictionary<string,string> fiileNames)
    :base("GirlSkin",fiileNames){
    }
    protected override void SetJsonData(JsonNode node){
      string girlID = node.FetchPath("GirlId")!.ToString();
      string skinID = node.FetchPath("Skin")!.ToString();
      JsonObject? girlObject = null;
      if(jsonData.ContainsKey(girlID)){
        girlObject = jsonData[girlID].AsObject();
      }else{
        girlObject = new JsonObject();
        jsonData[girlID] = girlObject;
      }
      girlObject[skinID] = node.Clone();
    }
    public string? GetWeponModelID(string girlID,string skinID="1"){
      JsonNode? node = Get(girlID);
      if(node==null)return null;
      node = node.FetchPath(skinID+"/MachineArmorModel1/0");
      if(node==null)return null;
      return node.ToString();
    }
    public string? GetLegModelID(string girlID,string skinID="1"){
      JsonNode? node = Get(girlID);
      if(node==null)return null;
      node = node.FetchPath(skinID+"/MachineArmorModel1/1");
      if(node==null)return null;
      return node.ToString();
    }
  }
}
