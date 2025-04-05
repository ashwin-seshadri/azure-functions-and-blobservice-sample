using AzureBlobProject.Models;
using AzureBlobProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobProject.Controllers
{
    public class ContainerController : Controller
    {
        private readonly IContainerService _containerService;

        public ContainerController(IContainerService containerService)
        {
            this._containerService = containerService;
        }

        public async Task<IActionResult> Index()
        {
            var allContainers = await this._containerService.GetAllContainers();
            return View(allContainers);
        }

        public async Task<IActionResult> Delete(string containerName)
        {
            await this._containerService.DeleteContainer(containerName);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Create()
        {
            return View(new Container());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Container container)
        {
            await this._containerService.CreateContainer(container.Name);
            return RedirectToAction(nameof(Index));
        }
    }
}
