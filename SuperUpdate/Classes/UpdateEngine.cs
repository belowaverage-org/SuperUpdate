using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SuperUpdate.Log;
using SuperUpdate.Xml;
using System.Xml;
using System.Security.Cryptography;
using System.IO;

namespace SuperUpdate.Update
{
    class UpdateEngine
    {
        public static XmlNode CurrentVersion = null;
        public static XmlNode LatestVersion = null;
        public static List<string> AvailableChannels = new List<string>();
        private static SHA1 HashObject = SHA1.Create();
        private static Dictionary<string, string> LocalFiles = new Dictionary<string, string>();
        public async static Task<bool> DetectUpdates()
        {
            AvailableChannels.Clear();
            CurrentVersion = LatestVersion = null;
            Logger.Log("Detecting current version...", LogLevels.Information);
            XmlNodeList updates = XmlEngine.UpdateXML.SelectNodes("/SuperUpdate/Updates/Update");
            foreach (XmlNode update in updates)
            {
                bool ThisUpdateMatches = true;
                foreach(XmlNode file in update.SelectNodes("File"))
                {
                    if(await GetHashOfFile(file.Attributes["Path"].Value) != file.Attributes["SHA1"].Value)
                    {
                        ThisUpdateMatches = false;
                        break;
                    }
                }
                if (ThisUpdateMatches)
                {
                    Logger.Log("Detected current version! Channel: " + update.Attributes["Channel"].Value + ", Version: " + update.Attributes["Version"].Value + ".");
                    CurrentVersion = update;
                    break;
                }
            }
            Logger.Log("Detecting latest version...", LogLevels.Information);
            foreach (XmlNode update in updates)
            {
                if (update.Attributes["Channel"].Value == CurrentVersion.Attributes["Channel"].Value)
                {
                    Logger.Log("Detected latest version! Channel: " + update.Attributes["Channel"].Value + ", Version: " + update.Attributes["Version"].Value + ".");
                    LatestVersion = update;
                    break;
                }
            }
            foreach (XmlNode update in updates)
            {
                string channel = update.Attributes["Channel"].Value;
                if (!AvailableChannels.Contains(channel)) AvailableChannels.Add(channel);
            }
            if (CurrentVersion == null || LatestVersion == null)
            {
                Logger.Log("Could not detect current / latest version!", LogLevels.Warning);
                return false;
            }
            return true;
        }
        private static Task<string> GetHashOfFile(string Path)
        {
            if(LocalFiles.ContainsKey(Path))
            {
                return Task.FromResult(LocalFiles[Path]);
            }
            return Task.Run(() => {
                Logger.Log("Computing hash for: " + Path + "...");
                if (File.Exists(Path))
                {
                    byte[] rawHash = HashObject.ComputeHash(File.OpenRead(Path));
                    string hash = BitConverter.ToString(rawHash).Replace("-", "");
                    LocalFiles.Add(Path, hash);
                    return hash;
                }
                Logger.Log("File not found!", LogLevels.Warning);
                return string.Empty;
            });
        }
    }
}
