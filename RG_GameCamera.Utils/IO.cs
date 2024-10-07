using System.IO;

namespace RG_GameCamera.Utils;

public static class IO
{
	public static string GetFileName(string absolutPath)
	{
		return Path.GetFileName(absolutPath);
	}

	public static string GetFileNameWithoutExtension(string absolutPath)
	{
		return Path.GetFileNameWithoutExtension(absolutPath);
	}

	public static string GetExtension(string absolutPath)
	{
		return Path.GetExtension(absolutPath).ToLower();
	}

	public static string ReadTextFile(string absolutPath)
	{
		if (File.Exists(absolutPath))
		{
			StreamReader streamReader = new StreamReader(absolutPath);
			string result = streamReader.ReadToEnd();
			streamReader.Close();
			return result;
		}
		return null;
	}

	public static void WriteTextFile(string absolutPath, string content)
	{
		StreamWriter streamWriter = new StreamWriter(absolutPath);
		streamWriter.Write(content);
		streamWriter.Close();
	}

	public static bool CopyFile(string src, string dst, bool overwrite)
	{
		if (File.Exists(src) && (!File.Exists(dst) || overwrite))
		{
			File.Copy(src, dst, overwrite);
			return true;
		}
		return false;
	}

	public static string ConvertFileSeparators(string path)
	{
		return path.Replace("\\", "/");
	}
}
