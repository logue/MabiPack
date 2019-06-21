using MabinogiResource;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace MabiPacker.Library
{
    internal class Unpacker : IDisposable
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
            Dispose();
        }
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _instance.Close();
            _instance.Dispose();
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
                using (PackResource Res = _instance.GetFileByIndex(i))
                {
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
                    p.Report(entry);
                }
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
                using (PackResource Res = _instance.GetFileByIndex(i))
                {
                    string InternalName = Res.GetName();
                    byte[] buffer = new byte[Res.GetSize()];
                    Res.GetData(buffer);
                    Res.Close();

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
                    p.Report(entry);
                }
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
                    Entry entry = new Entry(Res.GetName(), i);
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
            using (PackResource Res = _instance.GetFileByName(Name))
            {
                byte[] buffer = new byte[Res.GetSize()];
                Res.GetData(buffer);
                Res.Close();
                return buffer;
            }
        }
        /// <summary>
        /// Get file content by file index.
        /// </summary>
        /// <param name="Index">File index</param>
        /// <returns>byte</returns>
        public byte[] GetContent(uint Index)
        {
            using (PackResource Res = _instance.GetFileByIndex(Index))
            {
                byte[] buffer = new byte[Res.GetSize()];
                Res.GetData(buffer);
                Res.Close();
                return buffer;
            }
        }
        /// <summary>
        /// Extract file by filename
        /// </summary>
        /// <param name="Name">File Name</param>
        /// <param name="Distination">Output directory</param>
        public void Extract(string Name, string Distination)
        {
            using (PackResource Res = _instance.GetFileByName(Name))
            {
                byte[] buffer = new byte[Res.GetSize()];
                Res.GetData(buffer);
                Res.Close();
                string outputPath = Distination + "\\" + Name;

                using (FileStream fs = new FileStream(outputPath, FileMode.Create))
                {
                    fs.Write(buffer, 0, buffer.Length);
                    fs.Close();
                }
                // Modify File time
                File.SetCreationTime(outputPath, Res.GetCreated());
                File.SetLastAccessTime(outputPath, Res.GetAccessed());
                File.SetLastWriteTime(outputPath, Res.GetModified());
            }
        }
        /// <summary>
        /// Extract file by filename
        /// </summary>
        /// <param name="Index">File Index</param>
        /// <param name="Distination">Output directory</param>
        public void Extract(uint Index, string Distination)
        {
            using (PackResource Res = _instance.GetFileByIndex(Index))
            {
                byte[] buffer = new byte[Res.GetSize()];
                Res.GetData(buffer);
                Res.Close();
                string outputPath = Distination + "\\" + Res.GetName();

                using (FileStream fs = new FileStream(outputPath, FileMode.Create))
                {
                    fs.Write(buffer, 0, buffer.Length);
                    fs.Close();
                }
                // Modify File time
                File.SetCreationTime(outputPath, Res.GetCreated());
                File.SetLastAccessTime(outputPath, Res.GetAccessed());
                File.SetLastWriteTime(outputPath, Res.GetModified());
            }
        }
    }
}
