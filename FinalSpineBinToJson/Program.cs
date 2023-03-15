
using FinalHogen.json;
using FinalHogen.spine;

class ConsoleRun
{
  static void Main(string[] args)
  {
    exec(args);
    //testv36x(); //テストコード
  }
  /// <summary>
  ///  実行コード
  /// </summary>
  static public void exec(string[] args){
    if(args.Length<2){
      Console.WriteLine("使い方 : FinalSpineBinToJson.exe 読み込みskelファイルパス 出力jsonファイルパス");
      return;
    }
    string skel = args[0];
    string output = args[1];
    Type? execType = getExecVersion(skel);
    if(execType==null)return;
    
    using FileStream f = File.Open(skel,FileMode.Open);
    BinToJson read = (BinToJson)execType.GetConstructor(Type.EmptyTypes)!.Invoke(null);
    System.Text.Json.Nodes.JsonNode node = read.convertAll(f);
    JsonFile.save( node, output, false);
    Console.WriteLine("出力:"+output);
  }
  static public Type? getExecVersion(string skel){
    if(!File.Exists(skel)){
      Console.WriteLine("ファイルが見つかりません:"+skel);
      return null;
    }
    string? version = BinToJson.GetFileVersion(skel);
    if(version==null){
      Console.WriteLine("skelファイルではありません？:"+skel);
      return null;
    }
    string[] splitver = version.Split(".");
    if(splitver.Length<2){
      Console.WriteLine("ファイルバージョンが不明です。:"+version);
      return null;
    }
    string checkver = splitver[0]+"."+splitver[1];
    switch(checkver){
      case "3.6":
      return typeof(BinToJsonv36x);
    }
    Console.WriteLine("非対応バージョンです。:"+version);
    return null;
  }
  static void testv36x(){
    // 一部 colorや curve:stepped が設定されてなくて違いが検出されるが、問題ないようだ？ 
    List<string> checkList = new List<string>(){
      "examples/alien/export/alien-pro",
      "examples/coin/export/coin-pro",
      "examples/dragon/export/dragon-ess",
      "examples/goblins/export/goblins-pro",
      "examples/hero/export/hero-pro",
      "examples/powerup/export/powerup-pro",
      "examples/raptor/export/raptor-pro",
      "examples/speedy/export/speedy-ess",
      "examples/spineboy/export/spineboy-pro",
      "examples/spinosaurus/export/spinosaurus-ess",
      "examples/stretchyman/export/stretchyman-pro",  // JSONデータのscaleX,Yが変？
      "examples/tank/export/tank-pro",
      "examples/vine/export/vine-pro"
    };
    foreach(string check in checkList){
      test(check,typeof(BinToJsonv36x));
    }
    
  }
  static void test(string exsamplePath,Type testType){
    string skelpath = exsamplePath+".skel";
    string jsonpath = exsamplePath+".json";
    string outputpath = exsamplePath+".test.json";
    using FileStream f = File.Open(skelpath,FileMode.Open);
    BinToJson read = (BinToJson)testType.GetConstructor(Type.EmptyTypes)!.Invoke(null);
    System.Text.Json.Nodes.JsonNode node = read.convertAll(f);
    JsonFile.save( node, outputpath, true);
    System.Text.Json.Nodes.JsonNode? trueNode = JsonFile.load(jsonpath);
    JsonCompare compare = new JsonCompare(trueNode!,node,2);
    compare.Compare();
    Console.WriteLine("check start. "+exsamplePath);
    compare.ToConsole();
  }
}
