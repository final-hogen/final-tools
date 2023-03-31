// See https://aka.ms/new-console-template for more information
using FinalHogen.gear;
using FinalHogen.starship;

namespace FinalHogen
{
  class Program{
    static void Main(string[] args)
    {
      GearFiles.CheckFiles();
      StarShipWepon wepon = new StarShipWepon(GearFiles.finalFiileNames);
      wepon.SaveAll();
      StarShipBuff buff = new StarShipBuff(GearFiles.finalFiileNames);
      buff.SaveAll();
    }
  }
}
