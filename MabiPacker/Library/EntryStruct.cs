// MabiPacker
// Copyright (c) 2019 by Logue <http://logue.be/>
// Distributed under the MIT license

using System.IO;

namespace MabiPacker.Library
{
    public struct Entry
    {
        public uint Index;
        public string Name;
        public string File;
        public string Extension;
        public uint Size;

        public Entry(string path, uint index, uint size = 0)
        {
            File = path;
            Name = Path.GetFileName(path);
            Extension = Path.GetExtension(path);
            Index = index;
            Size = size;
        }
    }
}
