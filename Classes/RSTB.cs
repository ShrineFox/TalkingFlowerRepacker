using RstbLibrary;
using ShrineFox.IO;
using Soft160.Data.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Zstandard.Net;

namespace TalkingFlowerRepacker
{
    public class ResourceTable
    {

        public static void RemoveEntries(string rstbPath)
        {
            if (!File.Exists(rstbPath))
            {
                Console.WriteLine($"Could not find path to input RSTB: \"{rstbPath}\"" +
                    $"\n\tSkipping RSTB patching...", ConsoleColor.Yellow);
                return;
            }

            // Read game dump rstb.zs
            RSTB restbl = RSTB.FromBinary(Decompress(rstbPath));

            // Remove entries matching crcs in .txt
            foreach (var path in File.ReadAllLines("./Dependencies/pathsToRemove.txt"))
            {
                uint crc = StringToCRC32(path);
                if (restbl.CrcMap.Any(x => x.Key.Equals(crc)))
                {
                    restbl.CrcMap.Remove(crc);
                    Output.Log($"Successfully found and removed CRC32 ({crc}) for path: \"{path}\"", ConsoleColor.Green);
                }
                else
                    Output.Log($"Could not find CRC ({crc}) for path: \"{path}\"", ConsoleColor.Red);
            }

            // Create output directory
            string outPath = $"./Output/System/Resource/{Path.GetFileName(rstbPath)}";
            Directory.CreateDirectory(Path.GetDirectoryName(outPath));

            // Repack rstb.zs
            Compress(restbl.ToBinary().ToArray(), outPath);

            Output.Log($"\n\nSaved new RSTB file to: \"{outPath}\"");
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
