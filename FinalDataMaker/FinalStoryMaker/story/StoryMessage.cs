
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using FinalHogen.json;

namespace FinalHogen.story{
  public class StoryMessage : FinalJsonNode{
    public StoryMessage(Dictionary<string,string> fiileNames)
    :base("StoryMessage",fiileNames){}
    protected override void SetAllJsonData(JsonNode allNode){
      JsonValue jsonString = allNode.AsValue();
      JsonObject? messages = JsonObject.Parse((string)jsonString!) as JsonObject;
      if(messages==null)throw new Exception();
      foreach(KeyValuePair<string,JsonNode?> keyval in messages){
        if(keyval.Value==null)continue;
        jsonData[keyval.Key] = keyval.Value;
      }
    }
    public string? GetMessage(string id){
      JsonNode? node = Get(id);
      if(node==null)return null;
      string str = node.ToString();
      str = ReplaceColor(str);
      str = ReplaceSize(str);
      str = str.Replace("\"","”").Replace("&","＆").Replace("<","＜").Replace(">","＞");
      return str;
    }
    private string ReplaceColor(string str){
      MatchCollection maches = Regex.Matches(str,"<color.*?>");
      foreach(Match match in maches){
        string v = match.Value.Substring(match.Value.IndexOf("#"));
        v = v.Substring(0,v.IndexOf(">"));
        string color = v.Substring(0,6);
        string type = "";
        if(v.Length>6)type = v.Substring(6);
        string span = MakeColorSpanTag(color,type);
        str = str.Replace(match.Value,span);
      }
      str = Regex.Replace( str, "</color.*?>", "</span>" );
      return str;
    }
    private string ReplaceSize(string str){
      MatchCollection maches = Regex.Matches(str,"<size.*?>");
      foreach(Match match in maches){
        string v = match.Value.Substring(match.Value.IndexOf("="));
        string size = v.Substring(0,v.IndexOf(">"));
        string span = MakeSizeSpanTag(size);
        str = str.Replace(match.Value,span);
      }
      str = Regex.Replace( str, "</size.*?>", "</span>" );
      return str;
    }
    private string MakeColorSpanTag(string color, string type){
      string classAttribute="";
      switch(type){
        case "01":
        classAttribute = "class='miniyure-y miniyure-x' ";
        break;
        case "02":
        classAttribute = "class='yure' ";
        break;
      }
      string colorAttribute = "style='color:#"+color+";'";
      return "<span "+classAttribute+colorAttribute+">";
    }
    private string MakeSizeSpanTag(string size){
      return "<span style='font-size:"+size+"px;'>";
    }
  }
}
