using MabinogiResource;
using System;
using System.IO;
using System.Threading;
namespace MabiPacker.Library
{
    internal class Packer
    {
        private readonly string _OutputFile;
        private readonly string[] _Files;
        private readonly string _Distination;
        private readonly PackResourceSetCreater _Instance;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="OutputFile">Set filename of outputted *.pack file, with path.</param>
        /// <param name="Distination">Set distnation of data directory for pack.</param>
        /// <param name="Version">Set version of *.pack file.</param>
        /// <param name="Level">Set compress level of *.pack file.</param>
        public Packer(string OutputFile, string Distination, uint Version, int Level = -1)
        {
            _Instance = new PackResourceSetCreater(Version, Level);
            _OutputFile = OutputFile;
            _Distination = Distination;
            _Files = Directory.GetFiles(Distination, "*", SearchOption.AllDirectories);
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~Packer()
        {
            _Instance.Dispose();
        }

        /// <summary>
        /// Packing Process
        /// </summary>
        /// <param name="p"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool Pack(IProgress<uint> p, CancellationToken token)
        {
            // Get Filelist
            uint i = 0;
            uint count = (uint)_Files.Length;
            foreach (string Path in _Files)
            {

                _Instance.AddFile(Path.Replace(_Distination + "\\", ""), Path);
                if (token.IsCancellationRequested)
                {
                    return false;
                }
                if (p != null)
                {
                    p.Report(i++ * 100 / count);
                }
            }
            return _Instance.CreatePack(_OutputFile);
        }
        /// <summary>
        /// Packing Process
        /// </summary>
        /// <param name="p"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool Pack()
        {

            foreach (string Path in _Files)
            {
                _Instance.AddFile(Path.Replace(_Distination + "\\", ""), Path);
            }
            return _Instance.CreatePack(_OutputFile);
        }
    }
}
