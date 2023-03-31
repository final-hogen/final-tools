
using System.Text.Json.Nodes;
using FinalHogen.json;

namespace FinalHogen.parts
{
  /// <summary>
  /// パーツコンバータ基本クラス
  /// </summary>
  public class FinalParts : FinalJsonNode{
    public FinalParts(string targetName, Dictionary<string,string> fiileNames)
    :this(LoadFile(targetName,fiileNames),fiileNames){}
    protected FinalParts(JsonNode node,Dictionary<string,string> fiileNames)
    :base(node,fiileNames){}
  }
}
