// See https://aka.ms/new-console-template for more information
using FinalHogen.gear;
using FinalHogen.story;

namespace FinalHogen
{
  class Program{
    static void Main(string[] args)
    {
      Dictionary<string,string> files = GearFiles.MakeFinalfilenames();
      StoryAllStorys storys = new StoryAllStorys(files);
      storys.SaveAll();
    }
  }
}

