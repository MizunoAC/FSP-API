using System.Reflection;

namespace FSP.Domain.Helpers
{
    public class ResourceHelper
    {
            public static string? GetResource(string fileName)
            {
                if (string.IsNullOrWhiteSpace(fileName) ||
                    !fileName.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
                    return null;

                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = assembly
                    .GetManifestResourceNames()
                    .FirstOrDefault(name =>
                        name.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

                if (resourceName == null)
                    return null;

                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null) return null;

                using var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
    }
}
