// See https://aka.ms/new-console-template for more information
using FinalHogen.gear;
using FinalHogen.starship;

namespace FinalHogen
{
  class Program{
    static void Main(string[] args)
    {
      Dictionary<string,string> files = GearFiles.MakeFinalfilenames();
      StarShipWepon wepon = new StarShipWepon(files);
      wepon.SaveAll();
      StarShipBuff buff = new StarShipBuff(files);
      buff.SaveAll();
    }
  }
}
