// See https://aka.ms/new-console-template for more information
using FinalHogen.gear;
using FinalHogen.story;

namespace FinalHogen
{
  class Program{
    static void Main(string[] args)
    {
      GearFiles.CheckFiles();
      StoryAllStorys storys = new StoryAllStorys(GearFiles.finalFiileNames);
      storys.SaveAll();
    }
  }
}

