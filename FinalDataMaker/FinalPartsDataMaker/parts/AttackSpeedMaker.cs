

using System.Text.Json.Nodes;
using FinalHogen.json;

namespace FinalHogen.parts
{
  class AttackSpeedObject{
    float preTime = 0;
    float attackTime = 0;
    float attackCount = 0;
    float afterTime = 0;
    int bulletCount = 0;
    float reloadTime = 0;
    float chargeTime = 0;
    bool isFullLoop = false;
    internal AttackSpeedObject(JsonObject animNode, JsonNode? bulletCountNode,JsonNode? reloadTimeNode,JsonNode? chargeTimeNode){
      JsonArray times = animNode.FetchPath("動き")!.AsArray();
      if(bulletCountNode!=null)bulletCount = (int)bulletCountNode;
      if(reloadTimeNode!=null)reloadTime = (float)reloadTimeNode;
      if(chargeTimeNode!=null)chargeTime = (float)chargeTimeNode;
      isFullLoop = (bool)animNode["全ループ"]!;
      int count = GetAttackCount(times[0]);
      int index = 0;
      int max = times.Count;
      if(count<=0){
        preTime = GetTime(times[index++]);
      }
      if(times.Count>1){
        count = GetAttackCount(times[max-1]);
        if(count<=0){
          afterTime = GetTime(times[max-- -1]);
        }
      }
      for(;index<max;++index){
        attackTime += GetTime(times[index]);
        attackCount += GetAttackCount(times[index]);
      }
      float allTime =preTime+attackTime+afterTime;
      if(allTime<=0||attackCount<=0)throw new Exception();
    }
    public float GetAttackSpeed(){
      if(bulletCount==0)return GetAttackSpeedNoBullet();
      else return GetAttackSpeedWithBullet();
    }
    float GetAttackSpeedNoBullet(){
      if(isFullLoop){
        float pre = Math.Max(chargeTime,preTime);
        return (attackCount/(pre+attackTime+afterTime));
      }
      return (attackCount/(attackTime+chargeTime));
    }
    float GetAttackSpeedWithBullet(){
      float pre = Math.Max(preTime,chargeTime);
      if(isFullLoop){
        return (attackCount/(pre+attackTime+afterTime+(reloadTime/bulletCount)));
      }else{
        float attack = attackTime*bulletCount/attackCount;
        return bulletCount/(pre+attack+reloadTime);
      }
    }
    float GetTime(JsonNode? time){
      if(time==null)return 0;
      time = time["時間長さ"];
      if(time==null)return 0;
      return (float)time;
    }
    int GetAttackCount(JsonNode? time){
      if(time==null)return 0;
      JsonArray? attackes = time["攻撃"] as JsonArray;
      if(attackes==null)return 0;
      return attackes.Count;
    }
  }
  /// <summary>
  ///  データに攻撃速度を計算して追加する
  /// </summary>
  static class AttackSpeedMaker{
    public static void AddAllAttackSpeed(this JsonObject self){
      JsonArray animations = self.FetchPath("攻撃動作")!.AsArray();
      JsonNode? bulletCount = self.FetchPath("弾薬数");
      JsonNode? reloadTime = self.FetchPath("リロード時間");
      JsonNode? chargeTime = self.FetchPath("チャージ時間");
      foreach(JsonNode? node in animations){
        node!.AsObject().AddAttackSpeed(bulletCount,reloadTime,chargeTime);
      }
    }
    public static void AddAttackSpeed(this JsonObject self, JsonNode? bulletCount,JsonNode? reloadTime,JsonNode? chargeTime){
      AttackSpeedObject obj = new AttackSpeedObject(self,bulletCount,reloadTime,chargeTime);
      float speed = obj.GetAttackSpeed();
      self.AddData("攻撃速度",speed);
    }
  }
}
