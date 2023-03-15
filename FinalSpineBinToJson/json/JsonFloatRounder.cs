
using System.Text.Json.Nodes;

namespace FinalHogen.json
{
  /// <summary>
  /// float を探して桁を調整する。
  /// </summary>
  public class JsonFloatRounder
  {
    List<string[]> ignoreList = new List<string[]>();
    Dictionary<string[],float> dropList = new Dictionary<string[],float>();
    Dictionary<string[],int> roundList = new Dictionary<string[],int>();
    protected int roundCount;
    protected MidpointRounding adjusttype;
    public JsonFloatRounder(int defaultRound=2){
      roundCount = defaultRound;
      adjusttype = MidpointRounding.AwayFromZero;
    }
    public void Convert(JsonNode targetNode){
      switch(targetNode){
        case JsonObject obj:
          IEnumerator<KeyValuePair<string, JsonNode?>> itr = obj.GetEnumerator();
          List<string> keys = new List<string>();
          while(itr.MoveNext()){
            keys.Add(itr.Current.Key);
          }
          foreach( string key in keys){
            JsonNode? node = obj[key];
            if(node!=null)Convert(node);
          }
        return;
        case JsonArray array:
          int oldCount = array.Count;
          for(int i=0; i<array.Count;++i){
            JsonNode? node = array[i];
            if(node!=null)Convert(node);
            if(oldCount==array.Count)continue;
            oldCount = array.Count;
            --i; // 消えた
          }
        return;
        case JsonValue value:
          Convert(value);
        return;
      }
    }
    public void SetIgnoreList(string[] paths){
      foreach(string path in paths){
        ignoreList.Add(path.Split("/"));
      }
    }
    public void SetDropList(Dictionary<string,float> pathDropList){
      foreach(KeyValuePair<string,float> keyval in pathDropList){
        string[] path = keyval.Key.Split("/");
        dropList[path] = keyval.Value;
      }
    }
    public void SetRoundList(Dictionary<string,int> pathRoundList){
      foreach(KeyValuePair<string,int> keyval in pathRoundList){
        string[] path = keyval.Key.Split("/");
        roundList[path] = keyval.Value;
      }
    }
    protected bool IsIgnore(string[] nodepath){
      foreach(string[] path in ignoreList){
        if(JsonExtension.MatchFullpath(nodepath,path))return true;
      }
      return false;
    }
    protected bool IsDrop(string[] nodepath,float value){
      foreach(KeyValuePair<string[],float> keyval in dropList){
        if(value!=keyval.Value)continue;
        if(JsonExtension.MatchFullpath(nodepath,keyval.Key))return true;
      }
      return false;
    }
    protected int getRound(string[] nodepath){
      foreach(KeyValuePair<string[],int> keyval in roundList){
        if(JsonExtension.MatchFullpath(nodepath,keyval.Key))return keyval.Value;
      }
      return roundCount;
    }
    protected void Convert(JsonValue value){
      float dval;
      if(!value.TryGetValue<float>(out dval))return;
      string[] valuePath = value.GetFullpath();
      if(IsIgnore(valuePath))return;
      int count = getRound(valuePath);
      float newval = (float)Math.Round(dval,count,adjusttype);
      
      JsonValue newNode = JsonValue.Create(newval);
      bool drop = IsDrop(valuePath,newval);

      string[] paths = value.GetFullpath();
      string key =paths[paths.Length-1];
      switch(value.Parent){
        case JsonObject obj:
          if(drop)obj.Remove(key);
          else obj[key] = newNode;
        return;
        case JsonArray array:
          if(drop)array.RemoveAt(int.Parse(key));
          else array[int.Parse(key)] = newNode;
        return;
        default:
         throw new Exception();
      }
    }
  }
}