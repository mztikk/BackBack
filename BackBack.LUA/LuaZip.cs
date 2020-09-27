using System.IO;
using System.IO.Compression;

namespace BackBack.LUA
{
    public static class LuaZip
    {
        public static void Zip(string source, string dest)
        {
            if (Directory.Exists(source))
            {
                ZipDirectory(source, dest);
            }
            else if (File.Exists(source))
            {
                Zip_File(source, dest);
            }
        }

        public static void ZipDirectory(string source, string dest)
        {
            string targetDir = Path.GetDirectoryName(dest);

            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            ZipFile.CreateFromDirectory(source, dest, CompressionLevel.Optimal, true);
        }

        public static void Zip_File(string source, string dest)
        {
            using var stream = new FileStream(dest, FileMode.Create, FileAccess.ReadWrite);
            using var archive = new ZipArchive(stream);
            archive.CreateEntryFromFile(source, Path.GetFileName(source));
        }
    }
}
