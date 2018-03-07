#if !UNITY_EDITOR && UNITY_WEBGL
#define CACHE_FILE
#endif
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace Utility
{
    /// <summary> File工具类 </summary>
    public static class FileUtil {
        private static Dictionary<string, byte[]> m_CacheFiles = new Dictionary<string, byte[]>();
        private static Encoding DefaultEncoding = new UTF8Encoding(false);
        /// <summary> 字符串头一个字母大写 </summary>
        public static string ToOneUpper(string str) {
            if (string.IsNullOrEmpty(str))  return str;
            if (str.Length <= 1) return str.ToUpper();
            return char.ToUpper(str[0]) + str.Substring(1);
        }
        /// <summary> 格式化路径\换成/ </summary>
        public static string FormatPath(string path) {
            if (string.IsNullOrEmpty(path)) return "";
            return path.Replace('\\', '/');
        }
        /// <summary> 删除后缀名 </summary>
        public static string RemoveExtension(string file) {
            var index = file.LastIndexOf(".");
            return file.Substring(0, index);
        }
        /// <summary> 根据完整路径获得文件名 </summary>
        public static string GetFileName(string path) {
			return Path.GetFileName (path);
        }
        /// <summary> 根据完整路径获得文件路径 </summary>
        public static string GetFilePath(string path) {
            return FormatPath(Path.GetDirectoryName(path));
        }
        /// <summary> 判断文件是否存在 </summary>
        public static bool FileExist(string file) {
#if CACHE_FILE
            return !string.IsNullOrEmpty(file) && m_CacheFiles.ContainsKey(file);
#else
            return !string.IsNullOrEmpty(file) && File.Exists(file);
#endif
        }
        /// <summary> 判断文件夹是否存在 </summary>
        public static bool PathExist(string path) {
            return !string.IsNullOrEmpty(path) && Directory.Exists(path);
        }
        /// <summary> 创建一个目录 </summary>
        public static void CreateDirectoryByFile(string file) {
            CreateDirectory(Path.GetDirectoryName(file));
        }
        /// <summary> 创建一个目录 </summary>
        public static void CreateDirectory(string path) {
            try {
#if !CACHE_FILE
                if (!PathExist(path)) Directory.CreateDirectory(path);
#endif
            } catch (Exception ex) {
                logger.error("CreateDirectory is error : {0}", ex.ToString());
            }
        }
        /// <summary> 根据字符串创建文件 </summary>
        public static void CreateFile(string fileName, string buffer, string[] filePath, Encoding encoding) {
            if (filePath == null || filePath.Length < 0) return;
            for (int i = 0; i < filePath.Length; ++i) {
                if (!string.IsNullOrEmpty(filePath[i])) CreateFile(filePath[i] + "/" + fileName, buffer, encoding);
            }
        }
        /// <summary> 根据字符串创建一个文件 </summary>
        public static void CreateFile(string fileName, string buffer) {
            CreateFile(fileName, buffer, DefaultEncoding);
        }
        /// <summary> 根据字符串创建一个文件 </summary>
        public static void CreateFile(string fileName, string buffer, Encoding encoding) {
            try {
                CreateFile(fileName, encoding.GetBytes(buffer));
            } catch (Exception ex) {
                logger.error("CreateFile is error : {0}", ex.ToString());
            }
        }
        /// <summary> 根据byte[]创建文件 </summary>
        public static void CreateFile(string fileName, byte[] buffer, string[] filePath) {
            if (filePath == null || filePath.Length < 0) return;
            for (int i = 0; i < filePath.Length; ++i) {
                if (!string.IsNullOrEmpty(filePath[i]))
                    CreateFile(filePath[i] + "/" + fileName, buffer);
            }
        }
        /// <summary> 根据byte[]创建一个文件 </summary>
        public static void CreateFile(string fileName, byte[] buffer) {
            try {
                if (string.IsNullOrEmpty(fileName)) return;
#if CACHE_FILE
                m_CacheFiles[fileName] = buffer;
#else
                CreateDirectory(Path.GetDirectoryName(fileName));
                if (File.Exists(fileName)) File.Delete(fileName);
                using (FileStream fs = new FileStream(fileName, FileMode.Create)) {
                    fs.Write(buffer, 0, buffer.Length);
                    fs.Flush();
                }
#endif
            } catch (Exception ex) {
                logger.error("CreateFile is error : {0}", ex.ToString());
            }
        }
         /// <summary> 获得文件字符串 </summary>
        public static string GetFileString(string fileName) {
            return GetFileString(fileName, DefaultEncoding);
        }
        /// <summary> 获得文件字符串 </summary>
        public static string GetFileString(string fileName, Encoding encoding) {
            try {
                byte[] buffer = GetFileBuffer(fileName);
				if (buffer == null) return null;
                return encoding.GetString(buffer, 0, buffer.Length);
            } catch (Exception ex) {
                logger.error("GetFileString is error : {0}", ex.ToString());
            }
            return null;
        }
        /// <summary> 获得文件byte[] </summary>
        public static byte[] GetFileBuffer(string fileName) {
            try {
                if (!FileExist(fileName))
                    throw new Exception("File is not found : " + fileName);
#if CACHE_FILE
                return m_CacheFiles[fileName];
#else
                using (FileStream fs = new FileStream(fileName, FileMode.Open)) {
                    long length = fs.Length;
                    byte[] buffer = new byte[length];
                    fs.Read(buffer, 0, (int)length);
                    return buffer;
                }
#endif
            } catch (Exception ex) {
                logger.error("GetFileBuffer is error : {0}", ex.ToString());
            }
            return null;
        }
        /// <summary> 删除文件 </summary>
        public static void DeleteFile(string fileName) {
            if (FileExist(fileName)) {
#if CACHE_FILE
                m_CacheFiles.Remove(fileName);
#else
                File.Delete(fileName);
#endif
            }
        }
        /// <summary> 删除文件 </summary>
        public static void DeleteFile(string fileName, string[] filePath) {
            if (filePath == null || filePath.Length < 0) return;
            for (int i = 0; i < filePath.Length; ++i) {
                if (!string.IsNullOrEmpty(filePath[i]))
                    DeleteFile(filePath[i] + "/" + fileName);
            }
        }
        /// <summary> 复制文件 </summary>
        public static void CopyFile(string sourceFile, string destFile, bool overwrite) {
            if (FileExist(sourceFile)) {
                CreateDirectoryByFile(destFile);
                File.Copy(sourceFile, destFile, overwrite);
            }
        }
        /// <summary> 移动文件 </summary>
        public static void MoveFile(string sourceFile, string destFile, bool overwrite) {
            if (FileExist(sourceFile)) {
                CreateDirectoryByFile(destFile);
                if (overwrite) DeleteFile(destFile);
                File.Move(sourceFile, destFile);
            }
        }
        public static string[] GetFileList(string path, string searchPattern, bool recursive) {
            return GetFileList(path, searchPattern, recursive, false);
        }
        public static string[] GetFileList(string path, string searchPattern, bool recursive, bool sort) {
            List<string> files = new List<string>();
            GetFileList(files, path, searchPattern, recursive, sort);
            return files.ToArray();
		}
        private static void GetFileList(List<string> files, string path, string searchPattern, bool recursive, bool sort) {
            try {
#if CACHE_FILE
                foreach (var pair in m_CacheFiles) {
                    if (pair.Key.StartsWith(path)) {
                        files.Add(pair.Key);
                    }
                }
                if (sort) files.Sort();
#else
                if (!PathExist(path)) return;
                string[] getFiles = Directory.GetFiles(path, searchPattern);
                if (sort) Array.Sort(getFiles);
                files.AddRange(getFiles);
                if (recursive) {
                    string[] paths = Directory.GetDirectories(path);
                    if (sort) Array.Sort(paths);
                    foreach (var p in paths)
                        GetFileList(files, p, searchPattern, recursive, sort);
                }
#endif
            } catch (System.Exception e) {
				logger.error("GetFileList is error : {0}", e.ToString());
			}
        }
        /// <summary> 删除文件夹 </summary>
        public static void DeleteFiles(string sourceFolder, string strFilePattern, bool recursive) {
            if (!PathExist(sourceFolder)) return;
            string[] files = Directory.GetFiles(sourceFolder, strFilePattern);
            foreach (string file in files) { File.Delete(file); }
            if (recursive) {
                string[] folders = Directory.GetDirectories(sourceFolder);
                foreach (string folder in folders)
                    DeleteFiles(folder, strFilePattern, recursive);
            }
            if (Directory.GetDirectories(sourceFolder, "*").Length > 0 || Directory.GetFiles(sourceFolder, "*").Length > 0)
                return;
            Directory.Delete(sourceFolder);
        }
        /// <summary>
        /// 拷贝文件夹
        /// </summary>
        /// <param name="sourceFolder">源路径</param>
        /// <param name="destFolder">目标路径</param>
        /// <param name="strFilePattern">文件名匹配的搜索字符串</param>
        public static void CopyFolder(string sourceFolder, string destFolder, string strFilePattern) {
			if (!Directory.Exists(sourceFolder)) return;
            if (!Directory.Exists(destFolder)) Directory.CreateDirectory(destFolder);
            string[] files = Directory.GetFiles(sourceFolder, strFilePattern);
            foreach (string file in files) {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest, true);
            }
            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders) {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest, strFilePattern);
            }
        }
        public static void ClearCacheFiles() {
            m_CacheFiles.Clear();
        }
		/// <summary>
		/// 移动文件夹
		/// </summary>
		/// <param name="sourceFolder">源路径</param>
		/// <param name="destFolder">目标路径</param>
		/// <param name="strFilePattern">文件名匹配的搜索字符串</param>
		public static void MoveFolder(string sourceFolder, string destFolder, string strFilePattern) {
			if (!Directory.Exists(sourceFolder)) return;
			if (!Directory.Exists(destFolder)) Directory.CreateDirectory(destFolder);
			string[] files = Directory.GetFiles(sourceFolder, strFilePattern);
			foreach (string file in files) {
				string name = Path.GetFileName(file);
				string dest = Path.Combine(destFolder, name);
				MoveFile(file, dest, true);
			}
			string[] folders = Directory.GetDirectories(sourceFolder);
			foreach (string folder in folders) {
				string name = Path.GetFileName(folder);
				string dest = Path.Combine(destFolder, name);
				MoveFolder(folder, dest, strFilePattern);
			}
		}
        /// <summary> 获得一个文件的MD5码 </summary>
        public static string GetMD5FromFile(string fileName) {
            return GetMD5FromBuffer(GetFileBuffer(fileName));
        }
        /// <summary> 获得一段字符串的MD5 </summary>
        public static string GetMD5FromString(string buffer) {
            return GetMD5FromBuffer(DefaultEncoding.GetBytes(buffer));
        }
        /// <summary> 根据一段内存获得MD5码 </summary>
        public static string GetMD5FromBuffer(byte[] buffer) {
            if (buffer == null) return null;
			return MD5.GetMd5String (buffer);
        }
    }
}
