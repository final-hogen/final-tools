
using System.Text.Json.Nodes;
using FinalHogen.gear;
using FinalHogen.json;

namespace FinalHogen.pilot
{
  public class PilotSatusMaker
  {
    public PilotCommonData common;
    LevelupStatus levelup;
    GirlAwakeStaus awake;
    GIrlStarStatus star;
    TalentUndergroundData talent;
    public PilotSatusMaker(PilotCommonData commonData, TalentUndergroundData talentData, Dictionary<string,string> fileNames){
      common = commonData;
      talent = talentData;
      levelup = new LevelupStatus(common,fileNames);
      awake = new GirlAwakeStaus(common,fileNames);
      star = new GIrlStarStatus(common,fileNames);
    }
    public JsonObject Make(string girlId, GirlQualityType rare, ProfessionType classType, int startStar,JsonObject exchange){
      JsonObject lv60 = levelup.ConvertStatus(girlId,60)!.statusObject;
      JsonObject star6 = star.ConvertStatus(girlId,6)!;
      JsonObject? awakeSSR = awake.ConvertStatus(girlId,"SSR",0);
      JsonObject? awakeUR = awake.ConvertStatus(girlId,"UR",0);
      string talentkey = rare.ToString()+"/"+classType.ToString();
      JsonObject mytalent = talent.MakeSumData(talentkey)!;

      JsonObject starStart = star.ConvertStatus(girlId,startStar)!;
      lv60 = operatorStatus(lv60,starStart,"+");
      star6 = operatorStatus(star6,starStart,"-");
      exchange.AddData("操作",exchange.Pop("操縦"));
      if(isZeroStatus(awakeSSR))awakeSSR = null;
      if(isZeroStatus(awakeUR))awakeUR = null;
      if(awakeUR!=null&&awakeSSR!=null)awakeUR = operatorStatus(awakeUR,awakeSSR,"-");

      JsonObject pilotAll = operatorStatus(lv60,star6);
      pilotAll = operatorStatus(pilotAll,mytalent);
      if(awakeSSR!=null)pilotAll = operatorStatus(pilotAll,awakeSSR.AsObject());
      if(awakeUR!=null)pilotAll = operatorStatus(pilotAll,awakeUR.AsObject());
      JsonObject exchanged = operatorStatus(pilotAll,exchange,"*");
      exchanged = ExchengeName(exchanged);
      exchanged = operatorStatus(exchanged,mytalent);

      JsonObject result = new JsonObject();
      result.AddData("LV60",lv60);
      result.AddData("星6",star6);
      if(awakeSSR!=null)result.WritePath("覚醒/SSR",awakeSSR);
      if(awakeUR!=null)result.WritePath("覚醒/UR",awakeUR);
      result.AddData("フル潜在",mytalent);
      result.AddData("変換倍率",exchange);
      result.AddData("能力合計",pilotAll);
      result.AddData("ステ合計",exchanged);

      return result;
    }
    public bool isZeroStatus(JsonObject? check){
      if(check==null)return true;
      IEnumerator<KeyValuePair<String,JsonNode?>> enumrator = check.GetEnumerator();
      if(enumrator==null)return true;
      while(enumrator.MoveNext()){
        JsonNode? value = enumrator.Current.Value;
        if(value==null)continue;
        if(0!=int.Parse(value.ToString()))return false;
      }
      return true;
    }
    public JsonObject operatorStatus(JsonObject first, JsonObject second, string op="+")
    {
      JsonObject result = new JsonObject();
      IEnumerator<KeyValuePair<string,JsonNode?>> enumrator = first.GetEnumerator();
      if(enumrator==null)return first;
      while(enumrator.MoveNext()){
        JsonNode? firstValue = enumrator.Current.Value;
        JsonNode? secondValue = second[enumrator.Current.Key];
        if(firstValue==null||secondValue==null)continue;
        float firstF = (float)firstValue;
        float secondF = (float)secondValue;
        switch(op){
          case "+":firstF+=secondF;break;
          case "-":firstF-=secondF;break;
          case "*":firstF=MathF.Round(firstF*secondF);break;
        }
        result[enumrator.Current.Key] = firstF;
      }
      return result;
    }
    static Dictionary<string,string> exchangeNames = new Dictionary<string, string>(){
      {"強固","耐久"},
      {"操作","攻撃"},
      {"忍耐","防御"},
      {"反応","回避"},
      {"技術","会心"},
    };
    public JsonObject ExchengeName(JsonObject source){
      JsonObject result = new JsonObject();
      foreach(KeyValuePair<string,JsonNode?> keyval in source){
        string key = exchangeNames[keyval.Key];
        result[key] = keyval.Value!.Clone();
      }
      return result;
    }
  }
}
