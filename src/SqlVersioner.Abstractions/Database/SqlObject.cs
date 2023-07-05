namespace SqlVersioner.Abstractions.Database
{
  using System;
  using System.Text;

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