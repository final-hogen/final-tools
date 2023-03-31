
using System.Text.Json.Nodes;
using FinalHogen.json;
using FinalHogen.gear;

namespace FinalHogen.parts
{
  /// <summary>
  /// 生産情報
  /// </summary>
  public class WidgetIntroMaker : FinalParts{
    protected Dictionary<string,JsonNode> WidgetMap = new Dictionary<string, JsonNode>();
    public WidgetIntroMaker(Dictionary<string,string> fiileNames)
    :base("WidgetIntroData",fiileNames){
    }
    public JsonNode? ConvertByWidgetID(string id){
      if(!WidgetMap.ContainsKey(id))return null;
      return WidgetMap[id];
    }
    public JsonNode? Convert(JsonNode? node){
      if(node==null)return null;
      JsonObject result = new JsonObject();
      result.AddData("陣営",node.FetchEnum<WidgetCamp>("WidgetCamp").ToString());
      result.AddData("シリーズ名",node.FetchPath("SeriesName").ToLang(Lang));
      result.AddData("タイプ",node.FetchPath("JuniorGroupName").ToLang(Lang));
      result.AddData("型名",node.FetchPath("SuitName").ToLang(Lang));
      result.AddData("設計者",node.FetchPath("Designer").ToLang(Lang));
      result.AddData("製造者",node.FetchPath("Manufacturer").ToLang(Lang));
      result.AddData("説明",node.FetchPath("SuitDesc").ToLang(Lang));
      return result;
    }
    protected override void SetJsonData(JsonNode node){
      base.SetJsonData(node);
      JsonNode? convert = Convert(node);
      if(convert==null)throw new Exception();

      JsonArray? ids = node.FetchPath("SuitWeaponID") as JsonArray;
      SetMap(ids,convert);
      ids = node.FetchPath("SuitChestID") as JsonArray;
      SetMap(ids,convert);
      ids = node.FetchPath("SuitLegID") as JsonArray;
      SetMap(ids,convert);
      ids = node.FetchPath("SuitBackpackID") as JsonArray;
      SetMap(ids,convert);
    }
    private void SetMap(JsonArray? idArray, JsonNode node){
      if(idArray==null)return;
      for(int i=0;i<idArray.Count;++i){
        string id =  idArray[i]!.ToString();
        WidgetMap[id] = node;
      }
    }
  }
}
