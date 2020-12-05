// MabiPacker
// Copyright (c) 2019 by Logue <http://logue.be/>
// Distributed under the MIT license

using System.IO;

namespace MabiPacker.Library
{
    public struct Entry
    {
        public uint Index { get; }
        public string Name { get; }
        public string File { get; }
        public string Extension { get; }
        public uint Size { get; }

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
