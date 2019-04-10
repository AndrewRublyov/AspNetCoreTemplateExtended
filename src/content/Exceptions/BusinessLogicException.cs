using System;

namespace AspNetCoreTemplateExtended.Exceptions
{
  public class BusinessLogicException : Exception
  {
    public BusinessLogicException(string message) : base(message)
    {
      
    }
  }
}