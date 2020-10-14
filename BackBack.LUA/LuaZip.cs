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
            var fi = new FileInfo(dest);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }

            using var stream = new FileStream(dest, FileMode.Create, FileAccess.ReadWrite);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Create);
            archive.CreateEntryFromFile(source, Path.GetFileName(source));
        }
    }
}
