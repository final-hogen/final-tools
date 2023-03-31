

using System.Text.Json.Nodes;
using FinalHogen.json;

namespace FinalHogen.spine
{
  /// <summary>
  /// 一部特殊なタイプに対する例外処理
  /// </summary>
  public class FinalAttackAnimationReader : AttackAnimationReader
  {
    readonly static string events = "events";
    public override JsonNode? MakeAnimation(WeditInfomation widget){
      JsonNode? result = base.MakeAnimation(widget);
      return MakeExResultNode(widget.assetName,result);
    }
    protected override JsonObject? LoadAnimationNode(string path){
      JsonObject? obj = base.LoadAnimationNode(path);
      return MakeExAnimationNode(obj);
    }
    protected JsonObject? MakeExAnimationNode(JsonObject? obj){
      if(obj==null)return null;
      switch(nowWedget.assetName){
        case "Armor_000970":
        return Makeアーリン専用機Node(obj);
        case "Armor_000280":
        return Makeソラル専用機Node(obj);
        case "Armor_000950":
        return Makeゼロ専用機Node(obj);
        case "Armor_000210":
        return Makeタイシア専用機Node(obj);
      }
      return obj;
    }
    protected JsonNode? MakeExResultNode(string WidgetAssetName,JsonNode? result){
      if(result==null)return null;
      switch(WidgetAssetName){
        case "Arm_001262":
        return Make干し魚竿Result(result);
      }
      return result;
    }
    protected JsonObject Makeアーリン専用機Node(JsonObject source){
      const string gaikotu1 = "["
        +"{\"time\": 0.0333,\"name\": \"fire_1\"}"
        +",{\"time\": 0.0666,\"name\": \"fire_1\"}"
        +",{\"time\": 0.0999,\"name\": \"fire_1\"}"
        +"]";
      const string gaikotu2 = "["
        +"{\"time\": 0.0333,\"name\": \"fire_1\"}"
        +"]";
      JsonNode? gaikotu1Node = JsonNode.Parse(gaikotu1);
      JsonNode? gaikotu2Node = JsonNode.Parse(gaikotu2);
      JsonObject? attackNode = source.FetchPath("attack3") as JsonObject;
      attackNode![events] = gaikotu1Node;
      attackNode = source.FetchPath("attack4") as JsonObject;
      attackNode![events] = gaikotu1Node!.Clone();
      attackNode = source.FetchPath("attack5") as JsonObject;
      attackNode![events] = gaikotu2Node;
      return source;
    }
    protected JsonObject Makeソラル専用機Node(JsonObject source){
      List<JsonNode?> attackNode = new List<JsonNode?>();
      foreach(string key in source.GetKeys()){
        if(!key.StartsWith(attack))continue;
        attackNode.Add(source.Pop(key));
      }
      int index = 1;
      foreach(JsonNode? node in attackNode){
        string? fireName = GetFireEventString(node);
        if(fireName=="fire_2"){
          source.Add(attack+(index++),node);
          source.Add(attack+(index++),node!.Clone());
          source.Add(attack+(index++),node!.Clone());
        }else if(node!=null){
          source.Add(attack+(index++),node);
        }
      }
      return source;
    }
    protected JsonObject Makeゼロ専用機Node(JsonObject source){
      const string attack6_1 = "{\"time\": 0.3,\"name\": \"fire_1\",\"string\": \"fire_1\"}";
      const string attack6_2 = "{\"time\": 0.5,\"name\": \"fire_1\",\"string\": \"fire_1\"}";
      const string attack6_3 = "{\"time\": 0.7,\"name\": \"fire_1\",\"string\": \"fire_1\"}";
      const string attack7 = "{\"time\": 0.3333,\"name\": \"fire_1\",\"string\": \"fire_1\"}";

      JsonArray? attackevent = source.FetchPath("attack6/events") as JsonArray;
      attackevent!.Add(JsonNode.Parse(attack6_1));
      attackevent!.Add(JsonNode.Parse(attack6_2));
      attackevent!.Add(JsonNode.Parse(attack6_3));
      attackevent = source.FetchPath("attack7/events") as JsonArray;
      attackevent!.Add(JsonNode.Parse(attack7));
      return source;
    }
    protected JsonObject Makeタイシア専用機Node(JsonObject source){
      source.Remove("attack6");
      return source;
    }
    protected JsonNode Make干し魚竿Result(JsonNode node){
      JsonArray results = node.AsArray();
      foreach(JsonNode? item in results){
        JsonObject animatioonInfo = item!.AsObject();
        animatioonInfo["全ループ"] = true;
      }
      return node;
    }
    private string? GetFireEventString(JsonNode? node){
      if(node==null)return null;
      JsonArray? events = node.FetchPath("events") as JsonArray;
      if(events==null)return null;
      foreach(JsonNode? time in events){
        JsonObject? timeObj = time as JsonObject;
        if(timeObj==null)continue;
        JsonNode? str=timeObj["string"];
        if(str==null)continue;
        string name = str.ToString();
        if(name.StartsWith(fire))return name;
      }
      return null;
    }
  }
}
