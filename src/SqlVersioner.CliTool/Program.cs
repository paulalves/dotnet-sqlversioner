namespace SqlVersioner.CliTool
{
  using SqlVersioner.Abstractions.Arguments;

  public static class Program
  {
    public static int Main(string[] args)
    {
      CliArguments.Parse(new []
      {
        "--user", "sa",
        "--password", "Password123",
        "--database", "SqlVersioner",
        "--server", "localhost",
        "--output", "~/SqlVersioner",
        "--verbosity", "2"
      });
      
      return 0; 
    }
  }
}