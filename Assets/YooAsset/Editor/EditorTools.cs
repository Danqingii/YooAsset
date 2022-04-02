﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

namespace YooAsset.Editor
{
	/// <summary>
	/// 编辑器工具类
	/// </summary>
	public static class EditorTools
	{
		#region Assembly
		/// <summary>
		/// 调用私有的静态方法
		/// </summary>
		/// <param name="type">类的类型</param>
		/// <param name="method">类里要调用的方法名</param>
		/// <param name="parameters">调用方法传入的参数</param>
		public static object InvokeNonPublicStaticMethod(System.Type type, string method, params object[] parameters)
		{
			var methodInfo = type.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Static);
			if (methodInfo == null)
			{
				UnityEngine.Debug.LogError($"{type.FullName} not found method : {method}");
				return null;
			}
			return methodInfo.Invoke(null, parameters);
		}

		/// <summary>
		/// 调用公开的静态方法
		/// </summary>
		/// <param name="type">类的类型</param>
		/// <param name="method">类里要调用的方法名</param>
		/// <param name="parameters">调用方法传入的参数</param>
		public static object InvokePublicStaticMethod(System.Type type, string method, params object[] parameters)
		{
			var methodInfo = type.GetMethod(method, BindingFlags.Public | BindingFlags.Static);
			if (methodInfo == null)
			{
				UnityEngine.Debug.LogError($"{type.FullName} not found method : {method}");
				return null;
			}
			return methodInfo.Invoke(null, parameters);
		}
		#endregion

		#region EditorUtility
		/// <summary>
		/// 搜集资源
		/// </summary>
		/// <param name="searchType">搜集的资源类型</param>
		/// <param name="searchInFolders">指定搜索的文件夹列表</param>
		/// <returns>返回搜集到的资源路径列表</returns>
		public static string[] FindAssets(EAssetSearchType searchType, string[] searchInFolders)
		{
			// 注意：AssetDatabase.FindAssets()不支持末尾带分隔符的文件夹路径
			for (int i = 0; i < searchInFolders.Length; i++)
			{
				string folderPath = searchInFolders[i];
				searchInFolders[i] = folderPath.TrimEnd('/');
			}

			// 注意：获取指定目录下的所有资源对象（包括子文件夹）
			string[] guids;
			if (searchType == EAssetSearchType.All)
				guids = AssetDatabase.FindAssets(string.Empty, searchInFolders);
			else
				guids = AssetDatabase.FindAssets($"t:{searchType}", searchInFolders);

			// 注意：AssetDatabase.FindAssets()可能会获取到重复的资源
			List<string> result = new List<string>();
			for (int i = 0; i < guids.Length; i++)
			{
				string guid = guids[i];
				string assetPath = AssetDatabase.GUIDToAssetPath(guid);
				if (result.Contains(assetPath) == false)
				{
					result.Add(assetPath);
				}
			}

			// 返回结果
			return result.ToArray();
		}

		/// <summary>
		/// 搜集资源
		/// </summary>
		/// <param name="searchType">搜集的资源类型</param>
		/// <param name="searchInFolder">指定搜索的文件夹</param>
		/// <returns>返回搜集到的资源路径列表</returns>
		public static string[] FindAssets(EAssetSearchType searchType, string searchInFolder)
		{
			return FindAssets(searchType, new string[] { searchInFolder });
		}

		/// <summary>
		/// 打开搜索面板
		/// </summary>
		/// <param name="title">标题名称</param>
		/// <param name="defaultPath">默认搜索路径</param>
		/// <returns>返回选择的文件夹绝对路径，如果无效返回NULL</returns>
		public static string OpenFolderPanel(string title, string defaultPath, string defaultName = "")
		{
			string openPath = EditorUtility.OpenFolderPanel(title, defaultPath, defaultName);
			if (string.IsNullOrEmpty(openPath))
				return null;

			if (openPath.Contains("/Assets") == false)
			{
				Debug.LogWarning("Please select unity assets folder.");
				return null;
			}
			return openPath;
		}

