using ShrineFox.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkingFlowerRepacker
{
    public class BWAV
    {
        internal static void ReplaceRandomDialog(string sarcPath)
        {
            var placement = GetDialog();
            var voiceOnly = GetDialog("./Dependencies/TalkFlower_VoiceOnly.msbt.tsv");
            var random = GetRandomDialog();

            int rndCount = 0;
            for (int i = 0; i < placement.Count; i++)
            {
                if (rndCount >= random.Count)
                    rndCount = 0;

                var randomLine = random[rndCount];
                rndCount++;

                placement[i] = new Tuple<string, string, string>(placement[i].Item1, randomLine.Item1, randomLine.Item2);
            }
            for (int i = 0; i < voiceOnly.Count; i++)
            {
                if (rndCount >= random.Count)
                    rndCount = 0;

                var randomLine = random[rndCount];
                rndCount++;

                voiceOnly[i] = new Tuple<string, string, string>(voiceOnly[i].Item1, randomLine.Item1, randomLine.Item2);
            }

            CreateNewBWAVs();
            CopyBWAVs(placement, "TalkFlower_Placement_Stream");
            CopyBWAVs(voiceOnly, "TalkFlower_VoiceOnly_Stream");
            CreateNewMSBTs(placement, "TalkFlower_Placement.msbt");
            CreateNewMSBTs(voiceOnly, "TalkFlower_VoiceOnly.msbt");
            RepackMessageSARC(sarcPath);
        }

        private static void CreateNewBWAVs()
        {
            if (Directory.Exists("./Dependencies/Wav/Random"))
            {
                foreach (var wavPath in Directory.GetFiles("./Dependencies/Wav/Random", "*.wav", SearchOption.TopDirectoryOnly))
                {
                    string bwavPath = FileSys.GetExtensionlessPath(wavPath) + ".bwav";

                    if (!File.Exists(bwavPath) || new FileInfo(bwavPath).Length == 0)
                    {
                        Exe.Run(Path.GetFullPath("./Dependencies/brstm_converter-clang-amd64.exe"), 
                            $"\"{Path.GetFullPath(wavPath)}\" -o \"{Path.GetFullPath(bwavPath)}\"  --oEndian 0\"");

                        using (FileSys.WaitForFile(wavPath)) { }
                        if (!File.Exists(bwavPath) || new FileInfo(bwavPath).Length == 0)
                            Console.WriteLine($"Failed to convert to BWAV: {wavPath}");
                    }
                }
            }
        }

        private static void RepackMessageSARC(string sarcPath)
        {
            string msbtDir = "./Output/Mals/USen.Product.100_new/Voice";
            string extractedSarcDir = "./Output/Mals/USen.Product.100";
            File.WriteAllBytes(extractedSarcDir + "_uncompressed.sarc", ResourceTable.Decompress(sarcPath));

            SARC.ExtractToDir(extractedSarcDir + "_uncompressed.sarc", extractedSarcDir);

            foreach (var mstb in Directory.GetFiles(msbtDir, "*.msbt", SearchOption.TopDirectoryOnly))
                File.Copy(mstb, Path.Combine(extractedSarcDir, $"Voice/{Path.GetFileName(mstb)}"), true);
            SARC.Build(extractedSarcDir, extractedSarcDir + ".sarc");

            ResourceTable.Compress(File.ReadAllBytes(extractedSarcDir + ".sarc"), extractedSarcDir + ".sarc.zs");
        }

        private static void CreateNewMSBTs(List<Tuple<string, string, string>> list, string msbtName)
        {
            List<Tuple<string,string>> labelsAndText = new List<Tuple<string,string>>();
            foreach (var entry in list)
                labelsAndText.Add(new Tuple<string, string>(entry.Item1, entry.Item2));

            string outDir = "./Output/Mals/USen.Product.100_new/Voice";
            Directory.CreateDirectory(outDir);
            MSBT.Write(labelsAndText, Path.Combine(outDir, msbtName));
        }

        private static void CopyBWAVs(List<Tuple<string, string, string>> list, string folderName)
        {
            string outDir = $"./Output/Voice/Resource/USen/Voice/{folderName}";
            if (Directory.Exists(outDir))
                Directory.Delete(outDir, true);
            Directory.CreateDirectory(outDir);

            foreach (var entry in list)
            {
                string wavPath = Path.GetFullPath(Path.Combine("./Dependencies/Wav/", entry.Item3));
                string bwavPath = FileSys.GetExtensionlessPath(wavPath) + ".bwav";

                string copiedBwavPath = Path.Combine(outDir, entry.Item1 + ".bwav");

                if (File.Exists(bwavPath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(copiedBwavPath));
                    File.Copy(bwavPath, copiedBwavPath);
                }
        }
        }

        private static List<Tuple<string,string,string>> GetDialog(string tsvPath = "./Dependencies/TalkFlower_Placement.msbt.tsv")
        {
            List<Tuple<string, string, string>> dialog = new();
            foreach (var line in File.ReadAllLines(tsvPath))
            {
                if (!string.IsNullOrEmpty(line))
                {
                    var splits = line.Split('\t');
                    dialog.Add(new Tuple<string, string, string>(splits[0], splits[1], splits[2]));
                }
            }

            return dialog;
        }

        private static List<Tuple<string, string>> GetRandomDialog(string tsvPath = "./Dependencies/random.tsv")
        {
            List<Tuple<string, string>> randomDlg = new();
            foreach (var line in File.ReadAllLines(tsvPath))
            {
                if (!string.IsNullOrEmpty(line))
                {
                    var splits = line.Split('\t');
                    randomDlg.Add(new Tuple<string, string>(splits[0], splits[1]));
                }
            }

            return randomDlg;
        }

        internal static void UpdateSoundInfo(string bymlPath)
        {
            // TODO: Decompress ZSTD
            //File.WriteAllText($"./Dependencies/{Path.GetFileNameWithoutExtension(bymlPath)}.yml", Byml.FromBinary(File.ReadAllBytes(bymlPath)).ToText());
        }

        internal static void ReplaceBarsVoices(string extractedBarsDir, string bwavToUseDir)
        {
            var userBwav = Directory.GetFiles(bwavToUseDir, "*.bwav", SearchOption.AllDirectories);
            List<FileInfo> userBwavInfo = new();
            foreach (var file in userBwav)
                userBwavInfo.Add(new FileInfo(file));

            foreach (var bwav in Directory.GetFiles(extractedBarsDir, "*.BWAV", SearchOption.AllDirectories).Where(x => Path.GetFileNameWithoutExtension(x).StartsWith("Voice_Mario_")))
            {
                FileInfo fi = new FileInfo(bwav);
                var compatibleBwavs = userBwavInfo.Where(x => x.Length <= fi.Length).OrderBy(x => x.Length).Reverse();
                File.Copy(compatibleBwavs.First().FullName, bwav, true);
            }
        }
    }
}
