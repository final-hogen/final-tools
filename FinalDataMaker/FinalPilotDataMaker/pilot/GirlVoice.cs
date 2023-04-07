
using System.Text.Json.Nodes;
using FinalHogen.json;
using FinalHogen.property;

namespace FinalHogen.pilot
{
  class GirlVoice : JsonDictionary<string>
  {
    public GirlVoice(Dictionary<string,string> fileNames)
    :base("GirlVoice",fileNames){
    }
    protected override void SetJsonData(JsonNode node)
    {
      string id = node.FetchPath("BaseData\u002B\u003CID\u003Ek__BackingField")!.ToString();
      string value = node.FetchPath("Text").ToLang(Lang)!;
      jsonData[id]=value;
    }
    public JsonObject GetSkinVoice(int skinNumber){
      JsonObject result = new JsonObject();
      foreach(KeyValuePair<string,int> keyval in voiceNums){
        int voiceNumber = skinNumber*1000 + keyval.Value;
        string? msg = Get(voiceNumber.ToString());
        if(msg==null)continue;
        result.AddData(keyval.Key,msg);
      }
      return result;
    }
      static  Dictionary<string,int> voiceNums = new Dictionary<string, int>(){
          {"参加",0},
          {"対話1",1},
          {"対話2",2},
          {"対話3",3},
          {"対話4",4},
          {"対話5",5},
          {"対話6",6},
          {"対話7",7},
          {"対話8",8},
          {"対話9",9},
          {"誓約",10},
          {"待機",11},
          {"誕生日",12},
          {"部隊編入",13},
          {"戦闘開始",14},
          {"出撃",15},
          {"操作変更",16},
          {"戦闘不能",17},
          {"スキル準備",18},
          {"スキル1",19},
          {"スキル2",20},
          {"HP低下",21},
          {"勝利",22},
          {"敗北",23},
          {"マップ配置",24},
          {"強化",25},
          {"改造",26},
          {"スキル上昇",27},
          {"アイテム発見",28},
          {"敵発見",29},
          {"移動",30},
          {"拠点占拠",31},
          {"補給獲得",32},
          {"呼び1",39},
          {"呼び2",40},
          {"呼び3",41},
          {"呼び4",42},
          {"呼び5",43},
          {"呼び6",44},
        };
  }
}
