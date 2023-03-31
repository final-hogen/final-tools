
using FinalHogen.gear;
using FinalHogen.parts;

namespace FinalHogen
{
  class Program
  {
    static void Main(string[] args)
    {
      GearFiles.CheckFiles();
      
      convert(GearFiles.finalFiileNames);
    }
    static void convert(Dictionary<string,string> fiileNames){
      CommonPartsData common = new CommonPartsData(fiileNames);
      
      WeponPartsmaker wepon = new FinalWeponPartsmaker(common,fiileNames);
      wepon.SaveAll();
      LegPartsmaker leg = new LegPartsmaker(common,fiileNames);
      leg.SaveAll();
      BodyPartsmaker body = new BodyPartsmaker(common,fiileNames);
      body.SaveAll();
      wepon.SaveAll();
      BagPartsmaker bag = new BagPartsmaker(common,fiileNames);
      bag.SaveAll();
      ChipPartsmaker chip = new ChipPartsmaker(common,fiileNames);
      chip.SaveAll();
      common.SaveFileMap();
    }
  }
}
