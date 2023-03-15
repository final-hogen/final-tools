
using System.Text.Json.Nodes;
using FinalHogen.json;
using FinalHogen.spine.bincode.v36x;

namespace FinalHogen.spine
{
  /// <summary>
  /// バイナリデータを読み込んでJSONを出力するファイル
  /// 少数の有効桁数とかは見てないので、必要なら後でまとめて調整のこと。
  /// </summary>
  public class BinToJsonv36x : BinExtendv36x
  {
    protected string? boneRootColor=null;
    public BinToJsonv36x():base(){}
    public override JsonObject convertAll(Stream stream){
      StreamReader reader = new StreamReader(stream);
      converted.AddContent("skeleton",readSkeleton(reader));
      converted.AddContent("bones",readBones(reader));
      converted.AddContent("slots",readSlots(reader));
      converted.AddContent("ik",readIks(reader));
      converted.AddContent("transform",readTransforms(reader));
      converted.AddContent("path",readPaths(reader));
      converted.AddContent("skins",readSkins(reader));
      converted.AddContent("events",readEvents(reader));
      converted.AddContent("animations",readAnimations(reader));

    JsonFloatRounder rounder = new FloatRounderv36x();
    rounder.Convert(converted);
      return converted;
    }
    protected virtual JsonObject readSkeleton(StreamReader reader){
      return readSkeletonv36x(reader);
    }
    protected JsonObject readSkeletonv36x(StreamReader reader){
      JsonObject result = new JsonObject();
      string? hash = reader.read();
      result.AddContent("hash",hash);
      string? version = reader.read();
      result.AddContent("spine",version);
      float width = reader.read();
      result.Add("width",width);
      float height = reader.read();
      result.Add("height",height);

      this.nonessential = reader.read();
			if (nonessential) {
				float fps = reader.read();
       if(fps!=0) result.AddNoDefault("fps",fps);
        string? imagesPath = reader.read();
				if (String.IsNullOrEmpty(imagesPath)) imagesPath = null;
        result.AddContent("images",imagesPath);
			}
      return result;
    }
    protected virtual JsonArray readBones(StreamReader reader){
      JsonArray result = new JsonArray();
      int count = reader.ReadVarint();
      for( int i=0; i<count; ++i ){
        JsonNode? node = readBonev36x(reader,i==0);
        result.Add(node);
      }
      return result;
    }
    protected JsonObject readBonev36x(StreamReader reader,bool isRoot){
      JsonObject result = new JsonObject();
      string? name = reader.read();
      if(name==null)throw new Exception("bone name not found.");
      boneNames.Add(name);
      result.Add("name",name);

      if(!isRoot){
        int parentId = reader.ReadVarint();
        string searchParent = boneNames[parentId];
        result.Add("parent",searchParent);
      }
      result.AddNoDefault("rotation", (float)reader.read());
      result.AddNoDefault("x", (float)reader.read());
      result.AddNoDefault("y", (float)reader.read());
      result.AddNoDefault("scaleX", (float)reader.read());
      result.AddNoDefault("scaleY", (float)reader.read());
      result.AddNoDefault("shearX", (float)reader.read());
      result.AddNoDefault("shearY", (float)reader.read());
      result.AddNoDefault("length", (float)reader.read());
      int transModeNumber = reader.ReadVarint();
      TransformMode transformMode = StaticValues.TransformModeValues[transModeNumber];
      if(TransformMode.Normal!=transformMode)result.Add("transform",transformMode.toName());
      if (nonessential) {
        string color = reader.ReadRGBAString();
        if(isRoot)this.boneRootColor = color;
        else if(color!=this.boneRootColor)result.Add("color",color);
        //int dummy = reader.read(); // Skip bone color.
      }
      return result;
    }
    protected virtual JsonArray readSlots(StreamReader reader){
      return arrayLoop(reader,readSlotv36x);
    }
    protected JsonObject readSlotv36x(StreamReader reader){
      JsonObject result = new JsonObject();
      string? name = reader.read();
      if(name==null)throw new Exception("slot name not found.");
      slotNames.Add(name);
      result.Add("name",name);

      string? boneName = readBoneName(reader);
      if(boneName==null)throw new Exception("slot bone name not found.");
      result.Add("bone",boneName);

      result.AddNoDefault("color",reader.ReadRGBAString());
      result.AddNoDefault("dark",reader.ReadRGBString());

      result.AddContent("attachment",(string?)reader.read());
      
      BlendMode blendmode = reader.read<BlendMode>();
      result.AddNoDefault("blend",blendmode.toName());

      return result;
    }
    protected virtual JsonArray readIks(StreamReader reader){
      return arrayLoop(reader,readIkv36x);
    }
    protected JsonObject readIkv36x(StreamReader reader){
      JsonObject result = new JsonObject();
      string? name = reader.read();
      if(name==null)throw new Exception("ik name not found.");
      ikNames.Add(name);
      result.Add("name",name);

      int order = reader.ReadVarint();
      result.Add("order",order);
      JsonArray bones = readBoneNames(reader);
      result.AddContent("bones",bones);
      result.Add("target",readBoneName(reader));

      float mix = reader.read();
      if(mix!=1)result.Add("mix",mix);
      bool bendDirection = reader.read();
      if(bendDirection!=true)result.Add("bendPositive",bendDirection);

      return result;
    }
    protected virtual JsonArray readTransforms(StreamReader reader){
      return arrayLoop(reader,readTransformv36x);
    }
    protected JsonObject readTransformv36x(StreamReader reader){
      JsonObject result = new JsonObject();
      string? name = reader.read();
      if(name==null)throw new Exception("transform name not found.");
      transformNames.Add(name);
      result.Add("name",name);

      int order = reader.ReadVarint();
      result.Add("order",order);
      JsonArray bones = readBoneNames(reader);
      result.AddContent("bones",bones);
      result.Add("target",readBoneName(reader));
      
      result.AddNoDefault("local",(bool)reader.read());
      result.AddNoDefault("relative",(bool)reader.read());
      result.AddNoDefault("rotation",(float)reader.read());
      result.AddNoDefault("x",(float)reader.read());
      result.AddNoDefault("y",(float)reader.read());
      float scaleX = (float)reader.read();
      if(scaleX!=0)result.AddNoDefault("scaleX",scaleX);
      float scaleY = (float)reader.read();
      if(scaleY!=0)result.AddNoDefault("scaleY",scaleY);
      //result.AddNoDefault("shearX",(float)reader.read()); //ない
      result.AddNoDefault("shearY",(float)reader.read());
      result.AddNoDefault("rotateMix",(float)reader.read());
      result.AddNoDefault("translateMix",(float)reader.read());
      result.AddNoDefault("scaleMix",(float)reader.read());;
      result.AddNoDefault("shearMix",(float)reader.read());
      return result;
    }
    protected virtual JsonArray readPaths(StreamReader reader){
      return arrayLoop(reader,readpathv36x);
    }
    protected JsonObject readpathv36x(StreamReader reader){
      JsonObject result = new JsonObject();
      string? name = reader.read();
      if(name==null)throw new Exception("transform name not found.");
      pathNames.Add(name);
      result.Add("name",name);
      int order = reader.ReadVarint();
      result.Add("order",order);

      JsonArray bones = readBoneNames(reader);
      result.AddContent("bones",bones);
      result.Add("target",readSlotName(reader));

      PositionMode positionMode = reader.read<PositionMode>();
      result.AddNoDefault("positionMode",positionMode.toName());
      SpacingMode spacingMode = reader.read<SpacingMode>();
      result.AddNoDefault("spacingMode",spacingMode.toName());
      RotateMode rotateMode = reader.read<RotateMode>();
      result.AddNoDefault("rotateMode",rotateMode.toName());

      result.AddNoDefault("rotation",(float)reader.read());
      result.AddNoDefault("position",(float)reader.read());
      result.AddNoDefault("spacing",(float)reader.read());
      result.AddNoDefault("rotateMix",(float)reader.read());
      result.AddNoDefault("translateMix",(float)reader.read());

      return result;
    }
    protected virtual JsonObject readSkins(StreamReader reader){
      JsonObject result = new JsonObject();
      JsonObject defaultSkin = readSkinv36x(reader);
      result.AddContent("default",defaultSkin);
      skinNames.Add("default");

      int count = reader.ReadVarint();
      for( int i=0; i<count; ++i ){
        string? name = reader.read();
        if(String.IsNullOrEmpty(name))throw new Exception("skinn ame not found.");
        skinNames.Add(name);
        JsonObject data = readSkinv36x(reader,name);
        result.AddContent(name,data);
      }
      return result;
    }
    protected JsonObject  readSkinv36x(StreamReader reader,string? skinName=null){
      JsonObject result = new JsonObject();
      int slotCount = reader.ReadVarint();
			for (int i = 0; i < slotCount; i++) {
				string slotName = readSlotName(reader);
        JsonObject slot = new JsonObject();
        result.Add(slotName,slot);
        int entryCount = reader.ReadVarint();
        for (int j = 0; j < entryCount; j++) {
          KeyValuePair<string,JsonNode?> keyval = readAttachment(reader,skinName);
          if(keyval.Value!=null)slot.Add(keyval.Key,keyval.Value);
        }
      }
      return result;
    }
    protected virtual JsonObject readEvents(StreamReader reader){
      JsonObject result = new JsonObject();
      int count = reader.ReadVarint();
      for( int i=0; i<count; ++i ){
        KeyValuePair<string,JsonNode?> nameData = readEvents36x(reader);
        result.Add(nameData.Key,nameData.Value);  // 空でも追加
      }
      return result;
    }
    protected KeyValuePair<string,JsonNode?> readEvents36x(StreamReader reader){
      string? name = reader.read();
      if(name==null)throw new Exception("event name not found.");
      eventNames.Add(name);

      JsonObject result = new JsonObject();
      int intdata = reader.ReadVariantNoOptimize();
      if(intdata!=0)result.Add("int",intdata);
      float floatdata = reader.read();
      if(floatdata!=0)result.Add("float",floatdata);
      string? stringdata = reader.read();
      if(!String.IsNullOrEmpty(stringdata))result.Add("string",stringdata);

      return new  KeyValuePair<string,JsonNode?>(name,result);
    }
    protected virtual JsonObject readAnimations(StreamReader reader){
      return namemapLoop(reader,readAnimation);
    }
  }
}