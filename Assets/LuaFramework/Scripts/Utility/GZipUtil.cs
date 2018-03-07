using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace Utility
{
    /// <summary> GZIP 工具类 </summary>
    public static class GZipUtil
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
        public static byte[] Compress(byte[] source) {
/*
#if SCORPIO_UWP && !UNITY_EDITOR
            using (MemoryStream stream = new MemoryStream()) {
                System.IO.Compression.GZipStream zipStream = new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Compress);
                zipStream.Write(source, 0, source.Length);
                zipStream.Flush();
                zipStream.Dispose();
                byte[] ret = stream.ToArray();
                stream.Dispose();
                return ret;
            }
#else
*/
            using (MemoryStream stream = new MemoryStream()) {
                ICSharpCode.SharpZipLib.GZip.GZipOutputStream zipStream = new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(stream);
                zipStream.Write(source, 0, source.Length);
                zipStream.Finish();
                byte[] ret = stream.ToArray();
                zipStream.Dispose();
                stream.Dispose();
                return ret;
            }
/*
 *#endif
 */
        }
        /// <summary> 解压数据 </summary>
        public static byte[] Decompress(Stream source) {
/*
#if SCORPIO_UWP && !UNITY_EDITOR
            using (MemoryStream stream = new MemoryStream()) {
                System.IO.Compression.GZipStream zipStream = new System.IO.Compression.GZipStream(source, System.IO.Compression.CompressionMode.Decompress);
                int count = 0;
                byte[] data = new byte[4096];
                while ((count = zipStream.Read(data, 0, data.Length)) != 0) {
                    stream.Write(data, 0, count);
                }
                zipStream.Flush();
                zipStream.Dispose();
                byte[] ret = stream.ToArray();
                stream.Dispose();
                return ret;
            }
#else
*/
            using (MemoryStream stream = new MemoryStream()) {
                ICSharpCode.SharpZipLib.GZip.GZipInputStream zipStream = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(source);
                int count = 0;
                byte[] data = new byte[4096];
                while ((count = zipStream.Read(data, 0, data.Length)) != 0) {
                    stream.Write(data, 0, count);
                }
                zipStream.Flush();
                byte[] ret = stream.ToArray();
                zipStream.Dispose();
                stream.Dispose();
                return ret;
            }

/*
 * #endif
 */
        }
    }
}
