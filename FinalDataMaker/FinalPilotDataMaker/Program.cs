// See https://aka.ms/new-console-template for more information
using FinalHogen.gear;
using FinalHogen.pilot;

namespace FinalHogen
{
  class Program{
    static void Main(string[] args)
    {
      GearFiles.CheckFiles();
      PilotCommonData common = new PilotCommonData(GearFiles.finalFiileNames);
      TalentUndergroundData talent = new TalentUndergroundData(common,GearFiles.finalFiileNames);
      talent.SaveAll();
    }
  }
}
