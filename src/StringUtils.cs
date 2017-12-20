using System.IO;

namespace Tessin
{
    static class StringUtils
    {
        public static string NormalizeLineEndings(string s)
        {
            // normalize line endings to use *nix style only
            var writer = new StringWriter { NewLine = "\n" };
            using (var reader = new StringReader(s.Trim()))
            {
                for (; ; )
                {
                    var line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    writer.WriteLine(line);
                }
            }
            return writer.ToString();
        }
    }
}
