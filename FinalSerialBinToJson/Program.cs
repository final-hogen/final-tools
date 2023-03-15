
namespace FinalHogen
{
  class Program
  {
    static void Main(string[] args)
    {
      foreach (string path in args)
      {
        convert(path);
      }
    }
    static void convert(string path)
    {
      if (!File.Exists(path))
      {
        Console.WriteLine("file not found. " + path);
        return;
      }
      FinalHogen.serialize.SerialBinToJson.ConvertData(path);
    }
  }
}
