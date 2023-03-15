
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FinalHogen.json
{
  /// <summary>
  /// Jsonの中身を大雑把に比較するツール
  /// </summary>
  class JsonCompare{
    double span;      // OK にする幅
    JsonDocument firstDoc;
    JsonDocument secondDoc;
    public List<log> logs;
    /// <param name="checkR">比較する少数低以下の桁数(マイナスにすると幅チェックなし)</param>
    public JsonCompare(JsonNode first, JsonNode second,int checkRound=4){
      firstDoc = JsonDocument.Parse(first.ToJsonString());
      secondDoc = JsonDocument.Parse(second.ToJsonString());
      logs = new List<log>();
      if(checkRound>=0)span = 0.55*Math.Pow(10,-checkRound);
      else span=0;
    }
    ~JsonCompare(){
      firstDoc.Dispose();
      secondDoc.Dispose();
    }
    public void Compare(){
      Compare(firstDoc.RootElement,secondDoc.RootElement,new List<string>());
    }
    public void ToConsole(){
      foreach(log data in logs){
        data.ToConsole();
      }
    }
    protected void Compare(JsonElement first, JsonElement second,List<string> path){
      if(first.ValueKind!=second.ValueKind){
        switch(first.ValueKind){
          case JsonValueKind.Array:
          case JsonValueKind.Object:
            logs.Add(new log(path,log.logtype.diffElement,first.ValueKind,second.ValueKind));
          return;
          case JsonValueKind.True:
          case JsonValueKind.False:
            logs.Add(new log(path,log.logtype.diffValue,first.ValueKind,second.ValueKind));
          return;
          default:
            logs.Add(new log(path,log.logtype.diffValueType,first,second));
          return;
        }
      }
      switch(first.ValueKind){
          case JsonValueKind.Array:
            CompareArray(first,second,path);
          return;
          case JsonValueKind.Object:
            CompareObject(first,second,path);
          return;
          case JsonValueKind.String:
            CompareString(first,second,path);
          return;
          case JsonValueKind.Number:
            CompareNumber(first,second,path);
          return;
          default:
          return; //OK
      }
    }
    protected void CompareObject(JsonElement first, JsonElement second,List<string> path){
      List<string> firstKey = new List<string>();
      List<string> secondKey = new List<string>();
      List<string> bothKey = new List<string>();
      foreach(JsonProperty prop in first.EnumerateObject()){
        firstKey.Add(prop.Name);
      }
      foreach(JsonProperty prop in second.EnumerateObject()){
        if(firstKey.Remove(prop.Name))bothKey.Add(prop.Name);
        else secondKey.Add(prop.Name);
      }
      foreach(string key in firstKey){
        logs.Add(new log(path,log.logtype.diffKey,key,null));
      }
      foreach(string key in secondKey){
        logs.Add(new log(path,log.logtype.diffKey,null,key));
      }
      foreach(string key in bothKey){
        List<string> sub = new List<string>(path);
        sub.Add(key);
        Compare(first.GetProperty(key),second.GetProperty(key),sub);
      }
    }
    protected void CompareArray(JsonElement first, JsonElement second,List<string> path){
      List<JsonElement> firstValue = new List<JsonElement>();
      List<JsonElement> secondValue = new List<JsonElement>();
      foreach(JsonElement element in first.EnumerateArray()){
        firstValue.Add(element);
      }
      foreach(JsonElement element in second.EnumerateArray()){
        secondValue.Add(element);
      }
      if(firstValue.Count!=secondValue.Count){
        logs.Add(new log(path,log.logtype.diffArrayCount,firstValue.Count,secondValue.Count));
      }
      int count = Math.Min(firstValue.Count,secondValue.Count);
      for(int i=0;i<count;++i){
        List<string> sub = new List<string>(path);
        sub.Add(i.ToString());
        Compare(firstValue[i],secondValue[i],sub);
      }
    }
    protected void CompareString(JsonElement first, JsonElement second,List<string> path){
      string? firstvalue = first.GetString();
      string? secondvalue = second.GetString();
      if(firstvalue==secondvalue)return;
      logs.Add(new log(path,log.logtype.diffValue,firstvalue,secondvalue));
    }
    protected void CompareNumber(JsonElement first, JsonElement second,List<string> path){
      double firstvalue = first.GetDouble();
      double secondvalue = second.GetDouble();
      CompareDouble(firstvalue,secondvalue,path);
    }
    protected virtual void CompareDouble(double first,double second, List<string> path){
      bool check = false;
      if(span<=0){
        check = first==second;
      }else{
        check = (first>=(second-span))&&(first<=(second+span));
      }
      if(check)return;
      logs.Add(new log(path,log.logtype.diffValue,first,second));
    }
    public class log{
      public string path;
      public logtype type;
      public object? first;
      public object? second;
      public log(List<string> setPath, logtype setType, object? setFirst, object? setSecond){
        path=String.Join("/",setPath);
        type = setType;
        first = setFirst;
        second = setSecond;
      }
      public log(string[] setPath, logtype setType, object? setFirst, object? setSecond){
        path=String.Join("/",setPath);
        type = setType;
        first = setFirst;
        second = setSecond;
      }
      public static string message(Enum type){
        string internalFormat = type.ToString();
        var displayAttribute = type.GetType().GetField(internalFormat)?.GetCustomAttribute<DisplayAttribute>();
        return (displayAttribute?.Name ?? internalFormat);
      }
      new public string ToString(){
        string[] results = {path
          ,message(type)
          ,(first==null)?("null"):(first.ToString()??"null")
          ,(second==null)?("null"):(second.ToString()??"null")
        };
        return String.Join(": ",results);
      }
      public void ToConsole(){
        Console.WriteLine(this.ToString());
      }
      public enum logtype{
        [Display(Name = "要素違い")] diffElement,
        [Display(Name = "キー違い")] diffKey,
        [Display(Name = "配列数違い")] diffArrayCount,
        [Display(Name = "値の種類違い")] diffValueType,
        [Display(Name = "値違い")] diffValue,
      }
    }
  }
}