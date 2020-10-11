using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MADO.CLI
{
    public class DeploymentParameters
    {
        public string BaseDirectory = @"F:\My Projects\RubikCalc\src\rubikcalc";
        public string ApkTargetDirectory = @"C:\Users\AbdlrhmanShehata\Desktop";
        public string PackageName = "com.programmablecalculator.rubikcalcultra";
        public string ReleaseName = "rubikcalcultra";
        public string KeystoreFilePath = @"C:\Users\AbdlrhmanShehata\OneDrive\4.Mobile App Business\Applications Management\keystores\wisebay.programmablecalculator.pro.jks";
        public string KeystoreAlias = "rubik123";
        public string KeystoreType = "jks";
        public string KeystorePassword = "rubik123";
        public string KeyPassword = "rubik123";
        public string versionName = "1.3.1";
        public string versionCode = "6";
        public string CredentialsPath = @"C:\Users\AbdlrhmanShehata\OneDrive\4.Mobile App Business\Applications Management\client_secret_801942528673-iso7ja1i2gtoh78fjsn0qar4pltvavf2.apps.googleusercontent.com.json";
        //[production,beta,alpha]
        public string TrackName = "alpha";
        //[completed,draft]
        public string ReleaseStatus = "draft";
        public bool KeepTerminal = false;
        public bool ShowTerminal = false;
        public bool Rebuild = false;
    }
}
