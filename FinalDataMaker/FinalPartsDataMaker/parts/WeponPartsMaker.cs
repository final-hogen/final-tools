
using System.Text.Json.Nodes;
using FinalHogen.json;
using FinalHogen.spine;
using FinalHogen.gear;

namespace FinalHogen.parts
{
  public class WeponPartsmaker : WidgetPartsMaker{
    private bool debugPrefabName = false;  // prefab名出力
    protected AttackAnimationReader animReader = new FinalAttackAnimationReader();
    protected ArmData ArmData;
    protected float powerRate = 200.0f;
    public WeponPartsmaker(CommonPartsData com,Dictionary<string,string> fiileNames)
    :base(com,fiileNames){
      common = com;
      ArmData = new ArmData(fiileNames);
      powerRate = (float)common.BattleConfigData.baseNode.FetchPath("0/powerConfig/increasePerHit")!;
      saveSubFolder += "武器/";
    }
    public override JsonNode? Convert(string id){
      JsonNode? node = Get(id);
      if(node==null)return null;
      SetIconFile(node);
      EquipSubType subtype = node.FetchEnum<EquipSubType>("EquipSubType");

      JsonObject result = MakeCommonData(node);
      string SuitId = node.FetchPath("SuitID")!.ToString();
      if(debugPrefabName){
        result.AddData("wepID",id);
        result.AddData("SuitID",SuitId);
      }

      result.AddData("製造情報",MakeProduceInfo(id,SuitId));
      string modelID = node.FetchPath("Model")!.ToString();

      // 汎用機情報
      JsonNode? basicStatus = ConvertBasicStatus(node,modelID);
      string? prefab = ArmData.GetPrefabName(modelID);
      if(prefab!=null){
        JsonObject status =  basicStatus.AsObject();
        status.AddData("攻撃動作",MakeAnimationNode(prefab,subtype,status));
        status.AddAllAttackSpeed();
        if(debugPrefabName)basicStatus.AsObject().AddData("prefab",prefab);  //debug
      }
      // 専用機情報
      JsonNode? armorData = common.SuitData.GetArmorData(SuitId);
      string? armorModelID = common.SuitData.GetWeponModelID(SuitId);
      JsonNode? armorStatus = null;
      if(armorData!=null&&armorModelID!=null){
        armorStatus = ConvertBasicStatus(armorData,armorModelID);
      }

      if(armorStatus!=null){
        string? legID = common.SuitData.GetLegModelID(SuitId);
        prefab = common.LegData.GetPrefabName(legID!);
        if(prefab!=null){
          JsonObject status =  armorStatus.AsObject();
          status.AddData("攻撃動作",MakeAnimationNode(prefab,subtype,status));
          status.AddAllAttackSpeed();
          if(debugPrefabName)armorStatus.AsObject().AddData("prefab",prefab);
        }
      }

      result.AddData("基本性能",basicStatus);
      result.AddData("専用機性能",armorStatus);
      result.AddData("スキル",MakeFeatureSkills(node));

      return CalcResult(id,SuitId,result);
    }
    protected virtual JsonNode? CalcResult(string widgetId, string SuitId, JsonNode? result){
      return result;  // 一部例外を処理するため下位でオーバーライド
    }
    protected JsonNode? MakeAnimationNode(string prefabName,EquipSubType subtype, JsonObject status){
      JsonNode? bulletCount = status["弾薬数"];
      JsonNode? chargeTime = status["チャージ時間"];
      JsonNode? reloadTime = status["リロード時間"];
      AttackAnimationReader.WeditInfomation info = new AttackAnimationReader.WeditInfomation(
        prefabName,
        subtype,
        (bulletCount==null)?(0):((int)bulletCount),
        (chargeTime==null)?(0):((float)chargeTime),
        (reloadTime==null)?(0):((float)reloadTime)
      );
      return animReader.MakeAnimation(info);
    }
    protected JsonObject ConvertBasicStatus(JsonNode node, string armID){
      JsonNode? arm = ArmData.Convert(armID);
      if(arm==null)throw new Exception("arm not found. "+armID);
      
      JsonObject result = new JsonObject();
      result.AddData("武器倍率",node.FetchPath("BulletlAggRatio/0"));
      result.AddData("攻撃速度",node.FetchPath("WeaponSpeed"));
      float power = MathF.Round(powerRate*(float)node.FetchPath("Trigger")!,1);
      result.AddData("エネルギー回復",power);
      int bulletCount = (int)node.FetchPath("Clip")!;
      if(bulletCount<9999&&bulletCount>0){
        result.AddData("弾薬数",bulletCount);
        int reloadTime = (int)node.FetchPath("ReloadTime")!;
        result.AddData("リロード時間",reloadTime/1000.0f);
      }
      int chargeTime = (int)node.FetchPath("AccumulatingTime")!;
      if(chargeTime>0)result.AddData("チャージ時間",chargeTime/1000.0f);
      result.Merge(arm);
      if(debugPrefabName)result.AddData("armID",armID);
      return result;
    }
    protected override void SetJsonData(JsonNode partsNode){
      SetPartsData(partsNode,EquipType.武器);
    }
  }
}
