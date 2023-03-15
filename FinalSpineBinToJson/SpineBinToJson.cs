
using System.Text.Json.Nodes;
using FinalHogen.json;

namespace FinalHogen.spine
{
  abstract public class BinToJson
  {
    protected delegate JsonNode? ArrayDelegate(StreamReader stream);
    protected delegate KeyValuePair<string,JsonNode?> NamemapDelegate(StreamReader stream);
    protected JsonObject converted;
    protected bool nonessential;
    protected List<string> boneNames = new List<string>();
    protected List<string> slotNames = new List<string>();
    protected List<string> ikNames = new List<string>();
    protected List<string> transformNames = new List<string>();
    protected List<string> pathNames = new List<string>();
    protected List<string> skinNames = new List<string>();
    protected List<string> eventNames = new List<string>();
    public BinToJson()
    {
      converted = new JsonObject();
    }
    /// <returns>ex: "3.6.56"</returns>
    public static string? GetFileVersion(string filepath){
      if(!File.Exists(filepath))return null;
      using FileStream file = new FileStream(filepath,FileMode.Open,FileAccess.Read);
      StreamReader reader = new StreamReader(file);
      string? hash = reader.read();
      string? version = reader.read();
      return version;
    }
    public virtual JsonObject convertAll(Stream stream){
      throw new NotImplementedException();
    }
    protected virtual JsonArray arrayLoop(StreamReader reader, ArrayDelegate act){
      JsonArray result = new JsonArray();
      int count = reader.ReadVarint();
      for( int i=0; i<count; ++i ){
        JsonNode? node = act(reader);
        if(node!=null)result.Add(node);
      }
      return result;
    }
    protected virtual JsonObject namemapLoop(StreamReader reader, NamemapDelegate act){
      JsonObject result = new JsonObject();
      int count = reader.ReadVarint();
      for( int i=0; i<count; ++i ){
        KeyValuePair<string,JsonNode?> nameData = act(reader);
        if(result.ContainsKey(nameData.Key)&&nameData.Value!=null){
          JsonNode? node = result[nameData.Key];
          if(node!=null)node.Merge(nameData.Value);
        }else result.AddContent(nameData.Key,nameData.Value);
      }
      return result;
    }
    protected string readEventName(StreamReader reader){
      int number = reader.ReadVarint();
      string name = this.eventNames[number];
      return name;
    }
    protected string readSkinName(StreamReader reader){
      int number = reader.ReadVarint();
      string name = this.skinNames[number];
      return name;
    }
    protected string readPathName(StreamReader reader){
      int number = reader.ReadVarint();
      string name = this.pathNames[number];
      return name;
    }
    protected string readTransformName(StreamReader reader){
      int number = reader.ReadVarint();
      string name = this.transformNames[number];
      return name;
    }
    protected string readIkName(StreamReader reader){
      int number = reader.ReadVarint();
      string name = this.ikNames[number];
      return name;
    }
    protected JsonArray readBoneNames(StreamReader reader){
      JsonArray bones = new JsonArray();
      int count = reader.ReadVarint();
      for(int i=0; i<count; ++i){
        bones.Add(readBoneName(reader));
      }
      return bones;
    }
    protected string readBoneName(StreamReader reader){
      int number = reader.ReadVarint();
      string name = this.boneNames[number];
      return name;
    }
    protected string readSlotName(StreamReader reader){
      int number = reader.ReadVarint();
      string name = this.slotNames[number];
      return name;
    }
  }
  static class SpineExtends{
    public static bool AddNoDefault(this JsonObject node, string key, JsonNode? item){
      if(item==null)return false;
      switch(key){
        case "local":if(false==(bool?)item)return false;break;
        case "relative":if(false==(bool?)item)return false;break;
        case "bendPositive":if(true==(bool?)item)return false;break;
        case "closed":if(false==(bool?)item)return false;break;
        case "deform":if(true==(bool?)item)return false;break;
        case "constantSpeed":if(true==(bool?)item)return false;break;

        case "offset":if(0==(int?)item)return false;break;
        
        case "fps":if(30==(float?)item)return false;break;
        case "rotation":if(0==(float?)item)return false;break;
        case "position":if(0==(float?)item)return false;break;
        case "x":if(0==(float?)item)return false;break;
        case "y":if(0==(float?)item)return false;break;
        case "length":if(0==(float?)item)return false;break;
        case "spacing":if(0==(float?)item)return false;break;
        case "scaleX":if(1==(float?)item)return false;break;
        case "scaleY":if(1==(float?)item)return false;break;
        case "shearX":if(0==(float?)item)return false;break;
        case "shearY":if(0==(float?)item)return false;break;
        case "rotateMix":if(1==(float?)item)return false;break;
        case "translateMix":if(1==(float?)item)return false;break;
        case "scaleMix":if(1==(float?)item)return false;break;
        case "shearMix":if(1==(float?)item)return false;break;

        case "hull":if(0==(int?)item)return false;break;

        case "color":if("ffffffff"==(string?)item)return false;break;
        case "dark":if("ffffff"==(string?)item)return false;break;

        case "blend":if("normal"==(string?)item)return false;break;
        case "positionMode":if("percent"==(string?)item)return false;break;
        case "spacingMode":if("length"==(string?)item)return false;break;
        case "rotateMode":if("tangent"==(string?)item)return false;break;
        case "attachmenttype":if("region"==(string?)item)return false;break;
      }
      return node.AddContent(key,item);
    }
    public static string toTopLower(string src){
      if (String.IsNullOrEmpty(src))return src;
      return char.ToLower(src[0]) + src.Substring(1);
    }
    public static string toName(this Enum data){
      return toTopLower(data.ToString());
    }
  }
}