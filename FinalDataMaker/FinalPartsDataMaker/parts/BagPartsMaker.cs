
using System.Text.Json.Nodes;
using FinalHogen.gear;
using FinalHogen.json;

namespace FinalHogen.parts
{
  /// <summary>
  /// 背部パーツデータ
  /// </summary>
  public class BagPartsmaker : WidgetPartsMaker{
    public BagPartsmaker(CommonPartsData com,Dictionary<string,string> fiileNames)
    :base(com,fiileNames){
      common = com;
      saveSubFolder += "背部/";
    }
    public override JsonNode? Convert(string id){
      JsonNode? node = Get(id);
      if(node==null)return null;
      SetIconFile(node);

      JsonObject result = MakeCommonData(node);
      string SuitId = node.FetchPath("SuitID")!.ToString();

      result.AddData("製造情報",MakeProduceInfo(id,SuitId));
      string modelID = node.FetchPath("Model")!.ToString();
      result.AddData("スキル",MakeFeatureSkills(node));

      return result;
    }
    protected override void SetJsonData(JsonNode partsNode){
      SetPartsData(partsNode,EquipType.背部);
    }
  }
}
