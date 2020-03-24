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
        public async static Task<bool> DetectCurrentVersion()
        {
            Logger.Log("Detecting current version...", LogLevels.Information);
            bool FoundMatch = false;
            foreach(XmlNode update in XmlEngine.UpdateXML.SelectNodes("/SuperUpdate/Updates/Update"))
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
                    FoundMatch = true;
                    break;
                }
            }
            if(FoundMatch)
            {
                Logger.Log("Detected current version!");
                return true;
            }
            else
            {
                Logger.Log("Could not detect current version!", LogLevels.Warning);
                return false;
            }
        }
        private static SHA1 HashObject = SHA1.Create();
        private static Dictionary<string, string> LocalFiles = new Dictionary<string, string>();
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
