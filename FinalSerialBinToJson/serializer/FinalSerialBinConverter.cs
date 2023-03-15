
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace FinalHogen.serialize
{
  /// <summary>
  /// バイナリシリアライズされた情報をjsonに書き出す
  /// </summary>
  class SerialBinToJson
  {

    public static void ConvertData(string path)
    {
      string dirName = path + ".JsonData/";
      Directory.CreateDirectory(dirName);
      Dictionary<string, object> obj = LoadDataClass(path);
      SaveJsonData(obj.Keys, dirName + path + ".keys.json");

      foreach (KeyValuePair<string, object> item in obj)
      {
        try
        {
          SaveJsonData(item.Value, dirName + item.Key + ".json");
        }
        catch (Exception e)
        {
          Console.WriteLine("Failed to SaveJsonData. " + item.Key + " Reason: " + e.Message);
        }
      }
    }
    protected static Dictionary<string, object> LoadDataClass(string path)
    {
      Dictionary<string, object> list = new Dictionary<string, object>();
      using FileStream file = new FileStream(path,
          FileMode.Open,
          FileAccess.Read);
      Console.WriteLine("Start Pos: " + file.Position);
      while (file.Position < file.Length)
      {
        KeyValuePair<string, object> readObj = ReadFileData(file);
        list.Add(list.Count.ToString("000") + "-" + readObj.Key, readObj.Value);
      }
      Console.WriteLine("End Pos: " + file.Position);
      return list;
    }
    protected static KeyValuePair<string, object> ReadFileData(Stream file)
    {
      long startpos = file.Position;
      BinaryFormatter formatter = new BinaryFormatter();
      AnonymousBinder binder = new AnonymousBinder();
      binder.SetFormatter(formatter);
      object obj;
      //読み込み
      try
      {
        obj = formatter.Deserialize(file);
      }
      catch (SerializationException e)
      {
        ///継承の関係がバイナリファイルから読み取れないので、リストとか使うと代入エラーになることがある。
        Console.WriteLine("LasPos: " + file.Position);
        Console.WriteLine("Failed to LoadDataClass serialize. Reason: " + e.Message);
        file.Position = startpos;
        return ReadFinalFileData(file);
      }
      Console.WriteLine("Pos: " + file.Position);
      return new KeyValuePair<string, object>(binder.firstConvertClassName, obj);
    }
    /// <summary>
    /// 最終手段、クラス名なしで読み込む
    /// </summary>
    protected static KeyValuePair<string, object> ReadFinalFileData(Stream file)
    {
      long startpos = file.Position;
      BinaryFormatter formatter = new BinaryFormatter();
      AnonymousBinder binder = new FinalBinder();
      binder.SetFormatter(formatter);
      object obj;
      //読み込み
      try
      {
        obj = formatter.Deserialize(file);
      }
      catch (SerializationException e)
      {
        Console.WriteLine("LasPos: " + file.Position);
        Console.WriteLine("Failed to LoadDataClass serialize. Reason: " + e.Message);
        throw;
      }
      Console.WriteLine("Pos: " + file.Position);
      return new KeyValuePair<string, object>(binder.firstConvertClassName, obj);
    }
    public static void SaveJsonData(object source, string path)
    {
      using FileStream file = File.Create(path);
      var options = new JsonSerializerOptions
      {
        NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        WriteIndented = true
      };
      JsonSerializer.Serialize(file, source, options);
      Console.WriteLine(path+" done." );
    }
  }
}
