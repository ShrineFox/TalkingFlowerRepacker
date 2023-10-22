using RstbLibrary;
using System.Text;
using Zstandard.Net;
using Soft160.Data.Cryptography;
using System.IO.Compression;
using ShrineFox.IO;

namespace TalkingFlowerRepacker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ShrineFox.IO.Output.Logging = true;

            ResourceTable.RemoveEntries(args[0]);
            BWAV.ReplaceRandomDialog(args[1]);

            Output.Log("\n\nDone, press any key to exit.");
            Console.ReadKey();
        }

    }
}