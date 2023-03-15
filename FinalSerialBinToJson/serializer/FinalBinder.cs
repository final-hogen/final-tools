
namespace FinalHogen.serialize
{
  class FinalBinder : AnonymousBinder{
    public override Type BindToType(string assemblyName, string typeName)
    {
      if(String.IsNullOrEmpty(firstConvertClassName))firstConvertClassName = typeName;
      typeName = "unknown";
      return base.BindToType(assemblyName,typeName);
    }
  }
}
