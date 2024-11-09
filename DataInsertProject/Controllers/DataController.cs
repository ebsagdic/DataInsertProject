using DataInsertProject.Models;
using DataInsertProject.Service;
using Microsoft.AspNetCore.Mvc;

namespace DataInsertProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly DataService _dataService;

        public DataController(DataService dataService)
        {
            _dataService = dataService;
        }

        [HttpPost]
        public async Task<IActionResult> InsertData([FromBody] List<DataModel> dataModels)
        {
            await _dataService.ProcessDataAsync(dataModels);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            var data = await _dataService.GetCachedDataAsync();
            return Ok(data);
        }
    }
}
