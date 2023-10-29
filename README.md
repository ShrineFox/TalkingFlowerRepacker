# About This Program
TalkingFlowerRepacker is a commandline utility I made to quickly generate matching text and voice files for Super Mario Wonder.  
Right now, it's only useful for the US English voices/text for the talking flower, but can be expanded for use with other things.  
It works by patching your RSTB (Resource Size Table) file (from ``romfs\System\Resource``) to remove all the flower-related size entries, allowing unlimited filesizes.  
Then, it generates a new SARC from your input ``romfs\Mals\USen.Product.100.sarc.zs`` file, containing your custom text.  
Finally, it generates replacements for the streaming audio BWAV files in ``romfs\Voice\Resource\USen\Voice\`` with your custom WAVs.

# Setup
1. Have a romfs dump of Super Mario Wonder's v1.0.0 files
2. Save your custom WAV files to the program's ``.\Dependencies\Wav`` folder (must be 16-bit Uncompressed PCM, use Audacity)
3. Right click the ``.\Dependencies\brstm_converter-clang-amd64.exe`` and choose Properties. Check the Compatibility Mode box  
  (only have to do this once, otherwise 0 BWAV byte files will be generated)
4. Edit ``.\Dependencies\TalkFlower_Placement.msbt.tsv`` and ``.\Dependencies\TalkFlower_VoiceOnly.msbt.tsv`` in a text editor.

# Editing Text
``.tsv`` is a tab-separated-value text document format. Meaning, each line is a "row" and each "cell" is separated by tab characters (``\t``).  
- The first cell is the "label" of the voice line. You don't want to change this, since this will be used when remaking the MSBT file.
- The second cell is the "text" of the voice line. Change this to whatever you want, preferably something that matches the next cell.
- The third cell is the path to the .WAV to use for that line.  
  Wav path is relative to the Wavs folder in the dependencies folder.  
  (i.e. ``.\MyWavs\Sound002.wav`` if it's in ``.\Dependencies\Wav\MyWavs\Sound002.wav``)

# Running the Program
1. Open the command prompt at the location of the program
2. Write the path to your input RSTB and message SARC (example below) then hit Enter  
  ``TalkingFlowerRepacker.exe "C:\Path\To\ResourceSizeTable.Product.100.rsizetable.zs" "C:\Path\To\USen.Product.100.sarc.zs"``
  
# Limitations
- Hardcoded to only work with USen files (message SARC and flower voices) for now
- Doesn't auto-remove extra files used to repack SARC from output directory yet
- Game might still crash sometimes for unknown reasons when encountering a flower. Try making your audio clips shorter than the originals.

# Credits
- Uses [RstbLibrary by VelouriasMoon](https://github.com/VelouriasMoon/RstbLibrary) for RSTB Patching
- Uses MSBT code by [IcySon55](https://github.com/IcySon55/3DLandMSBTeditor/blob/master/BinaryTools.cs)
- Uses [openrevolution by ic-scm](https://github.com/ic-scm/openrevolution) for BWAV conversion 