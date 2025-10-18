using System.Xml.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProjektGrupowy.Application.Filters;

public sealed class EnumDescriptionsSchemaFilter : ISchemaFilter
{
    private static readonly Lazy<Dictionary<(string TypeFullName, string Member), string>> XmlSummaries =
        new(LoadSummaries);

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var type = context.Type;
        if (!type.IsEnum) return;

        var names = Enum.GetNames(type);
        var values = Enum.GetValues(type).Cast<object>()
            .Select(Convert.ToInt64).ToArray();

        // read <summary> of each enum field from XML
        var descs = names.Select(n =>
            XmlSummaries.Value.TryGetValue((type.FullName!, n), out var s)
                ? Trim(s)
                : null).ToArray();

        // vendor extensions (some tools use these)
        var xNames = new OpenApiArray();
        xNames.AddRange(names.Select(n => new OpenApiString(n)));
        var xDescs = new OpenApiArray();
        xDescs.AddRange(descs.Select(d => new OpenApiString(d ?? string.Empty)));
        schema.Extensions["x-enumNames"] = xNames;
        schema.Extensions["x-enumDescriptions"] = xDescs;

        // make it visible in Swagger UI by enriching the schema's description
        var sb = new System.Text.StringBuilder(schema.Description ?? string.Empty);
        if (sb.Length > 0) sb.AppendLine().AppendLine();
        sb.AppendLine("Values:");
        for (var i = 0; i < names.Length; i++)
        {
            sb.Append("- ").Append(names[i]).Append(" (").Append(values[i]).Append(')');
            if (!string.IsNullOrWhiteSpace(descs[i])) sb.Append(": ").Append(descs[i]);
            sb.AppendLine();
        }

        schema.Description = sb.ToString();
    }

    private static Dictionary<(string, string), string> LoadSummaries()
    {
        var dict = new Dictionary<(string, string), string>();
        foreach (var file in Directory.EnumerateFiles(AppContext.BaseDirectory, "*.xml"))
        {
            XDocument doc;
            try
            {
                doc = XDocument.Load(file);
            }
            catch
            {
                continue;
            }

            foreach (var m in doc.Descendants("member"))
            {
                // Enum fields look like: F:Namespace.Type.Member
                var nameAttr = m.Attribute("name")?.Value;
                if (nameAttr is null || !nameAttr.StartsWith("F:")) continue;

                var full = nameAttr.Substring(2); // Namespace.Type.Member
                var lastDot = full.LastIndexOf('.');
                if (lastDot <= 0) continue;

                var typeFullName = full[..lastDot];
                var memberName = full[(lastDot + 1)..];

                var summary = m.Element("summary")?.Value ?? "";
                dict[(typeFullName, memberName)] = Trim(summary);
            }
        }

        return dict;
    }

    private static string Trim(string s) =>
        string.Join(' ', s.Split(['\r', '\n', '\t'], StringSplitOptions.RemoveEmptyEntries)).Trim();
}