using Google.Apis.AndroidPublisher.v3.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace MADO.CLI
{
    public class BuildEngine
    {
        /*Constants
         ==========================================================*/
        #region Constants
        /// <summary>
        /// Android Project Directory, Drive Letter
        /// </summary>
        private const string SCRIPT_BUILD_FLUTTER = "pushd \"{0}\"\ncall flutter clean\ncall flutter build apk --release";
        private const string APK_RELEASE_DIRECTORY_FLUTTER = @"\build\app\outputs\apk\release";
        private const string APK_NAME_FLUTTER = "app-release.apk";

        /// <summary>
        /// Android Project Directory, Drive Letter
        /// </summary>
        private const string SCRIPT_BUILD_IONIC = "pushd \"{0}\"\ncall ionic cordova build android --prod --release";
        private const string APK_RELEASE_DIRECTORY_IONIC = @"\platforms\android\app\build\outputs\apk\release";
        private const string APK_NAME_IONIC = "app-release.apk";


        /// <summary>
        /// Android Project Directory, Drive Letter
        /// </summary>
        private const string SCRIPT_BUILD_NATIVE = "pushd \"{0}\"\ncall gradlew.bat assembleRelease";
        private const string APK_RELEASE_DIRECTORY_NATIVE = @"\app\build\outputs\apk\release";
        private const string APK_NAME_NATIVE = "app-release-unsigned.apk";


        /// <summary>
        /// Apk Path, Keystore Path,Keystore Type, Keystore Password, Key Password, Key Alias
        /// </summary>
        private const string SCRIPT_SIGN = "call zipalign -c -v 4 \"{0}\"\ncall apksigner sign --ks \"{1}\" --ks-type {2} --ks-pass pass:{3} --key-pass pass:{4} --ks-key-alias {5} \"{0}\"";
        #endregion



        public static async Task<string> Build(DeploymentParameters parameters)
        {
            Logger.instance.LogInfo($"Generating apk ...");
            string buildScript = string.Empty;
            string apkReleaseDirectory = string.Empty;
            string apkName = string.Empty;

            //Determine project type
            string technology = string.Empty;
            string flutter_check_path = Path.Combine(parameters.BaseDirectory, "pubspec.yaml");
            string ionic_check_path = Path.Combine(parameters.BaseDirectory, "ionic.config.json");
            string native_check_path = Path.Combine(parameters.BaseDirectory, "build.gradle");
            if (File.Exists(flutter_check_path))
            {
                apkReleaseDirectory = APK_RELEASE_DIRECTORY_FLUTTER;
                apkName = APK_NAME_FLUTTER;
                buildScript = SCRIPT_BUILD_FLUTTER;
            }
            if (File.Exists(ionic_check_path))
            {
                buildScript = SCRIPT_BUILD_IONIC;
                apkName = APK_NAME_IONIC;
                apkReleaseDirectory = APK_RELEASE_DIRECTORY_IONIC;
            }
            if (File.Exists(native_check_path))
            {
                buildScript = SCRIPT_BUILD_NATIVE;
                apkName = APK_NAME_NATIVE;
                apkReleaseDirectory = APK_RELEASE_DIRECTORY_NATIVE;
            }
            if (string.IsNullOrWhiteSpace(buildScript))
            {
                throw new Exception("Cannot determine project type");
            }

            //Creating Build Script
            string buildScriptPath = Path.Combine(Path.GetTempPath(), $"build_{Guid.NewGuid()}.bat");
            string buildFileContent = string.Format(buildScript, parameters.BaseDirectory);
            File.WriteAllText(buildScriptPath, buildFileContent);


            //Generating APK
            if (parameters.Rebuild != null && parameters.Rebuild.Value == true)
            {
                Process proc = new Process();
                proc.StartInfo.FileName = buildScriptPath;
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                if (parameters.ShowTerminal.HasValue && parameters.ShowTerminal == true)
                {
                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                }
                await Task.Run(() =>
                {
                    proc.Start();
                    proc.WaitForExit();
                });
            }
            else
            {
                Logger.instance.LogInfo($"Build step has been skipped");
            }
            string timestamp = DateTime.Now.ToString("yyyy.MM.dd.hh.mm.ss");
            string apkTargetName = $"{parameters.PackageName}.VC{parameters.VersionCode}.V{parameters.VersionName}.{timestamp}.apk";
            string outputFileName = $"{parameters.PackageName}.VC{parameters.VersionCode}.V{parameters.VersionName}.{timestamp}.json";
            string apkTargetPath = Path.Combine(parameters.ApkTargetDirectory, apkTargetName);
            string apkSourcePath = parameters.BaseDirectory + apkReleaseDirectory + @"\" + apkName;
            string outputFilePath = parameters.BaseDirectory + apkReleaseDirectory + @"\output.json";
            string outputTargetPath = Path.Combine(parameters.ApkTargetDirectory, outputFileName);
            if (File.Exists(apkSourcePath) && File.Exists(outputFilePath))
            {
                if (!Directory.Exists(parameters.ApkTargetDirectory))
                {
                    throw new Exception($"Directory [{parameters.ApkTargetDirectory}] doesn't exist");
                }
                File.Copy(apkSourcePath, apkTargetPath);
                File.Copy(outputFilePath, outputTargetPath);
            }
            else
            {
                throw new Exception($"Failed to locate the generated apk or the outputfile -> [{apkSourcePath}]");
            }
            Logger.instance.LogInfo($"Apk generated successfully at [{apkTargetPath}]");
            return apkTargetPath;
        }

        public static async Task Sign(string apkPath, DeploymentParameters parameters)
        {
            Logger.instance.LogInfo($"Signing apk ...");
            if (!File.Exists(apkPath))
            {
                throw new Exception($"Failed to locate the apk at [{apkPath}]");
            }
            //Sign APK
            string signScript = SCRIPT_SIGN;
            string signContent = string.Format(signScript,
                apkPath,
                parameters.KeystoreFilePath,
                parameters.KeystoreType,
                parameters.KeystorePassword,
                parameters.KeyPassword,
                parameters.KeystoreAlias
                );
            string signScriptPath = Path.Combine(Path.GetTempPath(), $"sign_{Guid.NewGuid()}.bat");
            File.WriteAllText(signScriptPath, signContent);
            if (!File.Exists(signScriptPath))
            {
                throw new FileNotFoundException($"Script file not found '{signScriptPath}'");
            }
            Process proc = new Process();
            proc.StartInfo.FileName = signScriptPath;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            if (parameters.ShowTerminal.HasValue && parameters.ShowTerminal == true)
            {
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            }
            await Task.Run(() =>
            {
                proc.Start();
                proc.WaitForExit();
            });
            File.Delete(signScriptPath);
            Logger.instance.LogInfo("APK signed successfully");
        }


    }
}