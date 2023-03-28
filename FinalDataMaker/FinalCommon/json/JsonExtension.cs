
using System.Text.Json.Nodes;

namespace FinalHogen.json
{
  /// <summary>
  /// JsonNode に色々機能を追加する
  /// </summary>
  public static class JsonExtension{
    /// <summary>
    /// null や 空 でなければコンテンツを追加する。すでに親もちの場合はCloneする。
    /// </summary>
    public static bool AddContent(this JsonObject node, string key, JsonNode? item){
      if(item==null)return false;
      if(item.Parent!=null)item = item.Clone();
      if(item is JsonArray && item.AsArray().Count<=0)return false;
      if(item is JsonObject && item.AsObject().Count<=0)return false;
      
      if(item.Parent!=null)node.Add(key,item.Clone());
      else node.Add(key,item);
      return true;
    }
    /// <summary>
    /// 親なしでノードを複製する
    /// </summary>
    public static JsonNode Clone(this JsonNode node){
      return JsonNode.Parse(node.ToJsonString())??throw new InvalidDataException(node.ToJsonString());
    }
    /// <summary>
    /// 子ノードを取得する
    /// </summary>
    public static JsonNode?[] GetChilds(this JsonNode node){
      switch(node){
        case JsonObject obj:
        {
          JsonNode?[] result = new JsonNode[obj.Count];
          IEnumerator<KeyValuePair<String,JsonNode?>> itr = obj.GetEnumerator();
          int i=0;
          while(itr.MoveNext()){
            result[i++] = itr.Current.Value;
          }
          return result;
        }
        case JsonArray array:
        {
          JsonNode?[] result = new JsonNode[array.Count];
          for(int i=0;i<array.Count;++i){
            result[i] = array[i];
          }
          return result;
        }
      }
      return new JsonNode[0];
    }
    /// <summary>
    /// 根元からのフルパスを取得する(スラッシュ区切り文字)
    /// </summary>
    public static string GetFullpathString(this JsonNode node){
      string[] result = GetFullpath(node);
      return String.Join("/",result);
    }
    /// <summary>
    /// 根元からのフルパスを取得する
    /// </summary>
    public static string[] GetFullpath(this JsonNode node){
      string jsonPath = node.GetPath();
      jsonPath = jsonPath.Substring(2);
      jsonPath = jsonPath.Replace("]","");
      jsonPath = jsonPath.Replace('[','.');
      return jsonPath.Split(".");
    }
    /// <summary>
    /// 指定されたパスと一致するか調べるワイルドカード(*)、空要素(後方一致)使用可(スラッシュ区切り)
    /// </summary>
    public static bool MatchFullpath(this JsonNode node, string fullpath){
      string[] split = fullpath.Split("/");
      return MatchFullpath(node,split);
    }
    /// <summary>
    /// 指定されたパスと一致するか調べるワイルドカード(*)、空要素(後方一致)使用可
    /// </summary>
    public static bool MatchFullpath(this JsonNode node, string[] fullpath){
      string[] myfullpath = node.GetFullpath();
      return MatchFullpath(myfullpath,fullpath);
    }
    public static bool MatchFullpath(string[] myfullpath, string[] fullpath){
      if(fullpath.Length<=0)return false;
      int i,j;
      // clip last empty.
      if(fullpath[fullpath.Length-1]==""){
        for( i=0; i< fullpath.Length; ++i){
          if(fullpath[fullpath.Length-1-i]!="")break;
        }
        if(i>=fullpath.Length)return false;
        fullpath = fullpath[0..(fullpath.Length-i)];
      }
      // check.
      for(i=0,j=0; i<myfullpath.Length&&j<fullpath.Length; ++i,++j){
        if(fullpath[j]=="*")continue;
        if(fullpath[j]==""){
          if(fullpath[j+1]==myfullpath[i])j+=1;
          else j-=1;
          continue;
        }
        if(fullpath[j]!=myfullpath[i])return false;
      }
      return i>=myfullpath.Length&&j>=fullpath.Length;
    }
    /// <summary>
    ///   指定されたパスのデータを探す(スラッシュ区切り)
    /// </summary>
    public static JsonNode? FetchPath(this JsonNode node, string path){
      string[] split = path.Split("/");
      return node.FetchPath(split);
    }
    /// <summary>
    ///   指定されたパスのデータを探す
    /// </summary>
    public static JsonNode? FetchPath(this JsonNode node, string[] jsonPath){
      for(int i=0;i<jsonPath.Length;++i)
      {
        JsonNode? check;
        if(node.GetType()==typeof(JsonObject))
        {
          check = node[jsonPath[i]];
        }else if(node.GetType()==typeof(JsonArray))
        {
          check = node[int.Parse(jsonPath[i])];
        }else{
          return null;
        }
        if(check==null)return null;
        node = check;
      }
      return node;
    }
    /// <summary>
    /// ワイルドカード(*)指定可能なfetchPath(スラッシュ区切り)。
    /// </summary>
    public static JsonNode?[] SearchPath(this JsonNode node, string jsonPath){
      string[] split = jsonPath.Split("/");
      return SearchPath(node,split);
    }
    /// <summary>
    /// ワイルドカード(*)指定可能なfetchPath。
    /// </summary>
    public static JsonNode?[] SearchPath(this JsonNode node, string[] jsonPath){
      return searchPath(node,jsonPath,0);
    }
    private static JsonNode?[] searchPath(this JsonNode node, string[] jsonPath, int position){
      if(position>=jsonPath.Length)return node.GetChilds();
      string search = jsonPath[position];
      if(search!="*"){
        JsonNode? child = node.FetchPath(search);
        if(child==null)return new JsonNode?[0];
        return child.searchPath(jsonPath,position+1);
      }
      List<JsonNode?> result = new List<JsonNode?>();
      JsonNode?[] childs = node.GetChilds();
      foreach( JsonNode? data in childs ){
        if(data==null)continue;
        result.AddRange(data.searchPath(jsonPath,position+1));
      }
      return result.ToArray();
    }
    /// <summary>
    /// JsonObjectのキーだけ取り出す
    /// </summary>
    public static List<string> GetKeys(this JsonObject? self){
      List<string> result = new List<string>();
      if(self==null)return result;
      foreach(KeyValuePair<string,JsonNode?> keyval in self){
        result.Add(keyval.Key);
      }
      return result;
    }
    /// <summary>
    ///  自身に追加のデータを混ぜる。
    /// </summary>
    public static JsonObject Merge(this JsonObject baseObject, JsonNode? addObject){
      if(addObject==null)return baseObject;
      JsonObject add = addObject.AsObject();

      IEnumerator<KeyValuePair<String,JsonNode?>> it = add.GetEnumerator();
      while(it.MoveNext()){
        KeyValuePair<String,JsonNode?> current = it.Current;
        if(current.Value==null)continue;
        if(baseObject.ContainsKey(current.Key)){
          if(baseObject.GetType()==typeof(JsonObject)){
            JsonNode? target = baseObject[current.Key];
            if(target!=null)target.AsObject().Merge(current.Value);
          }else if(baseObject.GetType()==typeof(JsonArray)){
            JsonArray array = baseObject.AsArray().Merge(addObject);
          }else baseObject[current.Key] = current.Value.Clone();
        }else baseObject[current.Key] = current.Value.Clone();
      }
      return baseObject;
    }
    public static JsonArray Merge(this JsonArray baseObject, JsonNode? addObject){
      if(addObject==null)return baseObject;
      if(!(addObject is JsonArray))return baseObject;
      JsonArray array = baseObject.AsArray();
      foreach(JsonNode? node in addObject.AsArray()){
        if(node!=null)array.Add(node.Clone());
      }
      return baseObject;
    }
    public static JsonNode Merge(this JsonNode baseObject, JsonNode? addObject){
      if(baseObject is JsonArray)return baseObject.AsArray().Merge(addObject);
      if(baseObject is JsonObject)return baseObject.AsObject().Merge(addObject);
      return baseObject;
    }
    /// <summary>
    /// データを取り出して消去
    /// </summary>
    public static JsonNode? Pop(this JsonObject? self, string key){
      if(self==null)return null;
      JsonNode? getValue = null;
      self.TryGetPropertyValue(key,out getValue);
      self.Remove(key);
      return getValue;
    }
    /// <summary>
    /// 同じ内容か判定
    /// </summary>
    public static bool DeepEquals(this JsonNode? self, JsonNode? other)
    {
        if (self is JsonArray selfArray && other is JsonArray otherArray)
        {
            return (selfArray.Count == otherArray.Count) &&
                selfArray.Zip(otherArray).All(p => DeepEquals(p.First, p.Second));
        }
        if (self is JsonObject selfObject && other is JsonObject otherObject)
        {
            if (selfObject.Count != otherObject.Count)
                return false;
            foreach (var (key, selfChild) in selfObject)
            {
                if (!otherObject.TryGetPropertyValue(key, out var otherChild))
                    return false;
                if (!DeepEquals(selfChild, otherChild))
                    return false;
            }
            return true;
        }
        if (self is JsonValue selfValue && other is JsonValue otherValue)
        {
            var selfJson = self.ToJsonString();
            var otherJson = other.ToJsonString();
            return selfJson == otherJson;
        }
        if (self is null && other is null)
        {
            return true;
        }

        return false;
    }
  }
  /// <summary>
  /// ファイル読み込み書き込み
  /// </summary>
  public static class JsonFile{
    public static JsonNode? load(string filepath){
      if(!File.Exists(filepath))return null;
      using FileStream file = new FileStream(filepath,FileMode.Open,FileAccess.Read);
      using StreamReader reader = new StreamReader(file);
      string text = reader.ReadToEnd();
      return JsonNode.Parse(text);
    }
    public static bool save(JsonNode? node, string filepath, bool indent=true){
       using FileStream file = File.Create(filepath);
       using StreamWriter writer = new StreamWriter(file);
       string? jsonText = toJsonString(node,indent);
       if(jsonText==null)return false;
       writer.WriteLine(jsonText);
       //Console.WriteLine("Write file done. "+filepath);
       return true;
    }
    /// <summary>
    /// 成形出力
    /// </summary>
    public static string? toJsonString(JsonNode? node,bool indent=true)
    {
      if(node==null)return null;
      var options = new System.Text.Json.JsonSerializerOptions
      {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(
          System.Text.Unicode.UnicodeRanges.All),
        WriteIndented = indent
      };
      return node.ToJsonString(options);
    }
  }
}