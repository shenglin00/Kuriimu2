﻿using Komponent.IO;
using Kontract.Interfaces.FileSystem;
using Kontract.Interfaces.Managers;
using Kontract.Interfaces.Plugins.Identifier;
using Kontract.Interfaces.Plugins.State;
using Kontract.Models;
using Kontract.Models.Context;
using Kontract.Models.IO;
using System;
using System.Threading.Tasks;

namespace plugin_sega.Text
{
    public class SharpMsgPlugin : IFilePlugin, IIdentifyFiles
    {
        public PluginType PluginType => PluginType.Text;

        public string[] FileExtensions => new[] { "*.MG" };

        public Guid PluginId => Guid.Parse("75414333-8C46-4014-A3DE-8227384D6527");

        public PluginMetadata Metadata { get; }

        public SharpMsgPlugin()
        {
            Metadata = new PluginMetadata("#MSG", "LITTOMA", "#MSG file found Sonic Generations");
        }

        public IPluginState CreatePluginState(IFileManager fileManager)
        {
            return new SharpMsgState();
        }

        public async Task<bool> IdentifyAsync(IFileSystem fileSystem, UPath filePath, IdentifyContext identifyContext)
        {
            var fs = await fileSystem.OpenFileAsync(filePath);
            var reader = new BinaryReaderX(fs);
            var header = reader.ReadType<SharpMsgHeader>();
            reader.Close();

            return header.magic == "#MSG";
        }
    }
}
