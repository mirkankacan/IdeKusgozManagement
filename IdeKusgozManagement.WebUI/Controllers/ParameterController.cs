using IdeKusgozManagement.WebUI.Extensions;
using IdeKusgozManagement.WebUI.Models.ParameterModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{

    [Route(template: "parametre")]
    public class ParameterController : Controller
    {
        private readonly IParameterApiService _parameterApiService;

        public ParameterController(IParameterApiService parameterApiService)
        {
            _parameterApiService = parameterApiService;
        }
        [Authorize(Roles = "Admin, Yönetici")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Admin, Yönetici")]
        [HttpGet("liste")]
        public async Task<IActionResult> GetParameters(CancellationToken cancellationToken)
        {
            var response = await _parameterApiService.GetParametersAsync(cancellationToken);
            return response.ToActionResult();
        }
        [Authorize]
        [HttpGet("{key}")]
        public async Task<IActionResult> GetParameterByKey(string key, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return BadRequest("Parametre ID'si gereklidir");
            }

            var response = await _parameterApiService.GetParameterByKeyAsync(key, cancellationToken);
            return response.ToActionResult();
        }

        //[ValidateAntiForgeryToken]
        //[HttpPost("")]
        //public async Task<IActionResult> CreateParameter([FromBody] CreateParameterViewModel model, CancellationToken cancellationToken)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var response = await _parameterApiService.CreateParameterAsync(model, cancellationToken);
        //    return response.ToActionResult();
        //}
        [Authorize(Roles = "Admin, Yönetici")]
        [ValidateAntiForgeryToken]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateParameter(string id, [FromBody] UpdateParameterViewModel model, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Parametre ID'si gereklidir");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _parameterApiService.UpdateParameterAsync(id, model, cancellationToken);
            return response.ToActionResult();
        }
        //[Authorize(Roles = "Admin, Yönetici")]
        //[ValidateAntiForgeryToken]
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteParameter(string id, CancellationToken cancellationToken)
        //{
        //    if (string.IsNullOrWhiteSpace(id))
        //    {
        //        return BadRequest("Parametre ID'si gereklidir");
        //    }

        //    var response = await _parameterApiService.DeleteParameterAsync(id, cancellationToken);
        //    return response.ToActionResult();
        //}
    }
}