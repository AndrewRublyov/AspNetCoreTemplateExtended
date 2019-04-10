namespace AspNetCoreTemplateExtended.Config.Options
{
  public class AuthOptions
  {
    public string Key { get; set; }
    public string Issuer { get; set; }
    public int ExpireDays { get; set; }
  }
}