		/// <summary>
		/// 打开搜索面板
		/// </summary>
		/// <param name="title">标题名称</param>
		/// <param name="defaultPath">默认搜索路径</param>
		/// <returns>返回选择的文件绝对路径，如果无效返回NULL</returns>
		public static string OpenFilePath(string title, string defaultPath, string extension = "")
		{
			string openPath = EditorUtility.OpenFilePanel(title, defaultPath, extension);
			if (string.IsNullOrEmpty(openPath))
				return null;

			if (openPath.Contains("/Assets") == false)
			{
				Debug.LogWarning("Please select unity assets file.");
				return null;
			}
			return openPath;
		}

		/// <summary>
		/// 显示进度框
		/// </summary>
		public static void DisplayProgressBar(string tips, int progressValue, int totalValue)
		{
			EditorUtility.DisplayProgressBar("进度", $"{tips} : {progressValue}/{totalValue}", (float)progressValue / totalValue);
		}

		/// <summary>
		/// 隐藏进度框
		/// </summary>
		public static void ClearProgressBar()
		{
			EditorUtility.ClearProgressBar();
		}
		#endregion

		#region EditorWindow
		public static void FocusUnitySceneWindow()
		{
			EditorWindow.FocusWindowIfItsOpen<SceneView>();
		}
		public static void FocusUnityGameWindow()
		{
			System.Type T = Assembly.Load("UnityEditor").GetType("UnityEditor.GameView");
			EditorWindow.GetWindow(T, false, "GameView", true);
		}
		public static void FocueUnityProjectWindow()
		{
			System.Type T = Assembly.Load("UnityEditor").GetType("UnityEditor.ProjectBrowser");
			EditorWindow.GetWindow(T, false, "Project", true);
		}
		public static void FocusUnityHierarchyWindow()
		{
			System.Type T = Assembly.Load("UnityEditor").GetType("UnityEditor.SceneHierarchyWindow");
			EditorWindow.GetWindow(T, false, "Hierarchy", true);
		}
		public static void FocusUnityInspectorWindow()
		{
			System.Type T = Assembly.Load("UnityEditor").GetType("UnityEditor.InspectorWindow");
			EditorWindow.GetWindow(T, false, "Inspector", true);
		}
		public static void FocusUnityConsoleWindow()
		{
			System.Type T = Assembly.Load("UnityEditor").GetType("UnityEditor.ConsoleWindow");
			EditorWindow.GetWindow(T, false, "Console", true);
		}
		#endregion

		#region 控制台
		private static MethodInfo _clearConsoleMethod;
		private static MethodInfo ClearConsoleMethod
		{
			get
			{
				if (_clearConsoleMethod == null)
				{
					Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
					System.Type logEntries = assembly.GetType("UnityEditor.LogEntries");
					_clearConsoleMethod = logEntries.GetMethod("Clear");
				}
				return _clearConsoleMethod;
			}
		}

		/// <summary>
		/// 清空控制台
		/// </summary>
		public static void ClearUnityConsole()
		{
			ClearConsoleMethod.Invoke(new object(), null);
		}
		#endregion

		#region 文件
		/// <summary>
		/// 创建文件所在的目录
		/// </summary>
		/// <param name="filePath">文件路径</param>
		public static void CreateFileDirectory(string filePath)
		{
			string destDirectory = Path.GetDirectoryName(filePath);
			CreateDirectory(destDirectory);
		}

