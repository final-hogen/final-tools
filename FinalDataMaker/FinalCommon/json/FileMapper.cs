
using System.Text.Json.Nodes;

namespace FinalHogen.json
{
  /// <summary>
  /// 画像の名前マップ
  /// </summary>
  public class FinalFileMapper{
    protected string name;
    protected JsonObject map = new JsonObject();
    public FinalFileMapper(string subName){
      name = subName;
    }
    public void AddImage(string newPath, string sourcePath){
      map[newPath] = sourcePath;
    }
    public void Save(){
      JsonFinalExtension.save("","filemapper"+name+".json",map);
    }
  }
}

