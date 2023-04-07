
using System.Text.Json.Nodes;
using FinalHogen.json;
using FinalHogen.gear;

namespace FinalHogen.parts
{
  /// <summary>
  /// Widgetの情報を扱う基本クラス
  /// </summary>
  public abstract class WidgetPartsMaker : FinalParts{
    protected CommonPartsData common;
    public WidgetPartsMaker(CommonPartsData com,Dictionary<string,string> fiileNames)
    :base(com.WidgetNode,fiileNames){
      common = com;
      saveSubFolder = "パーツ/";
    }
    public override void SaveAll(){
      Dictionary<string,JsonNode>.KeyCollection keys = jsonData.Keys;
      foreach(string key in keys){
        JsonNode? node = Convert(key);
        if(node==null)continue;
        string name = node.FetchPath("名前")!.ToString();
        Save(name,node);
      }
    }
    protected void SetIconFile(JsonNode node){
      EquipType type = node.FetchEnum<EquipType>("EquipType");
      string icon = node.FetchPath("Icon")!.ToString();
      string? name = node.FetchPath("BaseData+<Name>k__BackingField").ToLang(Lang);
      if(name==null)return;
      common.AddImage("パーツ/"+type.ToString()+"/"+name+"/アイコン",icon);
    }
    protected JsonObject MakeCommonData(JsonNode node){
      JsonObject result = new JsonObject();
      result.AddData("名前",node.FetchPath("BaseData+<Name>k__BackingField").ToLang(Lang));
      result.AddData("説明",node.FetchPath("Disc").ToLang(Lang));
      result.AddData("種類",node.FetchEnum<EquipSubType>("EquipSubType").ToName());
      return result;
    }
    protected JsonNode? MakeProduceInfo(string id, string suitId){
      JsonNode? node = Get(id);
      WidgetCamp camp = node.FetchEnum<WidgetCamp>("WidgetCamp");
      JsonNode? produceInfo = common.WidgetIntroData.ConvertByWidgetID(id);
      if(produceInfo!=null){
        produceInfo["陣営"] = camp.ToString();
        return produceInfo;
      }
      produceInfo = common.SuitData.ConvertProduce(suitId);
      if(produceInfo!=null)produceInfo["陣営"] = camp.ToString();
      return produceInfo;
    }
    protected JsonNode? MakeFeatureSkills(JsonNode node){
      return ConvertSkills(node.FetchPath("FeatureSkill"));
    }
    protected JsonNode? ConvertSkills(JsonNode? node){
      JsonArray? array = node as JsonArray;
      if(array==null)return null;
      if(array.Count<=0)return null;
      JsonArray result = new JsonArray();
      for(int i=0;i<array.Count;++i){
        string id = array[i]!.ToString();
        JsonNode? skill = common.SkillData.Get(id);
        if(skill==null)continue;
        string? desc = skill.FetchPath("TrunkSkillDesc").ToLang(Lang);
        if(desc==null)continue;
        if(desc.StartsWith("チップ："))desc+="（コラボ以外効果なし、戦闘力+100）";
        result.Add(desc);
      }
      return result;
    }
    protected void SetPartsData(JsonNode partsNode, EquipType partsType){
      EquipType type = partsNode.FetchEnum<EquipType>("EquipType");
      if(type!=partsType)return;
      string wepID = partsNode.FetchPath("WidgetIntroID")!.ToString();
      if(wepID=="0")return;
      PropertyType optionType = partsNode.FetchEnum<PropertyType>("PropertyType");
      
      if(jsonData.ContainsKey(wepID)){
        JsonNode checkNode = jsonData[wepID];
         PropertyType checkOption = checkNode.FetchEnum<PropertyType>("PropertyType");
         if(!isOverride(optionType,checkOption))return;
      }
      jsonData[wepID] = partsNode;
    }
    protected bool isOverride(PropertyType myType, PropertyType checkType){
      if(myType==PropertyType.固定オプション)return true;
      if(checkType==PropertyType.固定オプション)return false;
      return myType>checkType;
    }
  }
}
