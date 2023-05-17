// See https://aka.ms/new-console-template for more information
using FinalHogen.gear;
using FinalHogen.pilot;

namespace FinalHogen
{
  class Program{
    static void Main(string[] args)
    {
      Dictionary<string,string> files = GearFiles.MakeFinalfilenames();
      PilotCommonData common = new PilotCommonData(files);
      TalentUndergroundData talent = new TalentUndergroundData(common,files);
      talent.SaveAll();
      PilotData pilot = new PilotData(common,talent,files);
      pilot.SaveAll();
    }
  }
}
