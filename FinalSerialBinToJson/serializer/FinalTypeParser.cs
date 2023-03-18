

namespace FinalHogen.serialize
{
  /// <summary>
  /// Type.FullNameをパースするクラス
  /// 結果は FinalTypeParser.ClassInfoで出力。
  /// </summary>
  public class FinalTypeParser{
    public char startToken='[';
    public char endToken=']';
    public char splitToken=',';
    public static ClassInfo? parse(string typeString){
      FinalTypeParser parser = new FinalTypeParser();
      return parser.parseType(typeString);
    }
    public ClassInfo? parseType(string typeString){
      List<object> result = parseNest(typeString);
      ClassInfo info = new ClassInfo(result);
      return info;
    }
    protected List<object> parseNest(string str){
      int strat = str.IndexOf(startToken);
      if(strat<0)return new List<object>(){str};
      
      int end = getEnd(strat,str);
      if(end<0)throw new InvalidDataException("parse end pos not found. "+str);
      List<object> result = new List<object>();
      if(strat>0)result.Add(str.Substring(0,strat));
      string sub = str.Substring(strat+1,end-strat-1);
      result.Add(parseNest(sub));
      if(end<str.Length-2)result.AddRange(parseNest(str.Substring(end+1)));
      return result;
    }
    protected int getEnd(int startpos ,string str){
      int indent = 0;
      for(int i=startpos; i<str.Length; ++i){
        char c = str[i];
        if(c==startToken)++indent;
        else if(c==endToken){
          if(--indent==0)return i;
        }
      }
      return -1;
    }
    /// <summary>
    /// パース結果出力情報
    /// </summary>
    public class ClassInfo{
      public string className = "";
      public List<ClassInfo> generics = new List<ClassInfo>();
      public bool isGeneric{get{return generics.Count>0;}}
      internal ClassInfo(){}
      internal ClassInfo(string str){
        className = str;
      }
      internal ClassInfo(List<object> src){
        if(src.Count<=0)throw new ArgumentException("sourcedata invailed.");
        string name = (src[0] as string)!;

        int index = name.IndexOfAny("`,".ToCharArray());
        if(index<0)className=name;
        else className = name.Substring(0,index);
        if(src.Count<=1)return;
        addGenerics((src[1] as List<object>)!);
      }
      protected  void addGenerics(string str){
        if(str==",")return;
        addGenerics(new ClassInfo(str));
      }
      protected  void addGenerics(ClassInfo info){
        generics.Add(info);
      }
      protected  void addGenerics(List<object> src){
        for(int i=0;i<src.Count;++i){
          switch(src[i]){
            case string str:
            addGenerics(str);
            break;
            case List<object> list:
            addGenerics(new ClassInfo(list));
            break;
          }
        }
      }
    }
  }
}
