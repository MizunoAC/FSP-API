using System.Reflection;

namespace FSP.Domain.Helpers
{
    public class ResourceHelper
    {
        public static string? GetResource(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return null;

            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames();

            var resourceName = resourceNames
                .FirstOrDefault(name =>
                    name.Contains(".sql.", StringComparison.OrdinalIgnoreCase) &&
                    name.EndsWith($"{fileName}.sql", StringComparison.OrdinalIgnoreCase));

            if (resourceName == null)
                return null;

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null) return null;

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
