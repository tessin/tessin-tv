using System.IO;

namespace Tessin
{
    static class ResourceUtils
    {
        public static string GetStringResource(string name)
        {
            using (var reader = new StreamReader(typeof(RaspberryPiManager).Assembly.GetManifestResourceStream($"Tessin.{name}")))
            {
                return StringUtils.NormalizeLineEndings(reader.ReadToEnd());
            }
        }
    }
}
