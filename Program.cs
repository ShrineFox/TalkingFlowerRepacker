﻿using RstbLibrary;
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
            BWAV.ReplaceDialog(args[1]);
            //BWAV.ReplaceBarsVoices(args[2], args[3]);

            Output.Log("\n\nDone, press any key to exit.");
            Console.ReadKey();
        }

    }
}