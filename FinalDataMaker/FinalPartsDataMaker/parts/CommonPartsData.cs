
using System.Text.Json.Nodes;
using FinalHogen.json;

namespace FinalHogen.parts
{
  /// <summary>
  /// 共通で参照するデータをまとめた物
  /// </summary>
  public class CommonPartsData{
    public FinalParts WidgetData;
    public WidgetIntroMaker WidgetIntroData;
    public SuitDataMaker SuitData;
    public LegData LegData;
    public FinalParts SkillData;
    public FinalParts BattleConfigData;
    protected FinalFileMapper FileMapper;
    public JsonNode WidgetNode{get{return WidgetData.baseNode;}}
    public CommonPartsData(Dictionary<string,string> fiileNames){
      FileMapper = new FinalFileMapper("パーツ");
      WidgetData = new FinalParts("WidgetData",fiileNames);
      WidgetIntroData = new WidgetIntroMaker(fiileNames);
      SuitData = new SuitDataMaker(fiileNames);
      LegData = new LegData(fiileNames);
      SkillData = new FinalParts("SkillData",fiileNames);
      BattleConfigData = new FinalParts("BattleConfigData",fiileNames);
    }
    public void AddImage(string newPath, string sourcePath){
      FileMapper.AddImage(newPath,sourcePath);
    }
    public void SaveFileMap(){
      FileMapper.Save();
    }
  }
}
