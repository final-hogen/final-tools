
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Text.Json.Nodes;
using FinalHogen.json;

namespace FinalHogen.pilot
{
  class GirlSkin : FinalJsonNode
  {
    public PilotCommonData common;
    public GirlVoice voice;
    public FinalFileMapper mapper;
    public FinalFileMapper mapperSkill;
    public GirlSkin(PilotCommonData commonData, Dictionary<string,string> fileNames)
    :base("GirlSkin",fileNames){
      common = commonData;
      voice = new GirlVoice(fileNames);
      mapper = new FinalFileMapper("パイロット");
      mapperSkill = new FinalFileMapper("スキル");
    }
    protected override void SetJsonData(JsonNode node)
    {
      string girlID = node.FetchPath("GirlId")!.ToString();
      JsonObject? target = null;
      if(jsonData.ContainsKey(girlID))target = jsonData[girlID].AsObject();
      else jsonData[girlID] = target = new JsonObject();
      string skinID = node.FetchPath("Skin")!.ToString();
      target[skinID] = node.Clone();
    }
    public override JsonNode? Convert(string girlid)
    {
      JsonObject? nodes = Get(girlid) as JsonObject;
      if(nodes==null)return null;
      string? girlName = common.pilotName.Get(girlid);
      if(girlName==null)throw new Exception();

      JsonObject result = new JsonObject();
      JsonObject? firstVoice = null;
      foreach(KeyValuePair<string,JsonNode?> keyval in nodes){
        if(keyval.Value==null)continue;
        JsonObject skin = new JsonObject();
        string skinName = keyval.Value.FetchPath("SkinName").ToLang(Lang)!;
        skin.AddData("名前",skinName);
        int skinNumber = (int)keyval.Value.FetchPath("BaseData\u002B\u003CID\u003Ek__BackingField")!;
        JsonObject? voiceData = voice.GetSkinVoice(skinNumber);
        if(firstVoice==null){
          firstVoice=voiceData;
        }else if(EqualVoice(firstVoice,voiceData))voiceData=null;
        skin.AddData("ボイス",voiceData);
        
        result.AddData(keyval.Key,skin);

        string faceIcon = keyval.Value.FetchPath("StageHeadIcon")!.ToString();
        string normalIcon = keyval.Value.FetchPath("HeadIcon")!.ToString();
        string longIcon = keyval.Value.FetchPath("HeadIcon_square")!.ToString();
        string skinPath = "パイロット/"+girlName+"/スキン/"+skinName;
        mapper.AddImage(skinPath+"/顔",faceIcon);
        mapper.AddImage(skinPath+"/アイコン",normalIcon);
        mapper.AddImage(skinPath+"/ロング",longIcon);
      }
      return result;
    }
    private bool EqualVoice(JsonObject first, JsonObject second){
      foreach(KeyValuePair<string,JsonNode?> keyval in first){
        string key = keyval.Key;
        if(!second.ContainsKey(key))return false;
        if(first[key]!.ToString()!=second[key]!.ToString())return false;
      }
      return true;
    }
    public JsonObject? GetSkill(string girlID){
      JsonObject? node = Get(girlID) as JsonObject;
      if(node==null)return null;
      string? girlName = common.pilotName.Get(girlID);
      if(girlName==null)throw new Exception();
      JsonObject? result = ConvertSkill(node["1"]!);
      if(result==null)return null;
      foreach(KeyValuePair<string,JsonNode?> keyval in result){
        JsonNode? icon = keyval.Value!.AsObject().Pop("アイコン");
        string name = keyval.Value["名前"]!.ToString();
        string path = "パイロット/"+girlName+"/スキル/"+name+"/アイコン";
        mapperSkill.AddImage(path,icon!.ToString());
      }
      result.AddData("PS",common.skill.MakeTalentSkill(int.Parse(girlID)));
      return result;
    }
    protected JsonObject? ConvertSkill(JsonNode skinNode){
      JsonArray? skillArray = skinNode.FetchPath("SkillArray") as JsonArray;
      if(skillArray==null)return null;
      JsonObject result = new JsonObject();
      string[] skillTypes = {"AS","P1","P2","P3"};
      for(int i=0; i<skillArray.Count;++i){
        JsonArray? skillInfo  = skillArray[i] as JsonArray;
        if(skillInfo==null)continue;
        string skillNumber = (skillInfo[0]!).ToString();
        JsonNode? skillNode = common.skill.Convert(skillNumber);
        if(skillNode==null)throw new Exception();
        result.Add(skillTypes[i],skillNode);
      }
      skillArray = skinNode.FetchPath("SkillArrayUR") as JsonArray;
      if(skillArray==null)return result;
      if(skillArray.Count<=0)return result;
      string urNumber = (skillArray[0]![0]!).ToString();
      JsonNode? urNode = common.skill.Convert(urNumber);
      if(urNode==null)throw new Exception();
      result.Add("UR",urNode);
      return result;
    }
    public override void SaveAll()
    {
      mapper.Save();
      mapperSkill.Save();
    }
  }
}
