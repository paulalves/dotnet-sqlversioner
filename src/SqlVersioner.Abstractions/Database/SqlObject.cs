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
    /// <summary>
    /// Gets the definition of the SQL object.
    /// </summary>
    /// <param name="includeHeader">Include header.</param>
    /// <returns>The definition of the SQL object.</returns>
    /// <remarks>
    /// <para>This method is used to get the definition of the SQL object.</para>
    /// </remarks>
    public string GetSqlDefinition(bool includeHeader = true)
    {
      var sb = new StringBuilder();
      if (includeHeader)
      {
        sb.AppendLine("/*");
        sb.AppendLine($"Schema: {Schema}");
        sb.AppendLine($"Type: {Type}");
        sb.AppendLine($"ObjectName: {ObjectName}");
        sb.AppendLine($"Execution: {DateTime.UtcNow.ToString("f")}");
        sb.AppendLine("*/");
      }
      sb.AppendLine(Definition.ReplaceLineEndings(Environment.NewLine));
      return sb.ToString();
    }
    
    /// <inheritdoc/>
    public override string ToString()
    {
      return GetSqlDefinition(includeHeader: false);
    }
  }
}