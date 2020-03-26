using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Management.Automation;
using SuperUpdate.Xml;
using SuperUpdate.Log;

namespace SuperUpdate.Install
{
    class InstallEngine
    {
        private static PowerShell PS = PowerShell.Create();
        public static Task<bool> InstallUpdate(XmlNode UpdateNode)
        {
            return Task.Run(() => {
                Logger.Log("YEE");
                return true;
            });
        }
    }
}
