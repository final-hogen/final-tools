
using System.Text.Json.Nodes;
using FinalHogen.json;

namespace FinalHogen.story{
  /// <summary>
  /// ストーリーデータをストーリ読み機用に変換。完全ではないので後で要調整。
  /// </summary>
  public class StroyScene{
    public static JsonNode? Convert(StoryAllStorys common, JsonNode? node){
      if(node==null)return null;
      StroyScene scene = new StroyScene(common,node);
      return scene.Convert();
    }
    protected StoryAllStorys common;
    int nextDialogID = 0;
    Dictionary<int,string> dialogNameMap = new Dictionary<int, string>();
    Dictionary<int,int> selectListMap = new Dictionary<int, int>();
    JsonNode baseNode;
    public StroyScene(StoryAllStorys commonData, JsonNode node){
      common = commonData;
      baseNode = node;
      int nextDialogID = (int)node.FetchPath("nextDialogID")!;
    }
    public JsonNode? Convert(){
      JsonArray result = new JsonArray();
      do{
        JsonArray? node = Convert(nextDialogID);
        if(node!=null)result.Concat(node);
      }while(nextDialogID>0);
      return result;
    }
    protected JsonArray? Convert(int dialogID){
      JsonArray dataList = baseNode.FetchPath("dataList/_items")!.AsArray();
      if(dataList.Count<=0)return null;
      return Convert(dataList[dialogID]);
    }
    protected JsonArray? Convert(JsonNode? dialogNode){
      if(dialogNode==null){
        nextDialogID = -1;
        return null;
      }
      SetNext(dialogNode);
      string? className = dialogNode.FetchString("@@ClassName");
      switch(className){
        case "DialogSpecial":
        case "DialogFullScreen":
        return DialogNarration(dialogNode);
        case "DialogRoleMessageAction":
        return DialogMessage(dialogNode);
        case "DialogOption":
        return DialogOption(dialogNode);
        case "DialogLeaveDialogRole":
        return DialogLeaveDialogRole(dialogNode);
        case "DialogCreateDialogRole":
        case "DialogSetBackground":
        return DialogSetBackground(dialogNode);
        case "DialogCameraAction":
        return DialogCameraAction(dialogNode);

        //case "DialogPlotTheater":
        //case "DialogSoundAction":
      }
      return null;
    }
    protected JsonArray? DialogMessage(JsonNode node){
      return MakeMessage(node,null);
    }
    protected JsonArray? DialogNarration(JsonNode node){
      return MakeMessage(node,"narration");
    }
    protected JsonArray? DialogLeaveDialogRole(JsonNode node){
      JsonArray result = new JsonArray();
      JsonArray roleIds = node.FetchPath("roleid")!.AsArray();
      foreach(JsonNode? idNode in roleIds){
        if(idNode==null)continue;
        int id = (int)idNode;
        string? name=null;
        if(!dialogNameMap.TryGetValue(id,out name))continue;
        JsonObject msgObject = new JsonObject();
        msgObject.SetCommand("message");
        msgObject.Add("id",name);
        msgObject.Add("message","");
        msgObject.Add("stop",0);
        result.Add(msgObject);
      }
      return result;
    }
    protected JsonArray? DialogOption(JsonNode node){
      JsonArray result = new JsonArray();
      JsonArray? msgs = node.FetchPath("option") as JsonArray;
      foreach(JsonNode? msg in msgs!){
        if(msg==null)continue;
        string? message = common.message.GetMessage(msg.ToString());
        JsonObject msgObject = new JsonObject();
        msgObject.SetCommand("select");
        msgObject.AddContent("message",message);
        result.Add(msgObject);
      }
      return result;
    }
    protected JsonArray? MakeMessage(JsonNode node,string? command){
      string? name = GetName(node);
      JsonNode? dialogIdNode = node.FetchPath("dialogID");
      int dialogId = (dialogIdNode!=null)?((int)dialogIdNode):(0);
      if(dialogId!=0){
        if(String.IsNullOrEmpty(name))name = dialogNameMap[dialogId];
        else dialogNameMap[dialogId] = name;
      }
      JsonArray result = new JsonArray();
      JsonArray? msgs = node.FetchPath("msg") as JsonArray;
      foreach(JsonNode? msg in msgs!){
        if(msg==null)continue;
        string? message = common.message.GetMessage(msg.ToString());
        JsonObject msgObject = new JsonObject();
        if(!String.IsNullOrEmpty(command))msgObject.SetCommand(command);
        if(!String.IsNullOrEmpty(name))msgObject.AddContent("name",name);
        msgObject.AddContent("message",message);
        result.Add(msgObject);
      }
      return result;
    }

    protected JsonArray? DialogSetBackground(JsonNode node){
      int imageID = (int)node.FetchPath("BackgroundID")!;
      if(imageID==0)return null;
      JsonObject result = new JsonObject();
      result.SetScene("image"+imageID);
      return new JsonArray(result);
    }
    protected JsonArray? DialogCameraAction(JsonNode node){
      int shake = (int)node.FetchPath("shake")!;
      if(shake<=0)return null;
      JsonObject result = new JsonObject();
      result.SetShake();
      SetNext(node);
      return new JsonArray(result);
    }
    protected void SetNext(JsonNode node){
      JsonArray nextList = node.FetchPath("nextNodeList")!.AsArray();
      if(nextList.Count<=0)nextDialogID = 0;
      else if(nextList.Count<=1)nextDialogID = (int)nextList[0]!;
      else{
        // 複数ある時は上から選択
        int nowSelectIndex = 0;
        if(selectListMap.ContainsKey(nextDialogID)){
          nowSelectIndex = selectListMap[nextDialogID]+1;
        }
        selectListMap[nextDialogID] = nowSelectIndex;
        if(nowSelectIndex>=nextList.Count)nextDialogID = 0;
        else nextDialogID = (int)nextList[nowSelectIndex]!;
      }
    }
    protected string? GetName(JsonNode node){
      string? nameNode = node.FetchString("dialogName");
      if(!String.IsNullOrEmpty(nameNode)){
        return common.lang.GetLang(nameNode);
      }
      string? id = node.FetchString("dialogID");
      if(id==null)return null;
      return common.pilotNames.Get(id);
    }
  }
  static class Extension{
    public static string? FetchString(this JsonNode? self, string path){
      if(self==null)return null;
      JsonNode? node = self.FetchPath(path);
      if(node==null)return null;
      return node.ToString();
    }
    public static void SetScene(this JsonObject self, string imageName){
      self.SetCommand("scene");
      self.SetImage(imageName);
      self.SetStop(false);
    }
    public static void SetShake(this JsonObject self){
      self.SetCommand("shake");
      self.SetStop(false);
    }
    public static void SetStop(this JsonObject self, bool isStop){
      if(isStop)return; // defualt 1
      self.Add("stop",0);
    }
    public static void SetImage(this JsonObject self, string imageName){
      self.AddContent("image",imageName);
    }
    public static void SetCommand(this JsonObject self, string command){
      self.AddContent("command",command);
    }
  }
}
