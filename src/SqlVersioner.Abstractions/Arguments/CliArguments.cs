namespace SqlVersioner.Abstractions.Arguments
{
  using System;

  public class CliArguments
  {
#pragma warning disable CS8618
    private CliArguments()
#pragma warning restore CS8618
    {
      Verbosity = 1;
    }
    
    public string User { get; private set; }
    public string Password { get; private set; }
    public string Database { get; private set; }
    public string Server { get; private set; }
    public string Output { get; private set; }
    public int Verbosity { get; private set; }
    
    private CliArguments Validate()
    {
      if (string.IsNullOrWhiteSpace(User)) throw new CliArgumentException("The user cannot be null, empty, or whitespace.");
      if (string.IsNullOrWhiteSpace(Password)) throw new CliArgumentException("The password cannot be null, empty, or whitespace.");
      if (string.IsNullOrWhiteSpace(Database)) throw new CliArgumentException("The database cannot be null, empty, or whitespace.");
      if (string.IsNullOrWhiteSpace(Server)) throw new CliArgumentException("The server cannot be null, empty, or whitespace.");
      if (string.IsNullOrWhiteSpace(Output)) throw new CliArgumentException("The output cannot be null, empty, or whitespace.");
      if (Verbosity < 0 || Verbosity > 3 ) throw new CliArgumentException("The verbosity must be between 0 and 3.");
      return this;      
    }
    
    public static CliArguments Parse(string[] args)
    {
      var arguments = new CliArguments();
      for (var index = 0; index < args.Length; index++)
      {
        var arg = args[index];
        switch (arg)
        {
          case "--user":
            arguments.User = args[++index];
            break;
          case "--password":
            arguments.Password = args[++index];
            break;
          case "--database":
            arguments.Database = args[++index];
            break;
          case "--server":
            arguments.Server = args[++index];
            break;
          case "--output":
            arguments.Output = Environment.ExpandEnvironmentVariables(args[++index].Replace("~", "%HOME%"));
            break;
          case "--verbosity":
            arguments.Verbosity = int.Parse(args[++index]);
            break;
        }
      }
      return arguments.Validate();
    }
  }
}