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
            //ResourceTable.RemoveEntries(args[0]);
            BWAV.ReplaceRandomDialog(args[1]);
        }

    }
}