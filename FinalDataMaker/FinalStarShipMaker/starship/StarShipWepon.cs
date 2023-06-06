
using System.Text.Json.Nodes;
using FinalHogen.json;
using FinalHogen.gear;

namespace FinalHogen.starship
{
  public class StarShipWepon : FinalJsonNode
  {
    FinalFileMapper fileMapper;
    public StarShipWepon(Dictionary<string, string> fiileNames)
    : base("StarShipWepon", fiileNames)
    {
      this.saveSubFolder = "戦艦/艦載砲/";
      fileMapper = new FinalFileMapper("戦艦艦載砲");
    }
    protected override void SetJsonData(JsonNode node){
      string name = node.FetchPath("Weaponname").ToLang(Lang)!;
      string rare = node.FetchEnum<WeaponQuality>("WeaponQuality").ToString();
      string fileName = rare + "-" + name;
      if(fileName==null)return;
      JsonArray? target = null;
      if(jsonData.ContainsKey(fileName))target = jsonData[fileName] as JsonArray;
      else  jsonData[fileName] = target = new JsonArray();
      target!.Add(node.Clone());
    }
    public override JsonNode? Convert(string id)
    {
      JsonArray? nodes= Get(id) as JsonArray;
      if (nodes == null) return null;

      JsonObject result = new JsonObject();
      JsonNode node = nodes[0]!;
      string name = node.FetchPath("Weaponname").ToLang(Lang)!;
      result.WritePathData("名前",name);
      result.WritePathData("説明",node.FetchPath("Describe").ToLang(Lang));
      result.WritePathData("レアリティ",node.FetchEnum<WeaponQuality>("WeaponQuality").ToString());
      result.WritePathData("種別",node.FetchEnum<Category>("Category").ToString());
      result.WritePathData("発動タイプ",node.FetchEnum<Weapontype>("Weapontype").ToString());
      result.WritePathData("ダメージタイプ",node.FetchEnum<ShipWeaponType>("ShipWeaponType").ToString());
      foreach(JsonNode? ranknode in nodes){
        if(ranknode==null)continue;
        string Rank = "ランク/"+ranknode.FetchPath("Star")!.ToString()+"/";
        result.WritePathData(Rank+"説明",ranknode.FetchPath("EffectDescribe").ToLang(Lang));
        int minRange = (int)ranknode.FetchPath("Range/0")!;
        if(minRange>0){
          result.WritePathData(Rank+"射程/最小",minRange);
          result.WritePathData(Rank+"射程/最大",ranknode.FetchPath("Range/1"));
        }
      }
      
      string imageID = node.FetchPath("WeaponIcon")!.ToString();
      fileMapper.AddImage(this.saveSubFolder+name+"/アイコン",imageID);
      imageID = node.FetchPath("LongWeaponIcon")!.ToString();
      fileMapper.AddImage(this.saveSubFolder+name+"/ロング",imageID);
        
      return result;
    }
    public override void SaveAll()
    {
      base.SaveAll();
      fileMapper.Save();
    }
  }
}
