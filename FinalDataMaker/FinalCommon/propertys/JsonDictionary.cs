
using System.Text.Json.Nodes;
using FinalHogen.json;

namespace FinalHogen.property
{
  /// <summary>
  /// Jsonノードデータを辞書っぽくする
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public abstract class JsonDictionary<T> where T : class?
  {
    public JsonNode baseNode{get;}
    protected string saveSubFolder = "";
    protected JsonFinalLang Lang;
    protected Dictionary<string,T> jsonData = new Dictionary<string, T>();
    public JsonDictionary(string targetName, Dictionary<string,string> fiileNames)
    :this(LoadFile(targetName,fiileNames),fiileNames){}
    protected JsonDictionary(JsonNode node,Dictionary<string,string> fiileNames){
      baseNode = node;
      Lang = JsonFinalLang.JP.Get(fiileNames);
      SetAllJsonData(node);
    }
    public static JsonNode LoadFile(string targetName, Dictionary<string,string> fiileNames){
      string? fileName = fiileNames[targetName];
      if(fileName==null)throw new ArgumentException("file not found. "+targetName);
      JsonNode? load = JsonFile.load(fileName);
      if(load==null)throw new ArgumentException("json load failed. "+fileName);
      return load;
    }
    protected virtual void SetAllJsonData(JsonNode allNode){
      JsonArray array = allNode.AsArray();
      for(int i=0;i<array.Count;++i){
        JsonNode? node = array[i];
        if(node==null)continue;
        SetJsonData(node);
      }
    }
    protected virtual void SetJsonData(JsonNode node){
      throw new NotImplementedException();
    }
    public T? Get(JsonNode? id){
      if(id==null)return null;
      return Get(id.ToString());
    }
    public T? Get(string id){
      if(!jsonData.ContainsKey(id))return null;
      return jsonData[id];
    }
    public T? Get(int id){
      return Get(id.ToString());
    }
  }
  /// <summary>
  /// Jsonノードの辞書
  /// </summary>
  public class JsonNodeDictionary : JsonDictionary<JsonNode>{
    public JsonNodeDictionary(string targetName, Dictionary<string,string> fiileNames)
    :this(LoadFile(targetName,fiileNames),fiileNames){}
    protected JsonNodeDictionary(JsonNode node,Dictionary<string,string> fiileNames)
    :base(node,fiileNames){}
    protected override void SetJsonData(JsonNode node){
      JsonNode? id = node.FetchPath("BaseData\u002B\u003CID\u003Ek__BackingField");
      if(id==null)return;
      jsonData[id.ToString()] = node;
    }
  }
}
