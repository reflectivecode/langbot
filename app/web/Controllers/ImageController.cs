using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LangBot.Web.Models;
using LangBot.Web.Services;
using LangBot.Web.Slack;

namespace LangBot.Web.Controllers
{
    [Route("api/[controller]")]
    public class ImageController : Controller
    {
        private readonly RenderService _service;
        private readonly ImageUtility _imageUtility;

        public ImageController(RenderService service, ImageUtility imageUtility)
        {
            _service = service;
            _imageUtility = imageUtility;
        }

        [HttpGet]
        [ResponseCache(Duration = 60 * 60 * 24)]
        public async Task<IActionResult> Get([FromQuery] ImageRequest request)
        {
            if (_imageUtility.ImageHash(request.Image) != request.Hash) throw new SlackException("Invalid validation hash");
            var model = _imageUtility.DeserializeImage(request.Image);
            var image = await _service.Render(model);
            return File(image, "image/jpeg");
        }
    }
}
