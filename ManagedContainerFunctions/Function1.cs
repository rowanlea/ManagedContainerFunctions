using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.ContainerInstance;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ManagedContainerFunctions
{
    public class Function1
    {
        IConfiguration _configuration;

        public Function1(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [FunctionName("Start")]
        public async Task<IActionResult> Start(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            ArmClient client = GetClient();

            var containerGroup = client.GetContainerGroupResource(new Azure.Core.ResourceIdentifier("/subscriptions/13181d08-adda-48e7-a1f0-bc5f8e2927b2/resourceGroups/rg-container-management/providers/Microsoft.ContainerInstance/containerGroups/aci-managed-container"));
            await containerGroup.StartAsync(Azure.WaitUntil.Completed);

            return new OkObjectResult("Started ok");
        }

        [FunctionName("Stop")]
        public async Task<IActionResult> Stop(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            ArmClient client = GetClient();

            var containerGroup = client.GetContainerGroupResource(new Azure.Core.ResourceIdentifier("/subscriptions/13181d08-adda-48e7-a1f0-bc5f8e2927b2/resourceGroups/rg-container-management/providers/Microsoft.ContainerInstance/containerGroups/aci-managed-container"));
            await containerGroup.StopAsync();

            return new OkObjectResult("Stopped ok");
        }

        private ArmClient GetClient()
        {
            var tenantId = _configuration["tenantId"];
            var clientId = _configuration["clientId"];
            var clientSecret = _configuration["clientSecret"];

            var credentials = new ClientSecretCredential(tenantId, clientId, clientSecret);

            ArmClient client = new ArmClient(credentials);
            return client;
        }
    }
}
