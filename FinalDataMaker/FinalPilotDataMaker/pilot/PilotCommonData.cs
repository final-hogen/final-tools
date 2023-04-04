
using FinalHogen.json;

namespace FinalHogen.pilot
{
  public class PilotCommonData
  {
    public FinalSystemConfig systemConfig;
    public FinalPropertyName propertyName;
    public PilotCommonData(Dictionary<string,string> fileNames){
      propertyName = new FinalPropertyName(fileNames);
      systemConfig = new FinalSystemConfig(fileNames);
    }
  }
}
