// MabiPacker
// Copyright (c) 2019 by Logue <http://logue.be/>
// Distributed under the MIT license

using MabinogiResource;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

/// <summary>
/// Mabinogi Package file unpacking class
/// </summary>
namespace MabiPacker.Library
{
    internal class Unpacker : IDisposable
    {
        private PackResourceSet _instance;
        private bool disposedValue;
        private readonly uint _count;
        private readonly string _destination;
        private readonly string _inputFile;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="InputFile">*.pack file to unpack</param>
        /// <param name="Destination">output directory</param>
        public Unpacker(string InputFile, string Destination)
        {
            if (!File.Exists(InputFile))
            {
                throw new FileNotFoundException("File to unpack is not found.");
            }
            if (!Directory.Exists(Destination))
            {
                throw new DirectoryNotFoundException("Output directory is not found.");
            }
            _inputFile = InputFile;
            _instance = PackResourceSet.CreateFromFile(InputFile);
            _count = _instance.GetFileCount();
            _destination = Destination;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="File">*.pack file to unpack</param>
        public Unpacker(string InputFile)
        {
            if (!File.Exists(InputFile))
            {
                throw new FileNotFoundException("File to unpack is not found.");
            }
            _inputFile = InputFile;
            _instance = PackResourceSet.CreateFromFile(InputFile);
            _count = _instance.GetFileCount();
            _destination = Path.GetDirectoryName(Path.GetFullPath(Environment.GetCommandLineArgs()[0]));
        }
        /// <summary>
        /// Get packed files.
        /// </summary>
        /// <returns></returns>
        public uint Count()
        {
            return _count;
        }
        /// <summary>
        /// Unpack all files
        /// </summary>
        /// <param name="p"></param>
        /// <param name="token"></param>
        public void Unpack(IProgress<Entry> p, CancellationToken token)
        {
            for (uint i = 0; i < _count; ++i)
            {
                using PackResource Res = _instance.GetFileByIndex(i);
                string InternalName = Res.GetName();
                byte[] buffer = new byte[Res.GetSize()];
                Res.GetData(buffer);

                if (token.IsCancellationRequested)
                {
                    Res.Close();
                    return;
                }

                string outputPath = _destination + "\\data\\" + InternalName;
                // Create directory
                string DirPath = Regex.Replace(outputPath, @"([^\\]*?)$", "");
                if (!Directory.Exists(DirPath))
                {
                    Directory.CreateDirectory(DirPath);
                }
                // Delete old
                if (File.Exists(outputPath))
                {
                    DateTime dtUpdate = File.GetLastWriteTime(outputPath);
                    if (dtUpdate < Res.GetModified())
                    {
                        File.Delete(@outputPath);
                    }
                    else
                    {
                        // skip exists.
                        Res.Close();
                        continue;
                    }
                }

                // Write to file.
                using (FileStream fs = new FileStream(outputPath, FileMode.Create))
                {
                    fs.Write(buffer, 0, buffer.Length);
                    fs.Close();
                }
                // Modify File time
                File.SetCreationTime(outputPath, Res.GetCreated());
                File.SetLastAccessTime(outputPath, Res.GetAccessed());
                File.SetLastWriteTime(outputPath, Res.GetModified());

                // Progreess
                Entry entry = new Entry(InternalName, i, Res.GetSize());

                Res.Close();
                p.Report(entry);
            }
        }
        /// <summary>
        /// Unpack all files
        /// </summary>
        /// <param name="p"></param>
        public void Unpack(IProgress<Entry> p)
        {
            for (uint i = 0; i < _count; ++i)
            {
                using PackResource Res = _instance.GetFileByIndex(i);
                string InternalName = Res.GetName();
                byte[] buffer = new byte[Res.GetSize()];
                Res.GetData(buffer);

                string outputPath = _destination + "\\data\\" + InternalName;
                // Create directory
                string DirPath = Regex.Replace(outputPath, @"([^\\]*?)$", "");
                if (!Directory.Exists(DirPath))
                {
                    Directory.CreateDirectory(DirPath);
                }
                // Delete old
                if (File.Exists(outputPath))
                {
                    DateTime dtUpdate = File.GetLastWriteTime(outputPath);
                    if (dtUpdate < Res.GetModified())
                    {
                        File.Delete(@outputPath);
                    }
                    else
                    {
                        // skip exists
                        continue;
                    }
                }

                // Write to file.
                using (FileStream fs = new FileStream(outputPath, FileMode.Create))
                {
                    fs.Write(buffer, 0, buffer.Length);
                    fs.Close();
                }
                // Modify File time
                File.SetCreationTime(outputPath, Res.GetCreated());
                File.SetLastAccessTime(outputPath, Res.GetAccessed());
                File.SetLastWriteTime(outputPath, Res.GetModified());

                // Progreess
                Entry entry = new(InternalName, i, Res.GetSize());

                Res.Close();
                p.Report(entry);
            }
        }

        /// <summary>
        /// Get File List
        /// </summary>
        /// <returns></returns>
        public List<Entry> Entries()
        {
            List<Entry> ret = new List<Entry>();
            for (uint i = 0; i < _count; ++i)
            {
                using (PackResource Res = _instance.GetFileByIndex(i))
                {
                    Entry entry = new(Res.GetName(), i);
                    ret.Add(entry);
                }
            }
            // Sort by Name
            ret.Sort((a, b) => a.File.CompareTo(b.File));

            return ret;
        }
        /// <summary>
        /// Get file content by filename.
        /// </summary>
        /// <param name="Name">File name</param>
        /// <returns>byte</returns>
        public byte[] GetContent(string Name)
        {
            byte[] buffer;
            using (PackResource Res = PackResourceSet.CreateFromFile(_inputFile).GetFileByName(Name))
            {
                buffer = new byte[Res.GetSize()];
                _ = Res.GetData(buffer);
                Res.Close();
            }
            return buffer;

        }
        /// <summary>
        /// Get file content by file index.
        /// </summary>
        /// <param name="Index">File index</param>
        /// <returns>byte</returns>
        public byte[] GetContent(uint Index)
        {
            byte[] buffer;
            using (PackResource Res = PackResourceSet.CreateFromFile(_inputFile).GetFileByIndex(Index))
            {
                buffer = new byte[Res.GetSize()];
                _ = Res.GetData(buffer);
                Res.Close();
            }
            return buffer;
        }
        /// <summary>
        /// Extract file by filename
        /// </summary>
        /// <param name="Name">File Name</param>
        public void Extract(string Name)
        {
            using PackResource Res = _instance.GetFileByName(Name);
            byte[] buffer = new byte[Res.GetSize()];
            Res.GetData(buffer);
            string outputPath = _destination + "\\" + Name;

            using (FileStream fs = new(outputPath, FileMode.Create))
            {
                fs.Write(buffer, 0, buffer.Length);
                fs.Close();
            }
            // Modify File time
            File.SetCreationTime(outputPath, Res.GetCreated());
            File.SetLastAccessTime(outputPath, Res.GetAccessed());
            File.SetLastWriteTime(outputPath, Res.GetModified());
            Res.Close();
        }
        /// <summary>
        /// Extract file by filename
        /// </summary>
        /// <param name="Index">File Index</param>
        public void Extract(uint Index)
        {
            using PackResource Res = _instance.GetFileByIndex(Index);
            byte[] buffer = new byte[Res.GetSize()];
            _ = Res.GetData(buffer);
            
            string outputPath = _destination + "\\" + Res.GetName();
            using (FileStream fs = new(outputPath, FileMode.Create))
            {
                fs.Write(buffer, 0, buffer.Length);
                fs.Close();
            }
            // Modify File time
            File.SetCreationTime(outputPath, Res.GetCreated());
            File.SetLastAccessTime(outputPath, Res.GetAccessed());
            File.SetLastWriteTime(outputPath, Res.GetModified());
            Res.Close();
        }
        /// <summary>
        /// Extract file by filename
        /// </summary>
        /// <param name="Name">File Name</param>
        /// <param name="output">Output directory</param>
        public void Extract(string Name, string output)
        {
            using PackResource Res = _instance.GetFileByName(Name);
            byte[] buffer = new byte[Res.GetSize()];
            _ = Res.GetData(buffer);

            string outputPath = output + "\\" + Name;

            using (FileStream fs = new(output, FileMode.Create))
            {
                fs.Write(buffer, 0, buffer.Length);
                fs.Close();
            }
            // Modify File time
            File.SetCreationTime(outputPath, Res.GetCreated());
            File.SetLastAccessTime(outputPath, Res.GetAccessed());
            File.SetLastWriteTime(outputPath, Res.GetModified());

            Res.Close();
        }
        /// <summary>
        /// Extract file by filename
        /// </summary>
        /// <param name="Index">File Index</param>
        /// <param name="output">Output directory</param>
        public void Extract(uint Index, string output)
        {
            using PackResource Res = _instance.GetFileByIndex(Index);
            byte[] buffer = new byte[Res.GetSize()];
            _ = Res.GetData(buffer);
            string outputPath = output + "\\" + Res.GetName();

            using (FileStream fs = new(outputPath, FileMode.Create))
            {
                fs.Write(buffer, 0, buffer.Length);
                fs.Close();
            }
            // Modify File time
            File.SetCreationTime(outputPath, Res.GetCreated());
            File.SetLastAccessTime(outputPath, Res.GetAccessed());
            File.SetLastWriteTime(outputPath, Res.GetModified());
            Res.Close();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)
                    // _instance.Close();
                    // _instance.Dispose();
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // TODO: 大きなフィールドを null に設定します
                disposedValue = true;
            }
        }

        // // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
        // ~Packer()
        // {
        //     // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
