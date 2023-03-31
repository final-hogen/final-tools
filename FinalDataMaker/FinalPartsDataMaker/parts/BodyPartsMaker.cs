
using System.Text.Json.Nodes;
using FinalHogen.json;
using FinalHogen.gear;

namespace FinalHogen.parts
{
  /// <summary>
  /// コックピットパーツデータ
  /// </summary>
  public class BodyPartsmaker : WidgetPartsMaker{
    protected FinalParts BodyData;
    public BodyPartsmaker(CommonPartsData com,Dictionary<string,string> fiileNames)
    :base(com,fiileNames){
      common = com;
      BodyData = new FinalParts("EquipBodyData",fiileNames);
      saveSubFolder += "胸部/";
    }
    public override JsonNode? Convert(string id){
      JsonNode? node = Get(id);
      if(node==null)return null;
      SetIconFile(node);

      JsonObject result = MakeCommonData(node);
      string SuitId = node.FetchPath("SuitID")!.ToString();

      result.AddData("製造情報",MakeProduceInfo(id,SuitId));
      string modelID = node.FetchPath("Model")!.ToString();
      result.AddData("基本性能",ConvertBasicStatus(node,modelID));
      result.AddData("スキル",MakeFeatureSkills(node));

      return result;
    }
    protected JsonObject ConvertBasicStatus(JsonNode node, string bodyId){
      JsonNode? bodyNode = BodyData.Get(bodyId);
      if(bodyNode==null)throw new Exception("arm not found. "+bodyId);
      JsonObject result = new JsonObject();
      result.AddData("貫通耐性",bodyNode.FetchPath("bulletResistance"));
      return result;
    }
    protected override void SetJsonData(JsonNode partsNode){
      SetPartsData(partsNode,EquipType.胸部);
    }
  }
}
