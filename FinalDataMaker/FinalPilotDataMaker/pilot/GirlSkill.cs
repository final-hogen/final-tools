
using System.Text.Json.Nodes;
using FinalHogen.json;

namespace FinalHogen.pilot
{
  class SkillDesc : FinalJsonNode{
    public SkillDesc(Dictionary<string,string> fileNames)
    :base("SkillDesc",fileNames){}
  }
  public class GIrlSkill : FinalJsonNode
  {
    SkillDesc skilldesc;
    public GIrlSkill(Dictionary<string,string> fileNames)
    :base("SkillGroup",fileNames){
      skilldesc = new SkillDesc(fileNames);
    }
    protected override void SetJsonData(JsonNode node)
    {
      string skillID = node.FetchPath("ArrayID")!.ToString();
      JsonArray? target = null;
      if(jsonData.ContainsKey(skillID))target = jsonData[skillID].AsArray();
      else jsonData[skillID] = target = new JsonArray();
      target.Add(node.Clone());
    }
    public override JsonNode? Convert(string id)
    {
      JsonArray? nodes = Get(id) as JsonArray;
      if(nodes==null)return null;
      JsonArray skills = new JsonArray();
      foreach(JsonNode? skinNode in nodes){
        JsonNode skill = GetSkillData(skinNode!);
        skills.Add(skill.Clone());
      }
      return Convert(skills);
    }
    protected JsonNode? Convert(JsonArray skills)
    {
      if(skills.Count<=0)throw new Exception();
      
      JsonObject result = new JsonObject();
      JsonNode firstskill = skills[0]!;
      string name = firstskill.FetchPath("TrunkSkillName").ToLang(Lang)!;
      string icon = firstskill.FetchPath("TrunkSkillIcon")!.ToString();
      result.AddData("名前",name);
      result.AddData("アイコン",icon);
      foreach(JsonNode? skill in skills){
        if(skill==null)continue;
        string level = skill.FetchPath("TrunkSkillLevel")!.ToString();
        string desc = skill.FetchPath("TrunkSkillDesc").ToLang(Lang)!;
        JsonObject levelData = new JsonObject();
        levelData.AddData("説明",desc);
        result.AddData(level,levelData);
      }
      return result;
    }
    protected JsonNode GetSkillData(JsonNode node){
      string normalskillid = node.FetchPath("SkillID")!.ToString();
      return skilldesc.Get(normalskillid)!;
    }
    public JsonObject? MakeTalentSkill(int girlId){
      JsonArray skills = new JsonArray();
      for( int skillID=1; skillID<=3; ++skillID ){
        int skillNumber = 5000000 + (girlId*1000) + skillID;
        JsonNode? skill = skilldesc.Get(skillNumber);
        if(skill==null)return MakeTalentSkill(900);  // フィーバーアタック
        skills.Add(skill.Clone());
      }
      return Convert(skills) as JsonObject;
    }
    public JsonObject? MakeEquipSkill(JsonNode? skillID){
      JsonObject? skillData = skilldesc.Get(skillID) as JsonObject;
      if(skillData==null)return null;
      JsonObject result = new JsonObject();
      result.AddData("名前",skillData.FetchPath("TrunkSkillName").ToLang(Lang));
      result.AddData("説明",skillData.FetchPath("TrunkSkillDesc").ToLang(Lang));
      return result;
    }
  }
}
