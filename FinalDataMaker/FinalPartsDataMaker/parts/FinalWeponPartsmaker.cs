
using System.Text.Json.Nodes;
using FinalHogen.json;

/// <summary>
/// 結果を謎微調整するクラス
/// </summary>
namespace FinalHogen.parts
{
  public class FinalWeponPartsmaker : WeponPartsmaker{
    public FinalWeponPartsmaker(CommonPartsData com,Dictionary<string,string> fiileNames)
    :base(com,fiileNames){
    }
    protected override JsonNode? CalcResult(string widgetId, string SuitId, JsonNode? result)
    {
      if(widgetId=="2300113")return Calcイブリン武器Result(result);
      if(SuitId=="0")return CalcCommonWeponResult(result);
      return base.CalcResult(widgetId,SuitId,result);
    }
    private JsonNode? Calcイブリン武器Result(JsonNode? result){
      return CalcCommonWeponResult(result);
    }
    private JsonNode? CalcCommonWeponResult(JsonNode? result){
      JsonObject? seinou = result!.AsObject()["基本性能"] as JsonObject;
      JsonArray? attackRateList = seinou!["攻撃倍率"] as JsonArray;
      if(attackRateList==null)return result;
      float power = (float)seinou!["武器倍率"]!;
      seinou["武器倍率"] = (float)Math.Round(power/attackRateList.Count,2);
      return result;
    }
  }
}
