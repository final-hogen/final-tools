
using System.Text.Json.Nodes;
using FinalHogen.json;
using FinalHogen.gear;

namespace FinalHogen.pilot
{
  class MCVSkillDesc : FinalJsonNode
  {
    public MCVSkillDesc(Dictionary<string,string> fileNames)
    :base("McvSkillData",fileNames){
    }
    public override JsonNode? Convert(string id)
    {
      JsonNode? node = Get(id);
      if(node==null)return null;
      JsonObject result = new JsonObject();
      result.AddData("名前",node.FetchPath("SkillName").ToLang(Lang));
      result.AddData("説明",node.FetchPath("SkillDesc").ToLang(Lang));
      result.AddData("レベル",node.FetchPath("SkillLevel"));
      //node.FetchPath("SkillIcon");
      return result;
    }
  }
  public class MCVGirlSkill : FinalJsonNode
  {
    MCVSkillDesc skilldesc;
    public MCVGirlSkill(Dictionary<string, string> fileNames)
    : base("McvGirlSkill", fileNames)
    {
      skilldesc =new MCVSkillDesc(fileNames);
    }
    protected override void SetJsonData(JsonNode node)
    {
        string girlID = node.FetchPath("GirlID")!.ToString();
        JsonObject? target = null;
        if(jsonData.ContainsKey(girlID))target = jsonData[girlID].AsObject();
        else jsonData[girlID] = target = new JsonObject();
        string nature = node.FetchPath("NatureID").ToEnum<NatureID>().ToString();
        JsonObject? natureObj = null;
        if(target.ContainsKey(nature))natureObj = target[nature]!.AsObject();
        else target[nature] = natureObj = new JsonObject();
        string natureLevel = node.FetchPath("NatureLevel")!.ToString();
        natureObj[natureLevel] = node.FetchPath("SkillID")!.Clone();
    }
    public override JsonNode? Convert(string id)
    {
      JsonObject result = new JsonObject();
      return Convert(result,id);
    }
    public JsonNode? Convert(JsonObject result, string id)
    {
      JsonObject? node = Get(id) as JsonObject;
      if(node==null)return null;
      foreach(KeyValuePair<string,JsonNode?> nature in node){
        JsonObject natureResult = new JsonObject();
        result[nature.Key] = natureResult;
        foreach(KeyValuePair<string,JsonNode?> level in nature.Value!.AsObject()){
          JsonNode? skill = skilldesc.Convert(level.Value!.ToString());
          string skillLevel = skill!.AsObject().Pop("レベル")!.ToString();
          natureResult[skillLevel] = skill;
        }
      }
      return result;
    }
    public JsonObject MakeSkill(string girlID, string defaultSkillID){
      JsonNode? defaultSkill = skilldesc.Convert(defaultSkillID);
      if(defaultSkill==null)throw new Exception();
      JsonObject result = new JsonObject();
      defaultSkill.AsObject().Remove("レベル");
      result.AddData("固有",defaultSkill);
      Convert(result,girlID);
      return result;
    }
  }
}
