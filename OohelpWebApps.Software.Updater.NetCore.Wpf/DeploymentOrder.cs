
namespace OohelpWebApps.Software.Updater;
internal sealed class DeploymentOrder
{
    public UpdateCommand Command {  get; set; }
    public bool StartApplicationAfterDeployment { get; set; }

    public static DeploymentOrder Immediately => new DeploymentOrder { Command = UpdateCommand.UpdateImmediately, StartApplicationAfterDeployment = true };
    public static DeploymentOrder Quietly => new DeploymentOrder { Command = UpdateCommand.UpdateQuietly, StartApplicationAfterDeployment = false };
    public static DeploymentOrder NoUpdate => new DeploymentOrder { Command = UpdateCommand.NoUpdate, StartApplicationAfterDeployment = false };
}
