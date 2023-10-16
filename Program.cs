using RstbLibrary;
using System.Text;
using Zstandard.Net;
using Soft160.Data.Cryptography;
using System.IO.Compression;

namespace TalkingFlowerRepacker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            RemoveRSTBEntries(args[0]);
        }

        private static void RemoveRSTBEntries(string rstbPath)
        {
            // Read game dump rstb.zs
            RSTB restbl = RSTB.FromBinary(Decompress(rstbPath));
            
            // Remove entries matching crcs in .txt
            foreach (var path in File.ReadAllLines("./Dependencies/pathsToRemove.txt"))
            {
                uint crc = StringToCRC32(path);
                if (restbl.CrcMap.Any(x => x.Key.Equals(crc)))
                    restbl.CrcMap.Remove(crc);
                else
                    Console.WriteLine("Not Found: " + path);
            }

            // Create output directory
            string outPath = $"./Output/System/Resource/{Path.GetFileName(rstbPath)}";
            Directory.CreateDirectory(Path.GetDirectoryName(outPath));

            // Repack rstb.zs
            Compress(restbl.ToBinary().ToArray(), outPath);
        }

        private static uint StringToCRC32(string path)
        {
            return CRC.Crc32(Encoding.ASCII.GetBytes(path));
        }

        private static void GetPathCrcs(string dir)
        {
            string txt = "";
            foreach (var file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
                txt += file.Replace(dir + "\\", "").Replace("\\", "/") + "\n";
            File.WriteAllText("paths.txt", txt);
        }

        internal static void Compress(byte[] input, string output)
        {
            ZstdNet.Compressor compressor = new ZstdNet.Compressor();
            var outputBytes = compressor.Wrap(input);

            Directory.CreateDirectory(Path.GetDirectoryName(output));

            File.WriteAllBytes(output, outputBytes);
        }

        internal static byte[] Decompress(string input)
        {
            using (var ms = new MemoryStream(File.ReadAllBytes(input)))
            using (var compressionStream = new ZstandardStream(ms, CompressionMode.Decompress))
            using (var temp = new MemoryStream())
            {
                compressionStream.CopyTo(temp);
                byte[] outputBytes = temp.ToArray();
                return outputBytes;
            }
        }

        private static void ExtractTSVs(string msbtDir)
        {
            foreach (var msbtPath in Directory.GetFiles(msbtDir, "*.msbt", SearchOption.TopDirectoryOnly))
            {
                string txt = "";
                var msbt = MSBT.Deserialize(msbtPath);
                foreach (var pair in msbt)
                    txt += $"{pair.Item1}\t{pair.Item2.Replace("\n", "\\n").TrimEnd((char)0)}\t{pair.Item1}.wav\n";
                File.WriteAllText(msbtPath.Replace(".msbt", ".msbt.tsv"), txt);
            }
        }
    }
}