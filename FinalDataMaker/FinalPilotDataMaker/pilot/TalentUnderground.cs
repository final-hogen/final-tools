
using System.Text.Json.Nodes;
using FinalHogen.json;

namespace FinalHogen.pilot
{
  public class TalentUndergroundData : FinalJsonNode
  {
    static Dictionary<string,string> checkValues = new Dictionary<string, string>(){
    {"ProfessionType1","守護"},
    {"ProfessionType2","格闘"},
    {"ProfessionType3","炸裂"},
    {"ProfessionType4","狙撃"},
    {"ProfessionType5","投射"},
    {"ProfessionType6","射撃"},
    };
    static List<string> checkRares = new List< string>(){
    "UR",
    //"SSR",  //元データ
    "SR",
    "R",
    "N"
    };
    public FinalFileMapper mapper;
    protected Dictionary<string,float> RareRate = new Dictionary<string, float>();
    public PilotCommonData common;
    public Dictionary<string,JsonObject> Converted = new Dictionary<string, JsonObject>();
    static string RateKey= "品质机师天赋点奖励获取比例";  // SR品质机师天赋点奖励获取比例
    public TalentUndergroundData(PilotCommonData commonData, Dictionary<string,string> fileNames)
    :base("TalentRewardData",fileNames){
      common = commonData;
      saveSubFolder ="パラメータ/潜在/";
      mapper = new FinalFileMapper("潜在");
      foreach(string rare in checkRares){
        RareRate[rare] = GetRarerate(rare);
      }
      SetAllJsonData(baseNode);
    }
    protected override void SetAllJsonData(JsonNode allNode)
    {
      if(common==null)return;
      base.SetAllJsonData(allNode);
    }
    protected override void SetJsonData(JsonNode node)
    {
      const string sourceRare = "SSR";
      JsonNode? LayerNum = node.FetchPath("LayerNum");
      JsonNode? IsCore = node.FetchPath("IsCore");
      JsonNode? Icon = node.FetchPath("Icon1");
      JsonNode? type =node.FetchPath("ProfessionType1/1");
      JsonArray? target = null;
      foreach(KeyValuePair<string,string> keyval in checkValues){
        string keyName = sourceRare+"/"+keyval.Value; // SSR/守護
        if(jsonData.ContainsKey(keyName))target = jsonData[keyName] as JsonArray;
        else jsonData[keyName] = target = new JsonArray();
        JsonObject nodeData = new JsonObject();
        nodeData.AddData("LayerNum",LayerNum);
        nodeData.AddData("IsCore",IsCore);
        nodeData.AddData("Icon",Icon);
        nodeData.AddData("type",type);
        nodeData.AddData("value",node.FetchPath(keyval.Key+"/2"));
        target!.Add(nodeData);
        foreach(string rare in checkRares){
          keyName = rare+"/"+keyval.Value; // N/守護
          JsonObject exchange = MakeRareTalent(rare,nodeData);
          if(jsonData.ContainsKey(keyName))target = jsonData[keyName] as JsonArray;
          else jsonData[keyName] = target = new JsonArray();
          target!.Add(exchange);
        }
      }
    }
    public override JsonNode? Convert(string id)
    {
      if(Converted.ContainsKey(id))return Converted[id];
      JsonArray nodes = Get(id)!.AsArray();
      if(nodes.Count<=0)throw new Exception();
      JsonObject result = new JsonObject();
      foreach(JsonNode? src in nodes){
        if(src==null)continue;
        string LayerNum = src["LayerNum"]!.ToString();
        JsonObject? target = null;
        if(result.ContainsKey(LayerNum))target = result[LayerNum]!.AsObject();
        else result[LayerNum] = target = new JsonObject();
        string number = target.Count.ToString();
        JsonObject data = new JsonObject();
        string typeName = "潜在スキル";
        string typeID = src["type"]!.ToString();
        if(typeID!="1")typeName = common.propertyName.Get(typeID)!;
        string icon = src["Icon"]!.ToString();
        mapper.AddImage(saveSubFolder+typeName+"/アイコン",icon);
        data.AddData("種類",typeName);
        data.AddData("値",ConvertPacent(src));
        target.AddData(number,data);
      }
      Converted[id] = result;
      return result;
    }
    public JsonObject? MakeSumData(string id){
      JsonObject? talentObject = Convert(id) as JsonObject;
      if(talentObject==null)return null;
      JsonObject result = new JsonObject();
      foreach(KeyValuePair<string,JsonNode?> layers in talentObject){
        foreach(KeyValuePair<string,JsonNode?> numbers in layers.Value!.AsObject() ){
          JsonObject numberData =  numbers.Value!.AsObject();
          string Key = numberData.FetchPath("種類")!.ToString();
          float Value = (float)numberData.FetchPath("値")!;
          JsonNode? oldValue = null;
          if(result.TryGetPropertyValue(Key,out oldValue)){
            if(Key=="潜在スキル"){
              Value = Math.Max(Value,(float)oldValue!);
            }else Value = Value+(float)oldValue!;
          }
          result[Key]=Value;
        }
      }
      return result;
    }
    public override void SaveAll()
    {
      base.SaveAll();
      mapper.Save();
    }
    protected JsonNode ConvertPacent(JsonNode src){
      JsonNode valueNode = src["value"]!;
      if(!(bool)src["IsCore"]!)return valueNode;
      if(1==(int)src["type"]!)return valueNode; //潜在スキル
      float value = (float)valueNode;
      return value*100.0f;
    }
    protected JsonObject MakeRareTalent(string rare, JsonObject sourceNode){
      JsonObject result = sourceNode.Clone().AsObject();
      if((bool)result["IsCore"]!)return result;
      float rate = GetRarerate(rare);
      if(rate==1.0f)return result;
      int value = (int)result["value"]!;
      value = (int)(value * rate);  // 切り捨て
      result["value"] = value;
      return result;
    }
    protected float GetRarerate(string rare){
      if(rare=="SSR"||rare=="UR")return 1.0f;
      string? value = common.systemConfig.Get(rare+RateKey);
      if(value==null)return 1.0f;
      float rate;
      if( !float.TryParse(value, out rate) )return 1.0f;
      return rate;
    }
  }
}
