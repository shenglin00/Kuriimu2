﻿using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Kanvas;
using Kanvas.Format;
using Kanvas.Interface;
using Kanvas.Palette;
using Komponent.IO;

namespace plugin_yuusha_shisu.BTX
{
    /// <summary>
    /// 
    /// </summary>
    public class BTX
    {
        /// <summary>
        /// 
        /// </summary>
        public FileHeader Header;

        /// <summary>
        /// 
        /// </summary>
        public Bitmap Texture;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        public BTX(Stream input)
        {
            using (var br = new BinaryReaderX(input, true))
            {
                // Read
                Header = br.ReadType<FileHeader>();
                br.SeekAlignment();
                var fileName = br.ReadCStringASCII();

                // Setup
                var dataLength = Header.Width * Header.Height;
                var paletteDataLength = Header.ColorCount * 4;

                // Image
                br.BaseStream.Position = Header.ImageOffset;
                var texture = br.ReadBytes(dataLength);

                // Settings
                var settings = new ImageSettings
                {
                    Width = Header.Width,
                    Height = Header.Height,
                    Format = Formats[Header.Format]
                };

                // Palette
                if (Header.Format == ImageFormat.Palette_8)
                {
                    br.BaseStream.Position = Header.PaletteOffset;
                    var palette = br.ReadBytes(paletteDataLength);
                    (settings.Format as IPaletteFormat)?.SetPalette(Formats[ImageFormat.RGBA8888].Load(palette));
                }

                // Load!
                Texture = Common.Load(texture, settings);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="output"></param>
        public void Save(Stream output)
        {
            using (var bw = new BinaryWriterX(output, true))
            {

            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<ImageFormat, IImageFormat> Formats = new Dictionary<ImageFormat, IImageFormat>
        {
            [ImageFormat.RGBA8888] = new RGBA(8, 8, 8, 8) { ByteOrder = ByteOrder.BigEndian },
            [ImageFormat.Palette_8] = new Palette(8)
        };
    }
}
