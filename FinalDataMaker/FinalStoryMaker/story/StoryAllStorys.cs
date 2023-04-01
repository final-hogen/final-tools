
using System.Text.Json.Nodes;
using FinalHogen.json;
using FinalHogen.property;

namespace FinalHogen.story{
  /// <summary>
  /// ストーリーデータをストーリ読み機用に変換。完全ではないので後で要調整。
  /// </summary>
  public class StoryAllStorys : FinalJsonNode{
    private int readID = -999;
    private string lastSaveID="0";
    public StoryMessage message;
    public PilotNameDictionary pilotNames;
    public JsonFinalLang lang{get{return Lang;}}
    public StoryAllStorys(Dictionary<string,string> fiileNames)
    :base("StoryAction",fiileNames){
      message = new StoryMessage(fiileNames);
      pilotNames = new PilotNameDictionary(fiileNames);
      this.saveSubFolder = "ストーリー";
    }
    protected override void SetJsonData(JsonNode node){
      JsonNode? id = node.FetchPath("BaseData\u002B\u003CID\u003Ek__BackingField");
      if(id==null)return;
      int nowID = (int)id;
      JsonObject? target = null;
      if(nowID!=(readID+1)){
        target = new JsonObject();
        lastSaveID = MakeDialogName(id.ToString());
        jsonData[lastSaveID] = target;
      }else{
        target = jsonData[lastSaveID].AsObject();
      }
      readID = nowID;
      target.Add(id.ToString(), node.Clone());
    }
    public override JsonNode? Convert(string id)
    {
      JsonObject  result = new JsonObject();
      JsonObject? storyScenes = Get(id) as JsonObject;
      if(storyScenes==null)return null;
      foreach(KeyValuePair<string,JsonNode?> keyval in storyScenes){
        JsonObject scene = new JsonObject();
        scene.AddContent("commands",StroyScene.Convert(this,keyval.Value));
        result.AddContent("dialog"+keyval.Key,scene);
      }
      return result;
    }
    private string MakeDialogName(string id){
      int i = int.Parse(id);
      if(i==1)return "メインストーリー";
      if(i>=100&&i<=999)return "特殊ストーリー"+id;
      if(i>=1000&&i<=9999)return "ヴェロニカの観光案内"+id;
      if(i>=10000&&i<=99999)return "イベント"+id;
      if(i>=100000&&i<=799999)return "イベント"+id;
      if(i==800001)return "国境紛争"+id;
      if(i==970001)return "全面戦争会話"+id;
      if(i==980001)return "全面戦争"+id;
      if(i==980019)return "全面戦争"+id;
      if(i==1000001)return "メインストーリー？"+id;
      if(i>=1300000&&i<1399999)return "勢力戦前哨"+id;
      if(i>=9000000&&i<=9999999)return "イベント"+id;
      if(i>=9000000&&i<=9999999)return "イベント"+id;
      if(i>=10000000&&i<=12099999)return "イブリンストーリー"+id;
      if(i==12100001)return "エイダエスメラルダストーリー"+id;
      if(i==12200001)return "マーグレットストーリー"+id;
      if(i==12300002)return "2233娘ストーリー"+id;
      if(i>=20000000&&i<=21999999)return "マーグレットストーリー"+id;
      if(i==200000441)return "マーグレットストーリー"+id;
      if(i==50000001)return "国境紛争"+id;
      return "不明"+id;
    }
  }
}
