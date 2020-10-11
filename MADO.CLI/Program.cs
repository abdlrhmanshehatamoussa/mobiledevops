using System;
using System.Threading.Tasks;

namespace MADO.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            DeploymentParameters parameters = new DeploymentParameters();
            Deploy(parameters).Wait();
            Console.ReadLine();
        }

        static async Task Deploy(DeploymentParameters parameters)
        {
            Logger.instance.LogMessage($"Deployment started [Rebuild={parameters.Rebuild}, keep-terminal={parameters.KeepTerminal}, show-terminal={parameters.ShowTerminal}]");
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
