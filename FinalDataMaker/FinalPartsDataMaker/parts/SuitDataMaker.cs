
using System.Text.Json.Nodes;
using FinalHogen.json;
using FinalHogen.gear;

namespace FinalHogen.parts
{
  /// <summary>
  /// セット(専用機)情報
  /// </summary>
  public class SuitDataMaker : FinalParts{
    protected FinalParts ArmorData;
    protected SkinDataMaker SkinData;
    public SuitDataMaker(Dictionary<string,string> fiileNames)
    :base("SuitData",fiileNames){
      ArmorData = new FinalParts("MachineArmorData",fiileNames);
      SkinData = new SkinDataMaker(fiileNames);
    }
    public JsonNode? ConvertProduce(string id){
      JsonNode? node = Get(id);
      if(node==null)return null;
      JsonObject result = new JsonObject();
      result.AddData("陣営",node.FetchEnum<WidgetCamp>("WidgetCamp").ToString());
      result.AddData("型名",node.FetchPath("SuitName").ToLang(Lang));
      result.AddData("説明",node.FetchPath("Disc").ToLang(Lang));
      result.AddData("設計者",node.FetchPath("Designer").ToLang(Lang));
      result.AddData("製造者",node.FetchPath("Manufacturer").ToLang(Lang));
      return result;
    }
    public JsonNode? GetArmorData(string suitId){
      JsonNode? node = Get(suitId);
      if(node==null)return null;
      string armorID = node.FetchPath("MachineArmorID")!.ToString();
      return ArmorData.Get(armorID);
    }
    public string? GetGirlID(string suitId){
      JsonNode? node = Get(suitId);
      if(node==null)return null;
      node = node.FetchPath("SuitGirlID/0");
      if(node==null)return null;
      return node.ToString();
    }
    public string? GetWeponModelID(string suitId){
      string? girlId = GetGirlID(suitId);
      if(girlId==null)return null;
      return SkinData.GetWeponModelID(girlId);
    }
    public string? GetLegModelID(string suitId){
      string? girlId = GetGirlID(suitId);
      if(girlId==null)return null;
      return SkinData.GetLegModelID(girlId);
    }
  }
}