		/// <summary>
		/// 创建文件夹
		/// </summary>
		public static bool CreateDirectory(string directory)
		{
			if (Directory.Exists(directory) == false)
			{
				Directory.CreateDirectory(directory);
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// 删除文件夹及子目录
		/// </summary>
		public static bool DeleteDirectory(string directory)
		{
			if (Directory.Exists(directory))
			{
				Directory.Delete(directory, true);
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// 文件重命名
		/// </summary>
		public static void FileRename(string filePath, string newName)
		{
			string dirPath = Path.GetDirectoryName(filePath);
			string destPath;
			if (Path.HasExtension(filePath))
			{
				string extentsion = Path.GetExtension(filePath);
				destPath = $"{dirPath}/{newName}{extentsion}";
			}
			else
			{
				destPath = $"{dirPath}/{newName}";
			}
			FileInfo fileInfo = new FileInfo(filePath);
			fileInfo.MoveTo(destPath);
		}

		/// <summary>
		/// 文件移动
		/// </summary>
		public static void FileMoveTo(string filePath, string destPath)
		{
			FileInfo fileInfo = new FileInfo(filePath);
			fileInfo.MoveTo(destPath);
		}

		/// <summary>
		/// 拷贝文件夹
		/// 注意：包括所有子目录的文件
		/// </summary>
		public static void CopyDirectory(string sourcePath, string destPath)
		{
			sourcePath = EditorTools.GetRegularPath(sourcePath);

			// If the destination directory doesn't exist, create it.
			if (Directory.Exists(destPath) == false)
				Directory.CreateDirectory(destPath);

			string[] fileList = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
			foreach (string file in fileList)
			{
				string temp = EditorTools.GetRegularPath(file);
				string savePath = temp.Replace(sourcePath, destPath);
				CopyFile(file, savePath, true);
			}
		}

		/// <summary>
		/// 拷贝文件
		/// </summary>
		public static void CopyFile(string sourcePath, string destPath, bool overwrite)
		{
			if (File.Exists(sourcePath) == false)
				throw new FileNotFoundException(sourcePath);

			// 创建目录
			CreateFileDirectory(destPath);

			// 复制文件
			File.Copy(sourcePath, destPath, overwrite);
		}

		/// <summary>
		/// 清空文件夹
		/// </summary>
		/// <param name="folderPath">要清理的文件夹路径</param>
		public static void ClearFolder(string directoryPath)
		{
			if (Directory.Exists(directoryPath) == false)
				return;

			// 删除文件
			string[] allFiles = Directory.GetFiles(directoryPath);
			for (int i = 0; i < allFiles.Length; i++)
			{
				File.Delete(allFiles[i]);
			}

			// 删除文件夹
			string[] allFolders = Directory.GetDirectories(directoryPath);
			for (int i = 0; i < allFolders.Length; i++)
			{
				Directory.Delete(allFolders[i], true);
			}
		}

		/// <summary>
		/// 获取文件字节大小
		/// </summary>
		public static long GetFileSize(string filePath)
		{
			FileInfo fileInfo = new FileInfo(filePath);
			return fileInfo.Length;
		}

		/// <summary>
		/// 读取文件的所有文本内容
		/// </summary>
		public static string ReadFileAllText(string filePath)
		{
			if (File.Exists(filePath) == false)
				return string.Empty;

			return File.ReadAllText(filePath, Encoding.UTF8);
		}

		/// <summary>
		/// 读取文本的所有文本内容
		/// </summary>
		public static string[] ReadFileAllLine(string filePath)
		{
			if (File.Exists(filePath) == false)
				return null;

			return File.ReadAllLines(filePath, Encoding.UTF8);
		}

		/// <summary>
		/// 检测AssetBundle文件是否合法
		/// </summary>
		public static bool CheckBundleFileValid(byte[] fileData)
		{
			string signature = ReadStringToNull(fileData, 20);
			if (signature == "UnityFS" || signature == "UnityRaw" || signature == "UnityWeb" || signature == "\xFA\xFA\xFA\xFA\xFA\xFA\xFA\xFA")
				return true;
			else
				return false;
		}
		private static string ReadStringToNull(byte[] data, int maxLength)
		{
			List<byte> bytes = new List<byte>();
			for (int i = 0; i < data.Length; i++)
			{
				if (i >= maxLength)
					break;

				byte bt = data[i];
				if (bt == 0)
					break;

				bytes.Add(bt);
			}

			if (bytes.Count == 0)
				return string.Empty;
			else
				return Encoding.UTF8.GetString(bytes.ToArray());
		}
		#endregion

		#region 路径
		private static string YooAssetPath;

		/// <summary>
		/// 获取规范的路径
		/// </summary>
		public static string GetRegularPath(string path)
		{
			return path.Replace('\\', '/').Replace("\\", "/"); //替换为Linux路径格式
		}

		/// <summary>
		/// 获取资源框架目录路径
		/// </summary>
		public static string GetYooAssetPath()
		{
			if (string.IsNullOrEmpty(YooAssetPath) == false)
				return YooAssetPath;

			string packagesPath = ("Packages/com.tuyoo.yooasset/README.md");
			var obj = AssetDatabase.LoadAssetAtPath(packagesPath, typeof(TextAsset));
			if(obj != null)
			{
				YooAssetPath = "Packages/com.tuyoo.yooasset/";
				return YooAssetPath;
			}

			string[] allDirectorys = Directory.GetDirectories(Application.dataPath, "YooAsset", SearchOption.AllDirectories);
			if (allDirectorys.Length == 0)
			{
				Debug.LogError("Not found YooAsset Folder!");
				return string.Empty;
			}
			if (allDirectorys.Length > 1)
			{
				Debug.LogError("Found multiple YooAsset Folders!");
				return string.Empty;
			}
			YooAssetPath = AbsolutePathToAssetPath(allDirectorys[0]);
			return YooAssetPath;
		}

		/// <summary>
		/// 获取项目工程路径
		/// </summary>
		public static string GetProjectPath()
		{
			string projectPath = Path.GetDirectoryName(Application.dataPath);
			return GetRegularPath(projectPath);
		}

		/// <summary>
		/// 转换文件的绝对路径为Unity资源路径
		/// 例如 D:\\YourPorject\\Assets\\Works\\file.txt 替换为 Assets/Works/file.txt
		/// </summary>
		public static string AbsolutePathToAssetPath(string absolutePath)
		{
			string content = GetRegularPath(absolutePath);
			return Substring(content, "Assets", true);
		}

		/// <summary>
		/// 转换Unity资源路径为文件的绝对路径
		/// 例如：Assets/Works/file.txt 替换为 D:\\YourPorject/Assets/Works/file.txt
		/// </summary>
		public static string AssetPathToAbsolutePath(string assetPath)
		{
			string projectPath = GetProjectPath();
			return $"{projectPath}/{assetPath}";
		}

		/// <summary>
		/// 递归查找目标文件夹路径
		/// </summary>
		/// <param name="root">搜索的根目录</param>
		/// <param name="folderName">目标文件夹名称</param>
		/// <returns>返回找到的文件夹路径，如果没有找到返回空字符串</returns>
		public static string FindFolder(string root, string folderName)
		{
			DirectoryInfo rootInfo = new DirectoryInfo(root);
			DirectoryInfo[] infoList = rootInfo.GetDirectories();
			for (int i = 0; i < infoList.Length; i++)
			{
				string fullPath = infoList[i].FullName;
				if (infoList[i].Name == folderName)
					return fullPath;

				string result = FindFolder(fullPath, folderName);
				if (string.IsNullOrEmpty(result) == false)
					return result;
			}
			return string.Empty;
		}
		#endregion

		#region 字符串
		/// <summary>
		/// 是否含有中文
		/// </summary>
		public static bool IncludeChinese(string content)
		{
			foreach (var c in content)
			{
				if (c >= 0x4e00 && c <= 0x9fbb)
					return true;
			}
			return false;
		}

		/// <summary>
		/// 是否是数字
		/// </summary>
		public static bool IsNumber(string content)
		{
			if (string.IsNullOrEmpty(content))
				return false;
			string pattern = @"^\d*$";
			return Regex.IsMatch(content, pattern);
		}

		/// <summary>
		/// 首字母大写
		/// </summary>
		public static string Capitalize(string content)
		{
			return content.Substring(0, 1).ToUpper() + (content.Length > 1 ? content.Substring(1).ToLower() : "");
		}

		/// <summary>
		/// 截取字符串
		/// 获取匹配到的后面内容
		/// </summary>
		/// <param name="content">内容</param>
		/// <param name="key">关键字</param>
		/// <param name="includeKey">分割的结果里是否包含关键字</param>
		/// <param name="searchBegin">是否使用初始匹配的位置，否则使用末尾匹配的位置</param>
		public static string Substring(string content, string key, bool includeKey, bool firstMatch = true)
		{
			if (string.IsNullOrEmpty(key))
				return content;

			int startIndex = -1;
			if (firstMatch)
				startIndex = content.IndexOf(key); //返回子字符串第一次出现位置		
			else
				startIndex = content.LastIndexOf(key); //返回子字符串最后出现的位置

			// 如果没有找到匹配的关键字
			if (startIndex == -1)
				return content;

			if (includeKey)
				return content.Substring(startIndex);
			else
				return content.Substring(startIndex + key.Length);
		}
		#endregion

		#region 玩家偏好
		// 枚举
		public static void PlayerSetEnum<T>(string key, T value)
		{
			string enumName = value.ToString();
			EditorPrefs.SetString(key, enumName);
		}
		public static T PlayerGetEnum<T>(string key, T defaultValue)
		{
			string enumName = EditorPrefs.GetString(key, defaultValue.ToString());
			return StringUtility.NameToEnum<T>(enumName);
		}
		#endregion
	}
}