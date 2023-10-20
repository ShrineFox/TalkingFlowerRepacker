# TalkingFlowerRepacker
Batch replace the sound bytes and text in Super Mario Wonder

# Notice
**This is an extremely work-in-progress** project at the moment. A usable release is not available yet.  
Currently, batch editing MSBT and BWAV is working, but replacements longer than the originals crash the game.  
A workaround is being investigated.

# What is This?
This is a proof-of-concept mass BWAV replacer and MSBT editor for Super Mario Wonder.  
In the testing phase, I'm focusing on replacing the voice/dialog lines of the Talking Flower using a ``.TSV`` (tab-separated-values) document.
For each row, the ``.TSV`` looks like this.  
```cs
LabelName\tDialogText\t..\Path\To\WavFile.wav
```
There is also a ``random.TSV`` that omits the label, so there's only two cells per line.
The BWAVs/text lines are then assigned indiscriminately to each piece of dialog.

# Future Plans
If I can find a workaround for the filesize crash, I'll publish some mods using this method alongside this source.  
I may also be interested in making a more general-use editor for more than just the Talking Flower clips.
