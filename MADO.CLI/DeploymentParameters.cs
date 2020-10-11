using CommandLine;
using System;
using System.IO;

namespace MADO.CLI
{
    public class DeploymentParameters
    {
        [Option("BaseDirectory", Required = true, HelpText = "Base directory of the project.")]
        public string BaseDirectory { get; set; }

        [Option("PackageName", Required = true, HelpText = "Package name of the application")]
        public string PackageName { get; set; }

        [Option("ReleasePrefix", Required = true, HelpText = "Release name prefix to be attached to the full name")]
        public string ReleasePrefix { get; set; }

        [Option("KeystoreFilePath", Required = true, HelpText = "Release name prefix to be attached to the full name")]
        public string KeystoreFilePath { get; set; }

        [Option("KeystoreAlias", Required = true, HelpText = "Keystore alias")]
        public string KeystoreAlias { get; set; }

        [Option("KeystoreType", Required = false, HelpText = "Keystore type (jks)",Default ="jks")]
        public string KeystoreType { get; set; }

        [Option("KeystorePassword", Required = true, HelpText = "Password of the store")]
        public string KeystorePassword { get; set; }

        [Option("KeyPassword", Required = true, HelpText = "Password of the key")]
        public string KeyPassword { get; set; }

        [Option("ApkTargetDirectory", Required = true, HelpText = "Target directory ")]
        public string ApkTargetDirectory { get; set; }

        [Option("VersionName", Required = true, HelpText = "")]
        public string VersionName { get; set; }

        [Option("VersionCode", Required = true, HelpText = "")]
        public string VersionCode { get; set; }

        [Option("CredentialsPath", Required = true, HelpText = "")]
        public string CredentialsPath { get; set; }

        [Option("TrackName", Required = true, HelpText = "Name of the track on GooglePlay [production,alpha,beta]")]
        //[production,beta,alpha]
        public string TrackName { get; set; }

        [Option("ReleaseStatus", Required = true, HelpText = "Status of the release to be deployed [draft,completed]")]
        //[completed,draft]
        public string ReleaseStatus { get; set; }

        [Option('t',"ShowTerminal", Required = false, HelpText = "Show the terminal")]
        public bool? ShowTerminal { get; set; }

        [Option('r',"Rebuild", Required = false, HelpText = "Rebuild the project")]
        public bool? Rebuild { get; set; }

        public void Validate()
        {
            if (!Directory.Exists(BaseDirectory))
            {
                throw new Exception("Couldn't locate the project folder");
            }

            int versionCode = -1;
            if (!int.TryParse(VersionCode, out versionCode))
            {
                throw new Exception("Cannot parse version code");
            }

            if (!Directory.Exists(ApkTargetDirectory))
            {
                throw new Exception("APK target directory doesn't exist");
            }

            if (!File.Exists(KeystoreFilePath))
            {
                throw new Exception("Keystore file not found");
            }
        }
    }
}
