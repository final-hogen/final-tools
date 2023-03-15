
using System.Text.Json.Nodes;
using FinalHogen.json;
using FinalHogen.spine.bincode.v36x;

namespace FinalHogen.spine
{
  /// <summary>
  /// コードが長すぎるので、ファイル分割用のクラス特に意味はない
  /// </summary>
  public class BinExtendv36x : BinToJson
  {
    protected BinExtendv36x(){}
    public virtual KeyValuePair<string,JsonNode?> readAttachment(StreamReader reader,string? skinName){
      string? name = reader.read();
      if(String.IsNullOrEmpty(name))throw new Exception("attachment name not found.");
      string? target = reader.read();
      AttachmentType type = reader.read<AttachmentType>();
      JsonObject result = switchAttachmentType(type,name,target,reader);
      if(type!=AttachmentType.Region)result.Add("type",type.toName());

      return new KeyValuePair<string,JsonNode?>(name,result);
    }
    public virtual KeyValuePair<string,JsonNode?> readAnimation(StreamReader reader){
      string? name = reader.read();
      if(String.IsNullOrEmpty(name))throw new Exception("animation name not found.");
      
      JsonObject result = new JsonObject();
      result.AddContent("slots",readSlotAnimation(reader));
      result.AddContent("bones",readBoneAnimation(reader));
      result.AddContent("ik",readIkAnimation(reader));
      result.AddContent("transform",readTransformAnimation(reader));
      result.AddContent("paths",readPathAnimation(reader));
      result.AddContent("deform",readDeformAnimation(reader));
      result.AddContent("drawOrder",readDrawoderAnimation(reader));
      result.AddContent("events",readEventAnimation(reader));

      return new KeyValuePair<string,JsonNode?>(name,result);
    }
    protected JsonObject switchAttachmentType(AttachmentType type, string name,string? target,StreamReader reader){
      switch(type){
        case AttachmentType.Region:return readAttachRegion(name,target,reader);
        case AttachmentType.Boundingbox:return readAttachBoundingbox(name,target,reader);
        case AttachmentType.Mesh:return readAttachMesh(name,target,reader);
        case AttachmentType.Linkedmesh:return readAttachLinkedmesh(name,target,reader);
        case AttachmentType.Path:return readAttachPath(name,target,reader);
        case AttachmentType.Point:return readAttachPoint(name,target,reader);
        case AttachmentType.Clipping:return readAttachClipping(name,target,reader);
      }
      throw new NotImplementedException("not impl atacch."+type.ToString());
    }
    protected void setAttachmentPathData(JsonObject target,string name,string? path, string? targetname){
      bool isNewPath = (path==null)?(false):(name!=path);
      if(targetname!=null)target.Add("name",targetname);
      if(!isNewPath)return;
      target.Add("path",path);
    }
    protected JsonObject readAttachRegion(string name,string? target,StreamReader reader){
      JsonObject result = new JsonObject();

      string? path = reader.read();
      setAttachmentPathData(result,name,path,target);
      result.AddNoDefault("rotation", (float)reader.read());
      result.AddNoDefault("x", (float)reader.read());
      result.AddNoDefault("y", (float)reader.read());
      result.AddNoDefault("scaleX", (float)reader.read());
      result.AddNoDefault("scaleY", (float)reader.read());
      result.AddNoDefault("width", (float)reader.read());
      result.AddNoDefault("height", (float)reader.read());
      result.AddNoDefault("color", reader.ReadRGBAString());

      return result;
    }
    protected JsonObject readAttachBoundingbox(string name,string? target,StreamReader reader){
			int vertexCount = reader.ReadVarint();
      JsonArray  vertices = readVertices(vertexCount,reader);
      if (nonessential){
        reader.ReadRGBAString();// Avoid unused local warning.
      }
      JsonObject result = new JsonObject();
      result.Add("vertexCount",vertexCount);
      result.AddContent("vertices",vertices);
      return result;
    }
    protected JsonObject readAttachMesh(string name,string? target,StreamReader reader){
      JsonObject result = new JsonObject();
      string? path = reader.read();
      setAttachmentPathData(result,name,path,target);
      result.AddNoDefault("color", reader.ReadRGBAString());
      int vertexCount = reader.ReadVarint();
      result.AddContent("uvs", readFloatArray(vertexCount*2,reader));
      int trianglesCount = reader.ReadVarint();
      result.AddContent("triangles", readShortArray(trianglesCount,reader));
      result.AddContent("vertices", readVertices(vertexCount,reader));
      result.AddNoDefault("hull",(int)reader.ReadVarint());
      if (nonessential) {
        int edgesCount = reader.ReadVarint();
        result.AddContent("edges", readShortArray(edgesCount,reader));
        result.AddNoDefault("width",(float)reader.read());
        result.AddNoDefault("height",(float)reader.read());
      }

      return result;
    }
    protected JsonObject readAttachLinkedmesh(string name,string? target,StreamReader reader){
      JsonObject result = new JsonObject();
      string? path = reader.read();
      setAttachmentPathData(result,name,path,target);
      result.AddNoDefault("color", reader.ReadRGBAString());
      result.AddContent("skin",(string?)reader.read());
      result.AddContent("parent",(string?)reader.read());
      result.AddNoDefault("deform",(bool)reader.read());
      if (nonessential) {
        result.AddNoDefault("width",(float)reader.read());
        result.AddNoDefault("height",(float)reader.read());
      }
      return result;
    }
    protected JsonObject readAttachPath(string name,string? target,StreamReader reader){
      JsonObject result = new JsonObject();
      result.AddNoDefault("closed",(bool)reader.read());
      result.AddNoDefault("constantSpeed",(bool)reader.read());
      int vertexCount = reader.ReadVarint();
      result.Add("vertexCount",vertexCount);
      result.AddContent("vertices",readVertices(vertexCount,reader));
      int lengthCount = vertexCount/3;
      result.AddContent("lengths",readFloatArray(lengthCount,reader));

      if (nonessential){
        result.AddNoDefault("color", reader.ReadRGBAString());
      }
      return result;
    }
    protected JsonObject readAttachPoint(string name,string? target,StreamReader reader){
      JsonObject result = new JsonObject();
      result.Add("rotation",(float)reader.read());
      result.Add("x",(float)reader.read());
      result.Add("y",(float)reader.read());
      if (nonessential){
        reader.ReadRGBAString();  // Avoid unused local warning.
      }
      return result;
    }
    protected JsonObject readAttachClipping(string name,string? target,StreamReader reader){
      JsonObject result = new JsonObject();
      result.Add("end",readSlotName(reader));
      int vertexCount = reader.ReadVarint();
      result.Add("vertexCount",vertexCount);
      result.Add("vertices",readVertices(vertexCount,reader));
      if (nonessential){
        result.AddNoDefault("color", reader.ReadRGBAString());
      }
      
      return result;
    }
    protected JsonArray readVertices(int count, StreamReader reader){
      bool isWeight = reader.read();
      if(!isWeight)return readFloatArray( count*2,reader);

      JsonArray result = new JsonArray();
      for(int v=0; v<count; ++v){
        int boneCount = reader.ReadVarint();
        result.Add(boneCount);
        for(int b=0; b<boneCount; ++b){
          result.Add((int)reader.ReadVarint());  // bone number
          result.Add((float)reader.read()); // x
          result.Add((float)reader.read()); // y
          result.Add((float)reader.read()); // w
        }
      }
      return result;
    }
    protected JsonArray readFloatArray(int count, StreamReader reader){
      JsonArray result = new JsonArray();
      for( int i=0; i<count; ++i){
        result.Add((float)reader.read());
      }
      return result;
    }
    protected JsonArray readShortArray(int count, StreamReader reader){
      JsonArray result = new JsonArray();
      for( int i=0; i<count; ++i){
        int h = ((byte)reader.read())<<8;
        int l = (byte)reader.read();
        result.Add(h|l);
      }
      return result;
    }
    protected JsonNode? readAnimationCurve(StreamReader reader){
      CurveTimelineType type = reader.read<CurveTimelineType>();
      switch(type){
        case CurveTimelineType.Stepped:
          return JsonValue.Create("stepped");
        case CurveTimelineType.Bezier:
          return readFloatArray(4,reader);//x1,y1,x2,y2
        case CurveTimelineType.Linear:
          return null;  // do not noting??
      }
      return null;
    }
    public virtual JsonObject readSlotAnimation(StreamReader reader){
      JsonObject result = new JsonObject();
      int slotCount = reader.ReadVarint();
      for(int s=0; s<slotCount; ++s ){
        string? slotName = readSlotName(reader);
        if(String.IsNullOrEmpty(slotName))throw new Exception("animation slot name not found.");
        JsonObject data = namemapLoop(reader,readSlotTimelinev36x);
        result.AddContent(slotName,data);
      }
      return result;
    }
    public KeyValuePair<string,JsonNode?> readSlotTimelinev36x(StreamReader reader){
      JsonArray result = new JsonArray();
      SlotTimelineType type = reader.read<SlotTimelineType>();
      int frameCount = reader.ReadVarint();
      switch(type){
        case SlotTimelineType.Attachment:
					for (int frameIndex = 0; frameIndex < frameCount; frameIndex++){
              JsonObject time = new JsonObject();
              time.Add("time",(float)reader.read());
              time.Add("name",(string?)reader.read());  // nul 可
              result.Add(time);
          }
        break;
        case SlotTimelineType.Color:
					for (int frameIndex = 0; frameIndex < frameCount; frameIndex++){
              JsonObject time = new JsonObject();
              time.Add("time",(float)reader.read());
              time.Add("color",reader.ReadRGBAString());
              if (frameIndex < frameCount - 1){
                time.AddContent("curve",readAnimationCurve(reader));
              }
              result.Add(time);
          }
        break;
        case SlotTimelineType.TwoColor:
					for (int frameIndex = 0; frameIndex < frameCount; frameIndex++){
              JsonObject time = new JsonObject();
              time.Add("time",(float)reader.read());
              time.Add("light",reader.ReadRGBAString());
              time.AddContent("dark",reader.ReadRGBString());
              if (frameIndex < frameCount - 1) {
                time.AddContent("curve",readAnimationCurve(reader));
              }
              result.Add(time);
          }
        break;
        default:
          throw new Exception("unknown timeline type. "+type.ToString());
      }
      return new KeyValuePair<string,JsonNode?>(type.toName(),result);
    }
    public virtual JsonObject readBoneAnimation(StreamReader reader){
      JsonObject result = new JsonObject();
      int count = reader.ReadVarint();
      for(int s=0; s<count; ++s ){
        string? name = readBoneName(reader);
        if(String.IsNullOrEmpty(name))throw new Exception("animation bone name not found.");
        JsonObject data = namemapLoop(reader,readBoneTimelinev36x);
        result.AddContent(name,data);
      }
      return result;
    }
    public KeyValuePair<string,JsonNode?> readBoneTimelinev36x(StreamReader reader){
      JsonArray result = new JsonArray();
      BoneTimelineType type = reader.read<BoneTimelineType>();
      int frameCount = reader.ReadVarint();
      switch(type){
        case BoneTimelineType.Rotate:
					for (int frameIndex = 0; frameIndex < frameCount; frameIndex++) {
            JsonObject time = new JsonObject();
            time.Add("time",(float)reader.read());
            time.Add("angle",(float)reader.read());
            if (frameIndex < frameCount - 1) {
              time.AddContent("curve",readAnimationCurve(reader));
            }
            result.Add(time);
          }
          break;
        case BoneTimelineType.Translate:
        case BoneTimelineType.Scale:
        case BoneTimelineType.Shear:
					for (int frameIndex = 0; frameIndex < frameCount; frameIndex++) {
            JsonObject time = new JsonObject();
            time.Add("time",(float)reader.read());
            time.Add("x",(float)reader.read());
            time.Add("y",(float)reader.read());
            if (frameIndex < frameCount - 1) {
              time.AddContent("curve",readAnimationCurve(reader));
            }
            result.Add(time);
          }
        break;
      }
      return new KeyValuePair<string,JsonNode?>(type.toName(),result);
    }
    public virtual JsonObject readIkAnimation(StreamReader reader){
      JsonObject result = namemapLoop(reader,readIkTimelinev36x);
      return result;
    }
    public KeyValuePair<string,JsonNode?> readIkTimelinev36x(StreamReader reader){
      string? name = readIkName(reader);
      if(String.IsNullOrEmpty(name))throw new Exception("animation ik name not found.");
      JsonArray result = new JsonArray();
      int frameCount = reader.ReadVarint();
			for (int frameIndex = 0; frameIndex < frameCount; frameIndex++) {
        JsonObject time = new JsonObject();
        time.Add("time",(float)reader.read());
        time.AddNoDefault("mix",(float)reader.read());
        time.AddNoDefault("bendPositive",(bool)reader.read());
        if (frameIndex < frameCount - 1) {
          time.AddContent("curve",readAnimationCurve(reader));
        }
        result.Add(time);
      }
      return new KeyValuePair<string,JsonNode?>(name,result);
    }
    public virtual JsonObject readTransformAnimation(StreamReader reader){
      JsonObject result = namemapLoop(reader,readTransformTimelinev36x);
      return result;
    }
    public KeyValuePair<string,JsonNode?> readTransformTimelinev36x(StreamReader reader){
      string? name = readTransformName(reader);
      if(String.IsNullOrEmpty(name))throw new Exception("animation transform name not found.");
      JsonArray result = new JsonArray();
      int frameCount = reader.ReadVarint();
			for (int frameIndex = 0; frameIndex < frameCount; frameIndex++) {
        JsonObject time = new JsonObject();
        time.Add("time",(float)reader.read());
        time.AddNoDefault("rotateMix",(float)reader.read());
        time.AddNoDefault("translateMix",(float)reader.read());
        time.AddNoDefault("scaleMix",(float)reader.read());
        time.AddNoDefault("shearMix",(float)reader.read());
        if (frameIndex < frameCount - 1) {
          time.AddContent("curve",readAnimationCurve(reader));
        }
        result.Add(time);
      }
      return new KeyValuePair<string,JsonNode?>(name,result);
    }
    public virtual JsonObject readPathAnimation(StreamReader reader){
      JsonObject result = new JsonObject();
      int count = reader.ReadVarint();
      for(int s=0; s<count; ++s ){
        string? name = readPathName(reader);
        if(String.IsNullOrEmpty(name))throw new Exception("animation path name not found.");
        JsonObject data = namemapLoop(reader,readPathTimelinev36x);
        result.AddContent(name,data);
      }
      return result;
    }
    public KeyValuePair<string,JsonNode?> readPathTimelinev36x(StreamReader reader){
      JsonArray result = new JsonArray();
      PathTimelineType type = reader.read<PathTimelineType>();
      int frameCount = reader.ReadVarint();
      switch(type){
        case PathTimelineType.Position:
        case PathTimelineType.Spacing:
					for (int frameIndex = 0; frameIndex < frameCount; frameIndex++) {
            JsonObject time = new JsonObject();
            time.Add("time",(float)reader.read());
            time.AddNoDefault(type.toName(),(float)reader.read());
            if (frameIndex < frameCount - 1) {
              time.AddContent("curve",readAnimationCurve(reader));
            }
            result.Add(time);
          }
          break;
        case PathTimelineType.Mix:
					for (int frameIndex = 0; frameIndex < frameCount; frameIndex++) {
            JsonObject time = new JsonObject();
            time.Add("time",(float)reader.read());
            time.Add("rotateMix",(float)reader.read());
            time.Add("translateMix",(float)reader.read());
            if (frameIndex < frameCount - 1) {
              time.AddContent("curve",readAnimationCurve(reader));
            }
            result.Add(time);
          }
        break;
      }
      return new KeyValuePair<string,JsonNode?>(type.toName(),result);
    }
    public virtual JsonObject readDeformAnimation(StreamReader reader){
      JsonObject result = new JsonObject();
      int count = reader.ReadVarint();
      for(int i=0; i<count; ++i ){
        string? name = readSkinName(reader);
        if(String.IsNullOrEmpty(name))throw new Exception("animation deform name not found.");
        JsonObject slots = new JsonObject();
        int slotCount = reader.ReadVarint();
        for(int s=0; s<slotCount; ++s){
          string slotName = readSlotName(reader);
          JsonObject data = namemapLoop(reader,readDeformTimelinev36x);
          slots.AddContent(slotName,data);
        }
        result.Add(name,slots);
      }
      return result;
    }
    public KeyValuePair<string,JsonNode?> readDeformTimelinev36x(StreamReader reader){
      string? vertexName = reader.read();
      if(String.IsNullOrEmpty(vertexName))throw new Exception("deform name not found.");
      JsonArray result = new JsonArray();
      int frameCount = reader.ReadVarint();
		  for (int frameIndex = 0; frameIndex < frameCount; frameIndex++) {
        JsonObject time = new JsonObject();
        time.Add("time",(float)reader.read());
        int end = reader.ReadVarint();
        if(end!=0){
          int start = reader.ReadVarint();
          JsonArray vertices = new JsonArray();
          for (int v = 0; v < end; v++){
					  vertices.Add((float)reader.read());
          }
          // Spine 本体がバグってるので、こちらでoffsetを検出してあげる
          const int roundCount = 3;
          int i;
          for(i=0;i<vertices.Count;++i){
            float value = (float)vertices[i]!;
            if(Math.Round(value,roundCount)!=0)break;
          }
          if(i>=vertices.Count)vertices = new JsonArray();  //全消し
          int offset = start;
          if(i>0&&i<vertices.Count){
            offset += i;
            for(;i>0;--i){  // offsetぶん削除
              vertices.RemoveAt(0);
            }
          }
          // 後ろもチェック
          for(i=0;i<vertices.Count;++i){
            float value = (float)vertices[vertices.Count - i - 1]!;
            if(Math.Round(value,roundCount)!=0)break;
          }
          if(i>0&&i<vertices.Count){
            for(;i>0;--i){  // offsetぶん削除
              vertices.RemoveAt(vertices.Count-1);
            }
          }
          if(vertices.Count>0){
            time.AddNoDefault("offset",offset);
            time.AddContent("vertices",vertices);
          }
        }
        if (frameIndex < frameCount - 1) {
          time.AddContent("curve",readAnimationCurve(reader));
        }
        result.Add(time);
      }
      return new KeyValuePair<string,JsonNode?>(vertexName,result);
    }
    public virtual JsonArray readDrawoderAnimation(StreamReader reader){
      return arrayLoop(reader,readDraworderTimelinev36x);
    }
    public JsonObject readDraworderTimelinev36x(StreamReader reader){
      JsonObject result = new JsonObject();
      result.Add("time",(float)reader.read());
      int offsetCount = reader.ReadVarint();
      JsonArray offsets = new JsonArray();
      for( int o=0;o<offsetCount;++o){
        string slotName = readSlotName(reader);
        if(String.IsNullOrEmpty(slotName))throw new Exception("draworder slotname not found.");
        JsonObject offset = new JsonObject();
        offset.Add("slot",slotName);
        offset.Add("offset",(int)reader.ReadVarint());
        offsets.Add(offset);
      }
      result.Add("offsets",offsets);
      return result;
    }
    public virtual JsonArray readEventAnimation(StreamReader reader){
      return arrayLoop(reader,readEventTimelinev36x);
    }
    public JsonObject readEventTimelinev36x(StreamReader reader){
      JsonObject result = new JsonObject();
      result.Add("time",(float)reader.read());
      string? name = readEventName(reader);
      if(String.IsNullOrEmpty(name))throw new Exception("event timeline name not found.");
      result.Add("name",name);
      int Intdata = reader.ReadVarint();
      float floatdata = reader.read();
      bool haveString = reader.read();
      string? stringdata = null;
      if(haveString)stringdata = reader.read();
      JsonNode? eventNode = converted.FetchPath("events");
      int defaultInt = 0;
      float defaultFloat = 0;
      string? defaultString = null;
      if(eventNode!=null){
        defaultInt=(eventNode["int"]!=null)?((int)(eventNode["int"]??0)):(0);
        defaultFloat=(eventNode["float"]!=null)?((float)(eventNode["float"]??0)):(0);
        defaultString=(eventNode["string"]!=null)?((string?)eventNode["string"]):(null);
      }
      if(Intdata!=defaultInt)result.Add("int",Intdata);
      if(floatdata!=defaultFloat)result.Add("float",floatdata);
      if(stringdata!=defaultString)result.AddContent("string",stringdata);
      return result;
    }
  }
}
