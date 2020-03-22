﻿using System;
using System.Collections.Generic;
using System.IO;
using Komponent.IO.Attributes;
using Kontract.Interfaces.Progress;
using Kontract.Models.Archive;

namespace plugin_level5.Archives
{
    class Arc0Header
    {
        [FixedLength(4)]
        public string magic;
        public int directoryEntriesOffset;
        public int directoryHashOffset;
        public int fileEntriesOffset;
        public int nameOffset;
        public int dataOffset;
        public short directoryEntriesCount;
        public short directoryHashCount;
        public int fileEntriesCount;
        public uint unk1;
        public int zero1;

        //Hashes?
        public uint unk2;
        public uint unk3;
        public uint unk4;
        public uint unk5;

        public uint directoryCount;
        public int fileCount;
        public uint unk7;
        public int zero2;
    }

    class Arc0FileEntry
    {
        public uint crc32;  // only filename.ToLower()
        public uint nameOffsetInFolder;
        public uint fileOffset;
        public uint fileSize;
    }

    class Arc0DirectoryEntry
    {
        public uint crc32;   // directoryName.ToLower()
        public short firstDirectoryIndex;
        public short directoryCount;
        public short firstFileIndex;
        public short fileCount;
        public int fileNameStartOffset;
        public int directoryNameStartOffset;
    }

    class Arc0ArchiveFileInfo : ArchiveFileInfo
    {
        public Arc0FileEntry Entry { get; }

        public Arc0ArchiveFileInfo(Stream fileData, string filePath, Arc0FileEntry entry) :
            base(fileData, filePath)
        {
            Entry = entry;
        }

        public override void SaveFileData(Stream output, IProgressContext progress)
        {
            FileData.Position = 0;
            FileData.CopyTo(output);

            while (output.Position % 4 != 0)
                output.WriteByte(0);
        }
    }

    static class Arc0Support
    {
        public static IDictionary<string, Guid[]> PluginMappings = new Dictionary<string, Guid[]>
        {
            [".xi"] = new[] { Guid.Parse("898c9151-71bd-4638-8f90-6d34f0a8600c") },
            [".xr"] = new[] { Guid.Parse("de276e88-fb2b-48a6-a55f-d6c14ec60d4f") },
            [".xc"] = new[] { Guid.Parse("de276e88-fb2b-48a6-a55f-d6c14ec60d4f") },
            [".xa"] = new[] { Guid.Parse("de276e88-fb2b-48a6-a55f-d6c14ec60d4f") },
            [".xk"] = new[] { Guid.Parse("de276e88-fb2b-48a6-a55f-d6c14ec60d4f") }
        };
    }
}
