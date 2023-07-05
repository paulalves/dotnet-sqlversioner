namespace SqlVersioner.Abstractions.Arguments
{
  using System;
  using System.Runtime.Serialization;

  [Serializable]
  public class CliArgumentException : Exception, ISerializable
  {
    public CliArgumentException()
    {
    }

    public CliArgumentException(string message) : base(message)
    {
    }

    public CliArgumentException(string message, Exception inner) : base(message, inner)
    {
    }
  }
}