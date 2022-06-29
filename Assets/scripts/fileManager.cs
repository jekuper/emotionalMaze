using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class fileManager
{
    public static bool fileExist(string fileId)
    {
        string path = Application.persistentDataPath + "/" + fileId + ".save";
        return File.Exists(path);
    }
    public static void fileCreate(string fileId)
    {
        string path = Application.persistentDataPath + "/" + fileId + ".save";
        File.Create(path);
    }
    public static void DeleteFile(string fileId)
    {
        string path = Application.persistentDataPath + "/" + fileId + ".save";
        File.Delete(path);
    }
    public static void SaveId(string fileId, string data, bool append = false) {
        string path = Application.persistentDataPath + "/" + fileId + ".save";
        if (append)
        {
            File.AppendAllText(path, data + "\n");
        }
        else
        {
            data = "version=" + Application.version + "\n" + data;
            File.WriteAllText(path, data);
        }
    }
    public static string LoadFileVersion(string fileId) {
        string path = Application.persistentDataPath + "/" + fileId + ".save";
        if (!File.Exists(path)) {
            return null;
        }
        string res = File.ReadAllLines(@path).First();
        if (res.Length > 8 && res.Substring(0, 8) == "version=") {
            return res.Substring(8);
        }
        return null;
    }
    public static string[] LoadId(string fileId) {
        string path = Application.persistentDataPath + "/" + fileId + ".save";
        if (!File.Exists(path)) {
            string[] res = new string[1];
            res[0] = "{}";
            return res;
        }
        string[] lines = File.ReadAllLines(@path);
        if (lines.Length > 0 && lines[0].Length > 8 && lines[0].Substring(0, 8) == "version=") {
            List<string> tl = new List<string>(lines);
            tl.RemoveAt(0);
            lines = tl.ToArray();
        }
        return lines;
    }
    public static string ConnectLines(string[] data, string separator) {
        string s = "";
        foreach (string item in data) {
            s += item;
            s += separator;
        }
        return s;
    } 
}
