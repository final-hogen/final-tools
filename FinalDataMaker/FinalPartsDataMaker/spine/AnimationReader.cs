

using System.Diagnostics;
using System.Text.Json.Nodes;
using FinalHogen.json;
using FinalHogen.gear;

namespace FinalHogen.spine
{
  /// <summary>
  /// skelをjsonにコンバート
  /// </summary>
  public class SkelConvert{
    protected string readFolder = "TextAsset/";
    protected string writeBaseFolder = "出力spine/";
    protected string exeFile = "mytools/FinalSpineBinToJson.exe";
    protected string readExt = ".skel.asset";
    protected string writeExt = ".json";
    public string[] Convert(string outputSubFolder, string WidgetAssetName, string? outputName=null){
      if(String.IsNullOrEmpty(WidgetAssetName))return new string[]{};
      if(String.IsNullOrEmpty(outputName))outputName = WidgetAssetName;

      string[] results = ConvertSkel(SerachFolder(WidgetAssetName),WidgetAssetName,outputSubFolder,outputName);
      return results;
    }
    protected string[] SerachFolder(string WidgetAssetName){
      return Directory.GetFiles(readFolder,WidgetAssetName+"*"+readExt);
    }
    protected string[] ConvertSkel(string[] WidgetFiles, string WidgetAssetName,string outputSubFolder, string outputName){
      string outputFoler = writeBaseFolder+outputSubFolder;
      if((outputFoler[outputFoler.Length-1])!='/')outputFoler += "/";
      if(!File.Exists(outputFoler)){
        Directory.CreateDirectory(outputFoler);
      }

      List<string> resultFiled = new List<string>();
      foreach(string WidgetFile in WidgetFiles){
        string readPath = WidgetFile;
        string writePath = WidgetFile
          .Replace(readFolder,outputFoler)
          .Replace(WidgetAssetName,outputName)
          .Replace(readExt,writeExt);
        if( ConvertSkel(readPath,writePath) ){
          resultFiled.Add(writePath);
        }
      }
      return resultFiled.ToArray();
    }
    protected bool ConvertSkel(string WidgetAssetPath, string outputPath){
      if(File.Exists(outputPath))return true; //作成済み

      ProcessStartInfo info = new ProcessStartInfo(exeFile){
          ArgumentList = {
             WidgetAssetPath,
             outputPath}
      };
      Process? process = Process.Start(info);
      if(process==null)return false;
      process.WaitForExit();
      Console.WriteLine("convert success."+outputPath);
      return true;
    }
  }
  /// <summary>
  /// json変換した攻撃アニメーション情報を読み取る
  /// </summary>
  public class AttackAnimationReader{
    public struct WeditInfomation{
      public WeditInfomation(string name, EquipSubType type, int bullet, float charge, float reload){
        assetName = name;
        subtype = type;
        bulletCount = bullet;
        chargeTime = charge;
        reloadTime = reload;
      }
      public string assetName;
      public EquipSubType subtype;
      public int bulletCount;
      public float chargeTime;
      public float reloadTime;
    }
    public const char chargeNum = '2';
    public const char reloadNum = '8';
    public const string timeName = "時間長さ";
    public const string attackName = "攻撃";
    protected const string timeStr = "time";
    protected const string fire = "fire";
    protected const string attack = "attack";
    protected SkelConvert converter = new SkelConvert();
    protected WeditInfomation nowWedget;
    public AttackAnimationReader(){}
    public virtual JsonNode? MakeAnimation(WeditInfomation widget){
      nowWedget = widget;
      try{
        string[] results = converter.Convert("武器/",nowWedget.assetName,null);
        if(results.Length<=0)return null;
        if(nowWedget.assetName.StartsWith("Armor_")){
          results = new string[]{GetArmorJsonFile(results)};
        }
        List<JsonObject> animationNodes = LoadAnimationNode(results);
        if(animationNodes.Count<=0)return null;

        Dictionary<int,List<string>> keys = GetActionKeys(animationNodes);
        return MakeAnimations(keys, animationNodes);
      }catch(InvalidDataException e){
        Console.WriteLine("failed read animation: "+nowWedget.assetName);
        throw e;
      }
    }
    protected virtual JsonArray? MakeAnimations(Dictionary<int,List<string>> multikeys, List<JsonObject> animationNodes){
      JsonArray result = new JsonArray();
      foreach(List<string> keys in multikeys.Values){
        JsonArray times = new JsonArray();
        foreach(string key in keys){
          List<JsonObject> nodes = new List<JsonObject>();
          foreach(JsonObject obj in animationNodes){
            if(!obj.ContainsKey(key))continue;
            nodes.Add(obj[key]!.AsObject());
          }
          JsonNode? anim = MakeAnimation(key,nodes);
          if(anim!=null)times.Add(anim);
        }
        if(times.Count>0)result.Add(times);
      }
      JsonArray? calc = CalcAnimations(result);
      if(calc==null||calc.Count<0)throw new InvalidDataException("failed converted.");
      return calc;
    }
    protected string GetArmorJsonFile(string[] convertedFiles){
        foreach( string path in convertedFiles ){
          int lastIndex = ".json".Length+1;
          if(path[path.Length-lastIndex-1]=='_')continue;
          return path;
        }
        throw new InvalidDataException(convertedFiles.ToString());
    }
    protected virtual JsonArray CalcAnimations(JsonArray animations){
      JsonArray result = new JsonArray();
      foreach(JsonNode? node in animations){
        if(node==null)continue;
        JsonArray? animation = CalcAnimation(node.AsArray());
        if(animation!=null){
          JsonObject animInfo = new JsonObject();
          animInfo.AddData("動き",animation);
          animInfo.AddData("全ループ",false); // よく分からんが全ループのアニメーションは例外的
          result.Add(animInfo);
        }
      }
      if(result.Count<=0)throw new InvalidDataException("feiled read animations.");
      return result;
    }
    protected JsonArray? CalcAnimation(JsonArray animation){
      JsonArray result = new JsonArray();
      JsonObject? checkTime=null;
      bool haveAttack = false;
      foreach(JsonNode? node in animation){
        JsonObject? time = node as JsonObject;
        if(time==null)continue;
        if(time.ContainsKey(attackName)){
          haveAttack = true;
          if(checkTime!=null){
            result.AddTimeData(checkTime);
            checkTime=null;
          }
          result.Add(time.Clone());
        }else{
          if(checkTime!=null)checkTime = checkTime.AddTime(time);
          else checkTime = time.Clone().AsObject();
        }
      }
      if(checkTime!=null){
        result.AddTimeData(checkTime);
      }
      float allTime = 0;
      foreach(JsonNode? time in result){
        allTime += (float)time![timeName]!;
      }
      if(allTime<=0)return null;
      if(!haveAttack)return null;
      return result;
    }
    protected virtual JsonNode? MakeAnimation(string key, List<JsonObject> animationNodes){
      JsonObject result = new JsonObject();
      JsonArray? fireTimes = GetFireTime(animationNodes);
      result.AddData(attackName,fireTimes);
      // チャージ、リロードアクションの場合
      if(fireTimes==null){
        switch(key[key.Length-1]){
          case chargeNum:
          result.AddData(timeName,0.0f);
          //result.AddData(timeName,nowWedget.chargeTime);
          break;
          case reloadNum:
          if(nowWedget.bulletCount>0){
            result.AddData(timeName,0.0f);
            //result.AddData(timeName,nowWedget.reloadTime);
          }
          break;
        }
      }
      if(!result.ContainsKey(timeName)){
        result.AddData(timeName,GetMaxTime(animationNodes));
      }
      return result;
    }
    protected JsonArray? GetFireTime(List<JsonObject> animationNodes){
      JsonArray result = new JsonArray();
      foreach(JsonObject obj in animationNodes){
        JsonArray? times = obj.FetchPath("events") as JsonArray;
        if(times==null)continue;
        foreach(JsonNode? time in times){
          if(time==null)continue;
          JsonObject timeObj = time.AsObject();
          JsonNode? nameNoe = timeObj["name"];
          if(nameNoe==null)continue;
          string name = nameNoe.ToString();
          if(!name.StartsWith(fire))continue;
          result.Add(MakeFireNoe(timeObj));
        }
      }
      if(result.Count<=0)return null;
      return result;
    }
    protected JsonObject MakeFireNoe(JsonObject time){
      JsonObject result = new JsonObject();
      result.AddData("時間",(float)time[timeStr]!);
      string? eventString = (string?)time["string"];
      if(eventString==null)return result;
      const string startWord = fire+"_";
      if(!eventString.StartsWith(startWord))return result;
      string numberString = eventString.Substring(startWord.Length);
      int number = int.Parse(numberString)-1;
      result.AddData("攻撃特性",number);
      return result;
    }
    protected float GetMaxTime(List<JsonObject> animationNodes){
      float time=0;
      foreach(JsonObject animation in animationNodes){
        foreach(KeyValuePair<string,JsonNode?> keyVal in animation)
        {
          if(keyVal.Value==null)continue;
          JsonArray arry = GetFirstArray(keyVal.Value);
          JsonObject? timeObj = arry[arry.Count-1] as JsonObject;
          if(timeObj==null)throw new Exception();
          time = Math.Max(time,(float)timeObj[timeStr]!);
        }
      }
      return time;
    }
    protected JsonArray GetFirstArray(JsonNode node){
      JsonArray? result = node as JsonArray;
      if(result!=null)return result;
      JsonObject? obj = node as JsonObject;
      if(obj==null)throw new Exception();
      if(obj.Count<=0)throw new Exception();
      IEnumerator<KeyValuePair<string, JsonNode?>>  itr = obj.GetEnumerator();
      itr.MoveNext();
      return GetFirstArray(itr.Current.Value!);
    }
    protected Dictionary<int,List<string>> GetActionKeys(List<JsonObject> animationNodes){
      EquipSubType subtype = nowWedget.subtype;
      Dictionary<int,List<string>> keys = GetAllAttackKeys(animationNodes);
      switch(subtype){
        case EquipSubType.強襲武器:
        case EquipSubType.突撃武器:
        case EquipSubType.防壁武器:
        case EquipSubType.片手武器:
        return GetActionKeysSequential(keys,animationNodes);
        case EquipSubType.機銃武器:
        case EquipSubType.突進武器:
        case EquipSubType.炸裂武器:
        case EquipSubType.狙撃武器:
        case EquipSubType.投射武器:
        return keys;
      }
      throw new InvalidDataException("unknown type: " + subtype);
    }
    protected Dictionary<int,List<string>> GetAllAttackKeys(List<JsonObject> animationNodes){
      Dictionary<int,List<string>> result = new Dictionary<int,List<string>>();
      foreach(JsonObject animation in animationNodes){
        foreach(string Key in animation.GetKeys()){
          if(!Key.StartsWith(attack))continue;
          if(attack.Length==Key.Length)continue;
          string[] numbers = Key.Substring(attack.Length).Split("_");
          int main = 0;
          if(numbers.Length==2)main = int.Parse(numbers[0]);
          else if(numbers.Length>2)main = 99;
          List<string>? target = null;
          if(!result.ContainsKey(main))result[main] = target = new List<string>();
          else target = result[main];
          if(!target.Contains(Key))target.Add(Key);
        }
      }
      return result;
    }
    protected bool isFireAction(List<string> keys, List<JsonObject> animationNodes){
      foreach(JsonObject animation in animationNodes){
        if(isFireAction(keys,animation))return true;
      }
      return false;
    }
    protected bool isFireAction(List<string> keys, JsonObject animationNodes){
      foreach(string key in keys){
        JsonObject? anime = animationNodes[key] as JsonObject;
        if(anime==null)continue;
        JsonNode? eventNode = null;
        if(!anime.TryGetPropertyValue("events",out eventNode))continue;
        foreach(JsonNode? timeNode in eventNode!.AsArray()){
          JsonNode? nameNode = null;
          if(!timeNode!.AsObject().TryGetPropertyValue("name",out nameNode))continue;
          if(((string)nameNode!).StartsWith(fire) )return true;
        }
      }
      return false;
    }
    protected Dictionary<int,List<string>> GetActionKeysSequential(Dictionary<int,List<string>> keys, List<JsonObject> animationNodes){
      Dictionary<int,List<string>> result = new Dictionary<int, List<string>>();
      // メイン番号なしがあればそれ
      if(keys.ContainsKey(0)){
        if(isFireAction(keys[0],animationNodes)){
          result.Add(0,keys[0]);
          return result;
        }
      }
      // メイン番号1があれば複数パターン
      if(keys.ContainsKey(1)){
          return keys;
      }
      // メイン番号3から始まってたりするやつはぜんぶまとめ
      List<string> list = new List<string>();
      foreach(List<string> vlaue in keys.Values){
        list.AddRange(vlaue);
      }
      result[0] = list;
      return result;
    }
    protected List<JsonObject> LoadAnimationNode(string[] paths){
      List<JsonObject> result = new List<JsonObject>();
      foreach(string path in paths){
        JsonObject? obj = LoadAnimationNode(path);
        if(obj==null)continue;
        result.Add(obj);
      }
      return result;
    }
    protected virtual JsonObject? LoadAnimationNode(string path){
      JsonNode? node = JsonFile.load(path);
      if(node==null)return null;
      JsonObject? obj = node.FetchPath("animations") as JsonObject;
      return obj;
    }
  }
  static class JsonTimeExtenstion{
    readonly static string timeStr = AttackAnimationReader.timeName;
    public static void AddTimeData(this JsonArray array, JsonObject time){
      if((float)time[timeStr]!<=0)return;
      array.Add(time);
    }
    public static JsonObject AddTime(this JsonObject node1, JsonObject node2){
      float time1 = (float)node1[timeStr]!;
      float time2 = (float)node2[timeStr]!;
      JsonObject result = new JsonObject();
      result.Add(timeStr,time1+time2);
      return result;
    }
  }
}
