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
        private PowerShell PS = PowerShell.Create();
        public InstallEngine()
        {
            PS.Streams.Debug.DataAdded += PS_DataAdded;
            PS.Streams.Error.DataAdded += PS_DataAdded;
            PS.Streams.Warning.DataAdded += PS_DataAdded;
            PS.Streams.Verbose.DataAdded += PS_DataAdded;
            PS.Streams.Information.DataAdded += PS_DataAdded;
            PS.Streams.Progress.DataAdded += PS_DataAdded;
            //PS.
        }

        private void PS_DataAdded(object sender, DataAddedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InstallUpdate(XmlNode UpdateNode)
        {
            return Task.Run(() => {
                //PS.
                return true;
            });
        }
    }
}
