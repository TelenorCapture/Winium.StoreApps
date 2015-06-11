namespace Winium.StoreApps.Driver.EmulatorHelpers
{
    #region

    using System.Collections.Generic;

    #endregion

    internal interface IDeployer
    {
        #region Public Properties

        string DeviceName { get; }

        #endregion

        #region Public Methods and Operators

        void InstallDependency(string path);

        void Install();

        void Launch();

        void ReciveFiles(Dictionary<string, string> files);

        void SendFiles(Dictionary<string, string> files);

        void Terminate();

        void Uninstall();

        #endregion
    }
}
