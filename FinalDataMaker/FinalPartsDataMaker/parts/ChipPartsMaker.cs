
using System.Text.Json.Nodes;
using FinalHogen.json;
using FinalHogen.gear;

namespace FinalHogen.parts
{
  /// <summary>
  /// チップパーツデータ
  /// </summary>
  public class ChipPartsmaker : WidgetPartsMaker{
    public ChipPartsmaker(CommonPartsData com,Dictionary<string,string> fiileNames)
    :base(com,fiileNames){
      common = com;
      saveSubFolder += "チップ/";
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
      EquipType type = partsNode.FetchEnum<EquipType>("EquipType");
      if(type!=EquipType.チップ)return;
      string wepID = partsNode.FetchPath("BaseData\u002B\u003CID\u003Ek__BackingField")!.ToString();
      if(wepID=="0")throw new Exception();
      PropertyType optionType = partsNode.FetchEnum<PropertyType>("PropertyType");
      
      if(jsonData.ContainsKey(wepID)){
        JsonNode checkNode = jsonData[wepID];
         PropertyType checkOption = checkNode.FetchEnum<PropertyType>("PropertyType");
         if(!isOverride(optionType,checkOption))return;
      }
      jsonData[wepID] = partsNode;
    }
  }
}
