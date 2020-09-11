using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SuperUpdate.Log;
using System.Xml;
using System.Security.Cryptography;
using System.IO;

namespace SuperUpdate.Engines
{
    /// <summary>
    /// This class determines what version the client software is on, and what the latest update is.
    /// </summary>
    public class UpdateEngine
    {
        /// <summary>
        /// This property contains the current version XmlNode of the client software.
        /// </summary>
        public static XmlNode CurrentVersion = null;
        /// <summary>
        /// This property contains the most up-to-date XmlNode of the client software.
        /// </summary>
        public static XmlNode LatestVersion = null;
        /// <summary>
        /// This property contains a list of available update channels.
        /// </summary>
        public static List<string> AvailableChannels = new List<string>();
        private static readonly SHA1 HashObject = SHA1.Create();
        private static readonly Dictionary<string, string> LocalFiles = new Dictionary<string, string>();
        /// <summary>
        /// This method will detect and determine the current software version, and latest update available.
        /// </summary>
        /// <returns>Task: Bool: Returns true on success.</returns>
        public async static Task<bool> DetectUpdates()
        {
            AvailableChannels.Clear();
            CurrentVersion = LatestVersion = null;
            Logger.Log("Detecting current version...", LogLevels.Information);
            XmlNodeList updates = XmlEngine.UpdateXML.SelectNodes("/SU:SuperUpdate/SU:Updates/SU:Update", XmlEngine.XNS);
            foreach (XmlNode update in updates)
            {
                bool ThisUpdateMatches = true;
                foreach(XmlNode file in update.SelectNodes("SU:File", XmlEngine.XNS))
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
            if (CurrentVersion == null)
            {
                Logger.Log("Could not detect the current version!", LogLevels.Warning);
                return false;
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
                LocalFiles.Add(Path, "");
                return string.Empty;
            });
        }
    }
}
