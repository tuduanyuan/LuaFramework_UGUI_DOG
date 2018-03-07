using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace Utility
{
    /// <summary> Zip工具类 </summary>
    public static class ZipUtil
    {
        /// <summary> 压缩数据 </summary>
        public static byte[] Compress(Stream source) {
            long length = source.Length;
            byte[] buffer = new byte[length];
            source.Read(buffer, 0, (int)length);
            source.Dispose();
            return Compress(buffer);
        }
        /// <summary> 解压数据 </summary>
        public static byte[] Decompress(byte[] source) {
            return Decompress(new MemoryStream(source));
        }
        /// <summary> 压缩数据 </summary>
        public static byte[] Compress(byte[] source)
        {
#if false//SCORPIO_UWP && !UNITY_EDITOR
            using (MemoryStream stream = new MemoryStream()) {
                System.IO.Compression.ZipArchive zipStream = new System.IO.Compression.ZipArchive(stream, System.IO.Compression.ZipArchiveMode.Create);
                System.IO.Compression.ZipArchiveEntry zipEntry = zipStream.CreateEntry("0.txt");
                Stream entryStream = zipEntry.Open();
                entryStream.Write(source, 0, source.Length);
                entryStream.Flush();
                entryStream.Dispose();
                zipStream.Dispose();
                byte[] ret = stream.ToArray();
                stream.Dispose();
                return ret;
            }
#else
            using (MemoryStream stream = new MemoryStream()) {
                ICSharpCode.SharpZipLib.Zip.ZipOutputStream zipStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(stream);
                zipStream.PutNextEntry(new ICSharpCode.SharpZipLib.Zip.ZipEntry("0.txt"));
                zipStream.Write(source, 0, source.Length);
                zipStream.Finish();
                byte[] ret = stream.ToArray();
                zipStream.Dispose();
                stream.Dispose();
                return ret;
            }
#endif
        }
        
        /// <summary> 解压数据 </summary>
        public static byte[] Decompress(Stream source)
        {
#if false//SCORPIO_UWP && !UNITY_EDITOR
            using (MemoryStream stream = new MemoryStream()) {
                System.IO.Compression.ZipArchive zipStream = new System.IO.Compression.ZipArchive(source, System.IO.Compression.ZipArchiveMode.Read);
                System.IO.Compression.ZipArchiveEntry zipEntry = zipStream.Entries[0];
                Stream entryStream = zipEntry.Open();
                int count = 0;
                byte[] data = new byte[4096];
                while ((count = entryStream.Read(data, 0, data.Length)) != 0)
                    stream.Write(data, 0, count);
                zipStream.Dispose();
                byte[] ret = stream.ToArray();
                stream.Dispose();
                return ret;
            }
#else
            using (MemoryStream stream = new MemoryStream()) {
                ICSharpCode.SharpZipLib.Zip.ZipInputStream zipStream = new ICSharpCode.SharpZipLib.Zip.ZipInputStream(source);
                zipStream.GetNextEntry();
                int count = 0;
                byte[] data = new byte[4096];
                while ((count = zipStream.Read(data, 0, data.Length)) != 0)
                    stream.Write(data, 0, count);
                zipStream.Flush();
                byte[] ret = stream.ToArray();
                zipStream.Dispose();
                stream.Dispose();
                return ret;
            }
#endif
        }
    }
}
