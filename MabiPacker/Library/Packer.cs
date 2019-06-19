using MabinogiResource;
using System;
using System.IO;
using System.Threading;
namespace MabiPacker.Library
{
    internal class Packer : IDisposable
    {
        private readonly string _outputFile;
        private readonly string[] _files;
        private readonly string _distination;
        private readonly int _level;
        private readonly uint _version;
        private readonly uint _count;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="OutputFile">Set filename of outputted *.pack file, with path.</param>
        /// <param name="Distination">Set distnation of data directory for pack.</param>
        /// <param name="Version">Set version of *.pack file.</param>
        /// <param name="Level">Set compress level of *.pack file.</param>
        public Packer(string OutputFile, string Distination, uint Version, int Level = -1)
        {
            _version = Version;
            _level = Level;
            _outputFile = OutputFile;
            _distination = Distination;
            _files = Directory.GetFiles(Distination, "*", SearchOption.AllDirectories);
            _count = (uint)_files.Length;
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~Packer()
        {
            Dispose();
        }
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {

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
        /// <param name="p"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool Pack(IProgress<Entry> p, CancellationToken token)
        {
            // Get Filelist
            uint i = 0;
            using (PackResourceSetCreater instance = new PackResourceSetCreater(_version, _level))
            {
                foreach (string path in _files)
                {
                    instance.AddFile(path.Replace(_distination + "\\", ""), path);
                    if (token.IsCancellationRequested)
                    {
                        return false;
                    }
                    Entry entry = new Entry(path, i);

                    p.Report(entry);
                }
                return instance.CreatePack(_outputFile);
            }
        }
        /// <summary>
        /// Packing Process
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool Pack(IProgress<Entry> p)
        {
            uint i = 0;
            using (PackResourceSetCreater instance = new PackResourceSetCreater(_version, _level))
            {
                foreach (string path in _files)
                {

                    instance.AddFile(path.Replace(_distination + "\\", ""), path);
                    Entry entry = new Entry(path, i);

                    p.Report(entry);
                }
                return instance.CreatePack(_outputFile);
            }
        }
    }
}
