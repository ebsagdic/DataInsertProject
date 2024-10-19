using DataInsertProject.Models;
using DataInsertProject.Service;
using Microsoft.AspNetCore.Mvc;

namespace DataInsertProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataInsertController : ControllerBase
    {
        private readonly DataService _dataService;

        public DataInsertController(DataService dataService)
        {
            _dataService = dataService;
        }

        [HttpPost]
        public async Task<IActionResult> InsertData([FromBody] List<DataModel> dataModels)
        {
            await _dataService.ProcessDataAsync(dataModels);
            return Ok();
        }
    }
}
