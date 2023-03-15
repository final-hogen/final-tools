
using FinalHogen.json;

namespace FinalHogen.spine
{
  class FloatRounderv36x : JsonFloatRounder
  {
    static Dictionary<string,float> v36xDropList = new Dictionary<string, float>(){
      {"bones/*/x",0},
      {"bones/*/y",0},
      {"bones/*/scaleX",0},
      {"bones/*/scaleY",0},
      {"bones/*/rotation",0},
      {"skins/*/*/*/x",0},
      {"skins/*/*/*/y",0},
      {"skins/*/*/*/rotation",0},
      {"transform/*/scaleX",0},
      {"transform/*/scaleY",0},
    };
    static Dictionary<string,int> v36xRoundList = new Dictionary<string, int>(){
      {"animations//time",4},
      {"bones/*/scaleX",3},
      {"bones/*/scaleY",3},
      {"skins/*/*/*/scaleX",3},
      {"skins/*/*/*/scaleY",3},
      {"skins/*/*/*/uvs/*",4},
      {"skins/*/*/*/vertices/*",4},
      {"animations/*/*/*/*/mix",3},
      {"animations/*/*/*/*/rotateMix",3},
      {"animations//vertices/*",4},
      {"animations/*/bones/*/*/*/curve/*",3},
      {"animations/*/bones/*/scale/*/x",3},
      {"animations/*/bones/*/scale/*/y",3},
    };
    public FloatRounderv36x() : base(2)
    {
      setList();
    }
    protected virtual void setList(){
      SetDropList(v36xDropList);
      SetRoundList(v36xRoundList);
    }
  }
}