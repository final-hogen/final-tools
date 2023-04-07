
using FinalHogen.json;
using FinalHogen.property;

namespace FinalHogen.pilot
{
  public class PilotCommonData
  {
    public FinalSystemConfig systemConfig;
    public FinalPropertyName propertyName;
    public PilotNameDictionary pilotName;
    public FinalKeyToText keyTotext;
    public GIrlSkill skill;
    public MCVGirlSkill MCVskill;
    public PilotCommonData(Dictionary<string,string> fileNames){
      propertyName = new FinalPropertyName(fileNames);
      systemConfig = new FinalSystemConfig(fileNames);
      pilotName = new PilotNameDictionary(fileNames);
      keyTotext = new FinalKeyToText(fileNames);
      skill = new GIrlSkill(fileNames);
      MCVskill = new MCVGirlSkill(fileNames);
    }
  }
}
