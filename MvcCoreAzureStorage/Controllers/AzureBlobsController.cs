using Microsoft.AspNetCore.Mvc;
using MvcCoreAzureStorage.Models;
using MvcCoreAzureStorage.Services;
using System.Threading.Tasks;

namespace MvcCoreAzureStorage.Controllers
{
    public class AzureBlobsController : Controller
    {

        private ServiceStorageBlob service;

        public AzureBlobsController(ServiceStorageBlob service)
        {
            this.service = service;
        }
        public async Task<IActionResult> Index()
        {
            List<string> containers = await this.service.GetContainerAsync();
            return View(containers);
        }

        public IActionResult CreateContainer()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateContainer(string containerName)
        {
            await this.service.CreateContainerAsync(containerName);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteContainer(string containerName)
        {
            await this.service.DeleteContainerAsync(containerName);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ListBlobs(string containerName)
        {
            List<BlobModel> models = await this.service.GetBlobsAsync(containerName);
            return View(models);
        }

        public IActionResult UploadBlob(string containerName)
        {
            ViewData["CONTAINER"] = containerName;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadBlob(string containerName, IFormFile file)
        {
            string blobName = file.FileName;
            using (Stream stream = file.OpenReadStream())
            {
                await this.service.UploadBlobAsync(containerName, blobName, stream);
            }
            return RedirectToAction("ListBlobs", new { containerName = containerName });
        }

        public async Task<IActionResult> DeleteBlob(string containerName, string blobname)
        {
            await this.service.DeleteBlobAsync(containerName, blobname);
            return RedirectToAction("ListBlobs", new { containerName = containerName });
        }

        // NUEVO MÉTODO PARA MOSTRAR LAS IMÁGENES
        public async Task<IActionResult> ViewBlob(string containerName, string blobName)
        {
            Stream stream = await this.service.GetBlobStreamAsync(containerName, blobName);
            return File(stream, "image/jpeg");
        }
    }
}
