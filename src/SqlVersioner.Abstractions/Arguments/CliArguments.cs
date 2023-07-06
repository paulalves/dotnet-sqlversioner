namespace SqlVersioner.Abstractions.Arguments
{
  using System;

  /// <summary>
  /// Command line arguments
  /// </summary>
  public class CliArguments
  {
#pragma warning disable CS8618
    private CliArguments()
#pragma warning restore CS8618
    {
      Verbosity = 1;
    }

    /// <summary>The user to connect to the database </summary>
    public string User { get; private set; }
    
    /// <summary>The password to connect to the database</summary>
    public string Password { get; private set; }
    
    /// <summary>The database to connect to</summary>
    public string Database { get; private set; }
    
    /// <summary>The server to connect to</summary>
    public string Server { get; private set; }
    
    /// <summary>The output directory</summary>
    public string Output { get; private set; }
    
    /// <summary>The verbosity level</summary>
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
    
    /// <summary>
    /// Parses the command line arguments
    /// </summary>
    /// <param name="args">Program arguments</param>
    /// <returns>The parsed arguments</returns>
    /// <exception cref="CliArgumentException">Thrown when the arguments are invalid</exception>
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
          case "--help":
            throw new CliArgumentException("GetHelp was requested.");
          default: 
            throw new CliArgumentException($"Unknown argument: {arg}");
        }
      }
      return arguments.Validate();
    }

    /// <summary>
    /// Gets the usage message
    /// </summary>
    /// <returns>The usage message</returns>
    public static string GetUsage()
    {
      return $@"
sqlversioner --user <user> --password <password> --database <database> --server <server> --output <output> [--verbosity <verbosity>]

Options:

--user      <{nameof(User)}>..............The user to connect to the database
--password  <{nameof(Password)}>..........The password to connect to the database
--database  <{nameof(Database)}>..........The database to connect to
--server    <{nameof(Server)}>............The server to connect to
--output    <{nameof(Output)}>............The output directory
--verbosity <{nameof(Verbosity)}>.........The verbosity level. Allowed values are: 0 - Quiet, 1 - Minimal, 2 - Normal, 3 - Detailed. Default is 1.
--help..........................Shows this help message";
    }
  }
}