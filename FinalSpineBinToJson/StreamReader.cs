
namespace FinalHogen.spine
{
  public class StreamReader
  {
    protected Stream stream{get;}
    public StreamReader(Stream source){
      stream = source;
    }
    public value read(){return new value(this);}
    protected int ReadByte(){
      return stream.ReadByte();
    }
    protected byte[] ReadBuffer(int length,int offset=0){
      byte[] buf = new byte[length+offset];
      stream.Read(buf,offset,length);
      return buf;
    }
    public variant ReadVarint(){
      return new variant(stream);
    }
    public variantNoOptimize ReadVariantNoOptimize(){
      return new variantNoOptimize(stream);
    }
    public string? ReadRGBString(){
      rgb? color = read();
      if(color==null)return null;
      return color.ToSpineString();
    }
    public string ReadRGBAString(){
      rgba color = read();
      return color.ToSpineString();
    }
    public T read<T>() where T : System.Enum{
      int num = ReadVarint();
      return (T)Enum.ToObject(typeof(T), num);
    }
    public class rgb{
      protected int readValue;
      protected float[] c = new float[4];
      public rgb(StreamReader source,int readCount=3){
        readValue = source.read();
        c = new float[readCount];
        int count = Math.Min(readCount,c.Length);
        if(count>=4)c[3] = 1.0f;
        int filter = 0x000000ff;
        for( int i=0; i<count; ++i){
          int shiftCount = (count-i)*8;
          int colorInt = readValue>>shiftCount;
          c[i] = (colorInt&filter)/255f;
        }
      }
      public bool invalid{get{return (readValue==-1);}}
      public float r{get{return c[0];}}
      public float g{get{return c[1];}}
      public float b{get{return c[2];}}
      public virtual string ToSpineString(){
        return String.Format("{0:x6}",readValue&0xffffff);
      }
    }
    public class rgba : rgb{

      public rgba(StreamReader source):base(source,4){
      }
      public float a{get{return c[3];}}
      public override string ToSpineString(){
        return String.Format("{0:x8}",readValue);
      }
    }
    public class value
    {
      protected StreamReader stream;
      public value(StreamReader source){
        stream = source;
      }
      // 以下はSkeletonBinaryのぱくり
      public static implicit operator float(value v){
        byte[] buffer = new byte[4];
        buffer[3] = (byte)v.stream.ReadByte();
        buffer[2] = (byte)v.stream.ReadByte();
        buffer[1] = (byte)v.stream.ReadByte();
        buffer[0] = (byte)v.stream.ReadByte();
        return BitConverter.ToSingle(buffer, 0);
      }
      public static implicit operator int(value v){
			return (v.stream.ReadByte() << 24)
        + (v.stream.ReadByte() << 16)
        + (v.stream.ReadByte() << 8)
        + v.stream.ReadByte();
      }
      public static implicit operator byte(value v){
			return (byte)v.stream.ReadByte();
      }
      public static implicit operator sbyte(value v){
			return (sbyte)(byte)v;
      }
      public static implicit operator bool(value v){
			return v.stream.ReadByte()==1;  // 255(-1)がfalseとして扱われている
      }
      public static implicit operator string?(value v){
			int byteCount = v.stream.ReadVarint();
			switch (byteCount) {
			case 0:
				return null;
			case 1:
				return "";
			}
			byteCount--;
			byte[] buffer = v.stream.ReadBuffer(byteCount);
			return System.Text.Encoding.UTF8.GetString(buffer, 0, byteCount);
      }
      public static implicit operator rgba(value v){
        return new rgba(v.stream);
      }
      public static implicit operator rgb?(value v){
        rgb c = new rgb(v.stream);
        if(c.invalid)return null;
        return c;
      }
    }
    public class variant{
      protected int r;
      public variant(Stream source){
        int b = source.ReadByte();
        int result = b & 0x7F;
        if ((b & 0x80) != 0) {
          b = source.ReadByte();
          result |= (b & 0x7F) << 7;
          if ((b & 0x80) != 0) {
            b = source.ReadByte();
            result |= (b & 0x7F) << 14;
            if ((b & 0x80) != 0) {
              b = source.ReadByte();
              result |= (b & 0x7F) << 21;
              if ((b & 0x80) != 0) result |= (source.ReadByte() & 0x7F) << 28;
            }
          }
        }
        r = result;
      }
      virtual protected int getInt(){return r;}
      public static implicit operator int(variant v){
			  return v.getInt();
      }
    }
    public class variantNoOptimize : variant{
      public variantNoOptimize(Stream source):base(source){}
      override protected int getInt(){
        return  ((r >> 1) ^ -(r & 1));
      }
    }
  }
}