
using System.Text.Json.Nodes;
using FinalHogen.property;

namespace FinalHogen.json
{
  /// <summary>
  /// GearJSONでーた基本クラス
  /// </summary>
  public abstract class FinalJsonNode : JsonNodeDictionary{
    public FinalJsonNode(string targetName, Dictionary<string,string> fiileNames)
    :base(targetName,fiileNames){}
    protected FinalJsonNode(JsonNode node,Dictionary<string,string> fiileNames)
    :base(node,fiileNames){}
    public virtual  JsonNode? Convert(string id){
      throw new NotImplementedException();
    }
    public virtual  void SaveAll(){
      foreach(string key in jsonData.Keys){
        JsonNode? node = Convert(key);
        if(node==null)continue;
        Save(key,node);
      }
    }
    protected virtual void Save(string id, JsonNode node){
      Save(saveSubFolder, id, node);
    }
    protected virtual void Save(string subfolder, string id, JsonNode node){
      FinalJsonNode.save(subfolder, id+".json", node);
    }
    public static void save(string subdir, string path, JsonNode node){
      JsonFinalExtension.save(subdir,path,node);
    }
  }
}
