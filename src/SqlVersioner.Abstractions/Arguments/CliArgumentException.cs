namespace SqlVersioner.Abstractions.Arguments
{
  using System;
  using System.Runtime.Serialization;

  /// <summary>
  /// CliArgumentException class.
  /// </summary>
  /// <remarks>
  /// <para>This class is used to represent an exception thrown by the CLI.</para>
  /// </remarks>
  [Serializable]
  public class CliArgumentException : Exception, ISerializable
  {
    /// <summary>
    /// Default constructor.
    /// </summary>
    public CliArgumentException()
    {
    }

    /// <summary>
    /// Constructor with arguments.
    /// </summary>
    /// <param name="message">The message.</param>
    public CliArgumentException(string message) : base(message)
    {
    }

    /// <summary>
    /// Constructor with arguments.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="inner">The inner exception.</param>
    public CliArgumentException(string message, Exception inner) : base(message, inner)
    {
    }
  }
}