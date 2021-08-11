// MabiPacker
// Copyright (c) 2019 by Logue <http://logue.be/>
// Distributed under the MIT license

using MabinogiResource;
using System;
using System.IO;
using System.Threading;

/// <summary>
/// Mabinogi package file unpacking Class
/// </summary>
namespace MabiPacker.Library
{
    internal class Packer : IDisposable
    {
        private PackResourceSetCreater _instance;
        private bool disposedValue;
        private readonly string _outputFile;
        private readonly string[] _files;
        private readonly string _destination;
        private readonly uint _count;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="OutputFile">Set filename of outputted *.pack file, with path.</param>
        /// <param name="Destination">Set destnation of data directory for pack.</param>
        /// <param name="Version">Set version of *.pack file.</param>
        /// <param name="Level">Set compress level of *.pack file.</param>
        public Packer(string OutputFile, string Destination, uint Version, int Level = -1)
        {
            if (File.Exists(OutputFile))
            {
                throw new IOException("Output file is already exsists.");
            }
            if (!Directory.Exists(Destination))
            {
                throw new DirectoryNotFoundException("Input directory is not found.");
            }
            _outputFile = OutputFile;
            _destination = Destination;
            _files = Directory.GetFiles(Destination, "*", SearchOption.AllDirectories);
            _count = (uint)_files.Length;
            _instance = new PackResourceSetCreater(Version, Level);
        }
        /// <summary>
        /// Get files to pack.
        /// </summary>
        /// <returns></returns>
        public uint Count()
        {
            return _count;
        }
        /// <summary>
        /// Packing Process
        /// </summary>
        /// <param name="p">Process</param>
        /// <param name="token">Cancell token</param>
        /// <returns></returns>
        public bool Pack(IProgress<Entry> p, CancellationToken token)
        {
            // Get Filelist
            uint i = 0;
            foreach (string path in _files)
            {
                _instance.AddFile(path.Replace(_destination + "\\", ""), path);
                if (token.IsCancellationRequested)
                {
                    return false;
                }
                Entry entry = new(path, i);

                p.Report(entry);
            }
            return _instance.CreatePack(_outputFile);
        }
        /// <summary>
        /// Packing Process
        /// </summary>
        /// <param name="p">Progress</param>
        /// <returns></returns>
        public bool Pack(IProgress<Entry> p)
        {
            uint i = 0;
            foreach (string path in _files)
            {

                _instance.AddFile(path.Replace(_destination + "\\", ""), path);
                Entry entry = new(path, i);

                p.Report(entry);
            }
            return _instance.CreatePack(_outputFile);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // TODO: 大きなフィールドを null に設定します
                _instance.Dispose();
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
