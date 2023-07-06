namespace SqlVersioner.Abstractions.Database
{
  using System;
  using System.Text;

  /// <summary>
  /// SQL object.
  /// </summary>
  /// <remarks>
  ///  <para>This class is used to represent a SQL object.</para>
  /// </remarks>
  public record SqlObject(string Schema, string ObjectName, string Type, string Definition)
  {
    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.AppendLine("/*");
      sb.AppendLine($"Schema: {Schema}");
      sb.AppendLine($"Type: {Type}");
      sb.AppendLine($"ObjectName: {ObjectName}");
      sb.AppendLine($"Execution: {DateTime.UtcNow.ToString("f")}");
      sb.AppendLine("*/");
      sb.AppendLine(Definition);
      return sb.ToString();
    }
  }
}