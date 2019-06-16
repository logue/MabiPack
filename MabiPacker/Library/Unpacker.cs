using MabinogiResource;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace MabiPacker.Library
{
    internal class Unpacker
    {
        private readonly PackResourceSet _instance;
        private readonly uint _count;
        private readonly string _Distination;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="File"></param>
        public Unpacker(string File, string Distination)
        {
            _instance = PackResourceSet.CreateFromFile(File);
            _count = _instance.GetFileCount();
            _Distination = Distination;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="File"></param>
        public Unpacker(string File)
        {
            _instance = PackResourceSet.CreateFromFile(File);
            _count = _instance.GetFileCount();
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~Unpacker()
        {
            _instance.Dispose();
        }
        /// <summary>
        /// Unpack all files
        /// </summary>
        /// <param name="p"></param>
        /// <param name="token"></param>
        public void Unpack(IProgress<uint> p, CancellationToken token)
        {
            for (uint i = 0; i < _count; ++i)
            {
                PackResource Res = _instance.GetFileByIndex(i);
                string InternalName = Res.GetName();
                byte[] buffer = new byte[Res.GetSize()];
                Res.GetData(buffer);
                Res.Close();

                if (token.IsCancellationRequested)
                {
                    return;
                }

                string outputPath = _Distination + "\\data\\" + InternalName;
                // Create directory
                string DirPath = Regex.Replace(outputPath, @"([^\\]*?)$", "");
                if (!Directory.Exists(DirPath))
                {
                    Directory.CreateDirectory(DirPath);
                }
                // Delete old
                if (File.Exists(outputPath))
                {
                    //DateTime dtUpdate = System.IO.File.GetLastWriteTime(outputPath);
                    //if (dtUpdate > Res.GetModified()){
                    File.Delete(@outputPath);
                    //}else{
                    //Todo Overwrite confirm dialog
                    //}
                }

                // Write to file.
                FileStream fs = new FileStream(outputPath, FileMode.Create);
                fs.Write(buffer, 0, buffer.Length);
                fs.Close();
                // Modify File time
                File.SetCreationTime(outputPath, Res.GetCreated());
                File.SetLastAccessTime(outputPath, Res.GetAccessed());
                File.SetLastWriteTime(outputPath, Res.GetModified());
                // Progreess
                p.Report(i * 100 / _count);
            }
        }

        /// <summary>
        /// Get File List
        /// </summary>
        /// <returns></returns>
        public Dictionary<uint, string> Entries(IProgress<uint> p = null)
        {
            Dictionary<uint, string> ret = new Dictionary<uint, string>();
            for (uint i = 0; i < _count; ++i)
            {
                PackResource Res = _instance.GetFileByIndex(i);
                ret.Add(i, Res.GetName());
                if (p != null)
                {
                    p.Report(i * 100 / _count);
                }

            }
            return ret;
        }

        public uint Length => _count;
        /// <summary>
        /// Get file content by filename.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public byte[] Content(string Name)
        {
            PackResource Res = _instance.GetFileByName(Name);
            byte[] buffer = new byte[Res.GetSize()];
            Res.GetData(buffer);
            Res.Close();
            return buffer;
        }
        /// <summary>
        /// Get file content by Index.
        /// </summary>
        /// <param name="Index">File Index</param>
        /// <returns></returns>
        public byte[] GetContent(uint Index)
        {
            PackResource Res = _instance.GetFileByIndex(Index);
            byte[] buffer = new byte[Res.GetSize()];
            Res.GetData(buffer);
            Res.Close();
            return buffer;
        }
        /// <summary>
        /// Extract file by filename
        /// </summary>
        /// <param name="Name">File Name</param>
        /// <param name="Distination">Output directory</param>
        public void Extract(string Name, string Distination)
        {
            PackResource Res = _instance.GetFileByName(Name);
            byte[] buffer = new byte[Res.GetSize()];
            Res.GetData(buffer);
            Res.Close();
            string outputPath = Distination + "\\" + Name;

            FileStream fs = new FileStream(outputPath, FileMode.Create);
            fs.Write(buffer, 0, buffer.Length);
            fs.Close();
            // Modify File time
            File.SetCreationTime(outputPath, Res.GetCreated());
            File.SetLastAccessTime(outputPath, Res.GetAccessed());
            File.SetLastWriteTime(outputPath, Res.GetModified());
        }
        /// <summary>
        /// Extract file by filename
        /// </summary>
        /// <param name="Index">File Index</param>
        /// <param name="Distination">Output directory</param>
        public void Extract(uint Index, string Distination)
        {
            PackResource Res = _instance.GetFileByIndex(Index);
            byte[] buffer = new byte[Res.GetSize()];
            Res.GetData(buffer);
            Res.Close();
            string outputPath = Distination + "\\" + Res.GetName();

            FileStream fs = new FileStream(outputPath, FileMode.Create);
            fs.Write(buffer, 0, buffer.Length);
            fs.Close();
            // Modify File time
            File.SetCreationTime(outputPath, Res.GetCreated());
            File.SetLastAccessTime(outputPath, Res.GetAccessed());
            File.SetLastWriteTime(outputPath, Res.GetModified());
        }
    }
}
