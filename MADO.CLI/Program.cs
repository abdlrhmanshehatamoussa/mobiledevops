using CommandLine;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace MADO.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Parser.Default.ParseArguments<DeploymentParameters>(args).WithParsed<DeploymentParameters>(p => {
                    p.Validate();
                    Deploy(p).Wait();
                });
            }catch(Exception e)
            {
                Logger.instance.Log($"Error while parsing arguments: {e.Message}", null, false);
            }
        }

        static async Task Deploy(DeploymentParameters parameters)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            Logger.instance.Log($"Welcome to MADO V{version} ",null,false);
            Logger.instance.LogInfo($"Deployment started [Rebuild={parameters.Rebuild}, show-terminal={parameters.ShowTerminal}]");
            try
            {
                string apkPath = string.Empty;
                apkPath = await BuildEngine.Build(parameters);
                await BuildEngine.Sign(apkPath, parameters);
                await GooglePlayHelper.Instance.Initialize(parameters.CredentialsPath);
                await GooglePlayHelper.Instance.UploadAPK(apkPath,parameters);
            }
            catch (Exception e)
            {
                Logger.instance.LogError(e.Message);
            }   
        }
    }
}
