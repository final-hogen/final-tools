
using System.Text.Json.Nodes;
using FinalHogen.json;
using FinalHogen.gear;

namespace FinalHogen.parts
{
  public class LegPartsmaker : WidgetPartsMaker{
    protected LegData LegData;
    public LegPartsmaker(CommonPartsData com,Dictionary<string,string> fiileNames)
    :base(com,fiileNames){
      common = com;
      LegData = com.LegData;
      saveSubFolder += "脚部/";
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

      JsonNode? armorData = common.SuitData.GetArmorData(SuitId);
      string? armorModelID = common.SuitData.GetLegModelID(SuitId);
      if(armorData!=null&&armorModelID!=null){
        result.AddData("専用機性能",ConvertBasicStatus(armorData,armorModelID));
      }

      return result;
    }
    protected JsonObject ConvertBasicStatus(JsonNode node, string legID){
      JsonNode? leg = LegData.Convert(legID);
      if(leg==null)throw new Exception("arm not found. "+legID);
      return leg.AsObject();
    }
    protected override void SetJsonData(JsonNode partsNode){
      SetPartsData(partsNode,EquipType.脚部);
    }
  }
}
