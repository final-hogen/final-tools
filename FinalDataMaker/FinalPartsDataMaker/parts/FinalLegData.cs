
using System.Text.Json.Nodes;
using FinalHogen.json;

namespace FinalHogen.parts
{
  /// <summary>
  /// 武器モデル情報(弾丸付き)
  /// </summary>
  public class LegData : FinalParts{
    public LegData(Dictionary<string,string> fiileNames)
    :base("EquipLegData",fiileNames){
    }
    public string? GetPrefabName(string id){
      JsonNode? node = Get(id);
      if(node==null)return null;
      node = node.FetchPath("prefab");
      if(node==null)return null;
      
      return node.ToString();
    }
    public override JsonNode? Convert(string id){
      return Convert(Get(id));
    }
    protected JsonNode? Convert(JsonNode? source){
      if(source==null)return null;
      JsonNode? itemNode = source.FetchPath("components/_items/0");
      if(itemNode==null)return null;

      JsonObject result = new JsonObject();
      result.AddData("貫通耐性",itemNode.FetchPath("bulletResistance"));
      result.AddData("衝撃耐性",GetResist(source));
      result.AddData("移動速度",GetSpeed(itemNode));
      return result;
    }
    protected JsonNode? GetResist(JsonNode node){
      JsonObject result = new JsonObject();
      result.AddData("基礎",node.FetchPath("components/_items/0/HitGradeResist"));
      result.AddData("衝撃減衰",node.FetchPath("hurtProtectData/stableValuePerHit"));
      result.AddData("ノックバック",node.FetchPath("components/_items/0/HitBackResist"));
      return result;
    }
    protected JsonNode? GetSpeed(JsonNode itemNode){
      JsonObject result = new JsonObject();
      result.AddData("通常",itemNode.FetchPath("MoveSpeed"));
      result.AddData("ダッシュ",itemNode.FetchPath("RunSpeed"));
      result.AddData("後退",itemNode.FetchPath("MoveBackSpeed"));
      return result;
    }
  }
}
