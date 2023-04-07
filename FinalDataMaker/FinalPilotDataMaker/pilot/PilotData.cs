
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using FinalHogen.gear;
using FinalHogen.json;

namespace FinalHogen.pilot
{
  class PilotData : FinalJsonNode
  {
    public PilotCommonData common;
    protected PilotSatusMaker statusMaker;
    protected GirlSkin skin;
    protected JsonFinalLang CNLang;
    public PilotData(PilotCommonData commonData, TalentUndergroundData talentData, Dictionary<string,string> fileNames)
    :base("GirlData",fileNames){
      CNLang = JsonFinalLang.CN.Get(fileNames);
      common = commonData;
      statusMaker = new PilotSatusMaker(common,talentData,fileNames);
      skin = new GirlSkin(common,fileNames);
      saveSubFolder = "パイロット/";
    }
    protected override void SetJsonData(JsonNode node)
    {
      int girlID = (int)node.FetchPath("BaseData\u002B\u003CID\u003Ek__BackingField")!;
      if(girlID>5000)return;  // デバッグキャラ？
      string? name = node.FetchPath("BaseData\u002B\u003CName\u003Ek__BackingField").ToLang(Lang);
      if(name==null)return;
      jsonData[name] = node;
    }
    public override JsonNode? Convert(string id)
    {
      JsonNode? node = Get(id);
      if(node==null)return null;
      string girlID = node.FetchPath("BaseData\u002B\u003CID\u003Ek__BackingField")!.ToString();
      string nameNumber = node.FetchPath("BaseData\u002B\u003CName\u003Ek__BackingField")!.ToString();
      string name = Lang.GetLang(nameNumber);
      string CNName = CNLang.GetLang(nameNumber);

      JsonObject result = new JsonObject();
      GirlQualityType rare = node.FetchEnum<GirlQualityType>("GirlQualityType");
      result.AddData("レアリティ",rare.ToString());
      result.AddData("名前",name);
      result.AddData("英名",node.FetchPath("EnglishName"));
      result.AddData("中国名",CNName);

      result.WritePath("伝記/簡略",node.FetchPath("OwnStory").ToLang(Lang));
      result.WritePath("伝記/詳細",node.FetchPath("Autobiography").ToLang(Lang));

      result.WritePath("人柄/好き",node.FetchPath("LikeThing").ToLang(Lang));
      result.WritePath("人柄/嫌い",node.FetchPath("GirlHate").ToLang(Lang));
      result.WritePath("人柄/趣味",node.FetchPath("Hobby").ToLang(Lang));
      result.WritePath("人柄/好物",MakeFavorite(node.FetchPath("FavoriteArticles") as JsonArray));
      
      ProfessionType classType = node.FetchEnum<ProfessionType>("ProfessionType");
      result.WritePath("情報/クラス",classType.ToString());
      int startStar = (int)node.FetchPath("BasicStarLevel")!;
      result.WritePath("情報/初期星",startStar);
      result.WritePath("情報/誕生日",node.FetchPath("Birthday"));
      result.WritePath("情報/血液型",node.FetchPath("BloodType"));
      result.WritePath("情報/身長",node.FetchPath("GirlHeight"));
      result.WritePath("情報/声優",node.FetchPath("CV").ToLang(Lang));

      result.WritePath("所属/勢力",node.FetchEnum<GirlCamp>("GirlCamp").ToString());
      result.WritePath("所属/役職",node.FetchPath("GirlJob").ToLang(Lang));

      result.WritePath("誓約/参列者",MakeGirlNames(node.FetchPath("GirlID") as JsonArray));
      result.WritePath("誓約/効果",ConvertAffection(node.FetchPath("AffectionAttribute")!));

      result.WritePath("精通/武器",common.skill.MakeEquipSkill(node.FetchPath("ExpertSkillWeapeon")));
      result.WritePath("精通/防具",common.skill.MakeEquipSkill(node.FetchPath("ExpertSkillEquip")));
      
      string defaultSkill = node.FetchPath("DefaultSkill")!.ToString();
      result.WritePath("ベースカー/スキル",common.MCVskill.MakeSkill(girlID,defaultSkill));

      JsonObject exchange = common.propertyName.ConvertKeyValues(node.FetchPath("GirlChangePara")!);
      JsonObject status = statusMaker.Make(girlID,rare,classType,startStar,exchange);
      result.WritePath("ステータス",status);
      result.WritePath("スキル",skin.GetSkill(girlID));
      result.WritePath("スキン",skin.Convert(girlID));
      return result;
    }
    private JsonArray? MakeFavorite(JsonArray? list){
      if(list==null)return null;
      JsonArray result = new JsonArray();
      foreach(JsonNode? node in list){
        if(node==null)continue;
        string key = "KindnessGift"+node.ToString();
        result.Add(common.keyTotext.Get(key));
      }
      if(result.Count<=0)return null;
      return result;
    }
    private JsonArray MakeGirlNames(JsonArray? list){
      JsonArray result = new JsonArray();
      if(list==null)return result;
      foreach(JsonNode? node in list){
        if(node==null)continue;
        result.Add(common.pilotName.Get(node));
      }
      return result;
    }
    private JsonArray ConvertAffection(JsonNode node){
      JsonArray result = new JsonArray();
      JsonObject affection = node.AsObject();
      JsonArray? keys = affection.FetchPath("keys") as JsonArray;
      JsonArray? values = affection.FetchPath("values") as JsonArray;
      if(keys==null||values==null)return result;
      if(keys.Count<values.Count)return result;
      for(int i=0;i<keys.Count;++i){
        string key = common.propertyName.Get(keys[i])!;
        float value = 100.0f  * (float)values[i]!;
        JsonObject data = new JsonObject();
        data.AddData("効果",key);
        data.AddData("値",value);
        result.Add(data);
      }
      return result;
    }
    public override void SaveAll()
    {
      base.SaveAll();
      skin.SaveAll();
    }
  }
}
