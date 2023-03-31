
using System;
using System.Text.Json.Nodes;
using FinalHogen.json;

namespace FinalHogen.starship
{
  public class StarShipBuff : FinalJsonNode
  {
    FinalFileMapper fileMapper;
    public StarShipBuff(Dictionary<string, string> fiileNames)
    : base("StarShipBuff", fiileNames)
    {
      this.saveSubFolder = "戦艦/バフ/";
      fileMapper = new FinalFileMapper("戦艦バフ");
    }
    protected override void SetJsonData(JsonNode node){
      string fileName = node.FetchPath("BuffName").ToLang(Lang)!;
      if(fileName==null)return;
      jsonData[fileName] = node;
    }
    public override JsonNode? Convert(string id)
    {
      JsonNode? node = Get(id);
      if (node == null) return null;

      JsonObject result = new JsonObject();
      string name = node.FetchPath("BuffName").ToLang(Lang)!;
      result.WritePathData("名前",name);
      result.WritePathData("説明",node.FetchPath("Describe").ToLang(Lang));
      result.WritePathData("レベル",node.FetchPath("Bufflevel"));

      string imageID = node.FetchPath("BuffIcon")!.ToString();
      fileMapper.AddImage(this.saveSubFolder+name+"/アイコン",imageID);
      return result;
    }
    public override void SaveAll()
    {
      base.SaveAll();
      fileMapper.Save();
    }
  }
}
