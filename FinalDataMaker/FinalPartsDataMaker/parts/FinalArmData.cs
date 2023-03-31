
using System.Text.Json.Nodes;
using FinalHogen.json;
using FinalHogen.gear;

namespace FinalHogen.parts
{
  /// <summary>
  /// 武器モデル情報(弾丸付き)
  /// </summary>
  public class ArmData : FinalParts{
    protected FinalParts BulletData;
    public ArmData(Dictionary<string,string> fiileNames)
    :base("EquipArmData",fiileNames){
      BulletData = new FinalParts("BulletData",fiileNames);
    }
    public override JsonNode? Convert(string id){
      return Convert(Get(id));
    }
    public string? GetPrefabName(string id){
      JsonNode? node = Get(id);
      if(node==null)return null;
      node = node.FetchPath("prefab");
      if(node==null)return null;
      
      return node.ToString();
    }
    protected JsonNode? Convert(JsonNode? source){
      if(source==null)return null;

      JsonObject resullt = new JsonObject();

      resullt.AddData("攻撃距離",source.FetchPath("rangeData/rangeX"));
      resullt.AddData("貫通耐性",source.FetchPath("bulletResistance"));
      resullt.AddData("衝撃力",source.FetchPath("components/_items/0/PowerLevel"));
      resullt.AddData("攻撃倍率",makeDameges(source.FetchPath("components/_items/0/powerRatioList")));
      resullt.AddData("攻撃特性",makeBullets(source.FetchPath("components/_items/0/fireData/bulletDatas")));
      
      float randomRotate = (float)source.FetchPath("components/_items/0/RandomRotate")!;
      if(randomRotate>0)resullt.AddData("攻撃角度ブレ",randomRotate);
      return resullt;
    }
    protected JsonArray? makeDameges(JsonNode? node){
      JsonArray? array = node as JsonArray;
      if(array==null)return null;
      if(array.Count<=0)return null;
      float sum = 0;
      foreach(JsonNode? value in array){
        sum += (float)value!;
      }
      if(sum<=0)return null;
      float rate = array.Count / sum;
      JsonArray result = new JsonArray();
      for(int i=0;i<array.Count;++i){
        float pow = MathF.Round(((float)array[i]!)*rate,3);
        result.Add(pow);
      }
      return result;
    }
    protected JsonArray? makeBullets(JsonNode? node){
      JsonArray? array = node as JsonArray;
      if(array==null)return null;
      if(array.Count<=0)return null;
      
      JsonArray result = new JsonArray();
      foreach(JsonNode? value in array){
        JsonObject? bullet = makeBullet(value);
        if(bullet==null)continue;
        result.Add(bullet);
      }
      return result;
    }
    protected JsonObject? makeBullet(JsonNode? node){
      if(node==null)return null;
      JsonNode? id = node.FetchPath("ID");
      JsonNode? bullet = BulletData.Get(id);
      if(bullet==null)return null;
      JsonObject result = new JsonObject();
      BulletType type = bullet.FetchEnum<BulletType>("type");
      result.AddData("種類",type.ToString());
      string range = node.FetchPath("range")!.ToString();
      if(range!="0")result.AddData("飛距離",node.FetchPath("range"));
      float hitTimes = (float)bullet.FetchPath("hitTimes")!;
      if(hitTimes!=0)result.AddData("貫通力",hitTimes);
      float knockback = (float)bullet.FetchPath("beatBackData/BackSpeed/x")!;
      int dumping = (int)bullet.FetchPath("beatBackData/Damping")!;
      if(dumping==0)knockback = 0;  //無敵？
      else knockback *= (100.0f/dumping)*0.5f;  //謎半分
      if(knockback!=0)result.AddData("ノックバック",knockback);
      float multiAttack = (float)node.FetchPath("segmentNumber")!;
      if(multiAttack>1)result.AddData("多段ヒット",multiAttack);

      return result;
    }
  }
}
