using AutoMapper;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;
using WebService.Models.ViewModels.RepairGroup;

namespace WebService.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RepairGroupApiController : ControllerBase
    {
        private readonly RepairGroupService _repairGroupService;

        private readonly IMapper _mapper;

        public RepairGroupApiController(IMapper mapper, RepairGroupService repairGroupService)
        {
            _repairGroupService = repairGroupService;
            _mapper = mapper;
        }

        public async Task<IActionResult> GetAll()
        {
            try
            {
                var allGroups = _mapper.Map<IEnumerable<RepairGroupViewModel>>(await _repairGroupService.GetItems());

                return Ok(allGroups.ToList());
            }
            catch
            {
                return NoContent();
            }
        }
    }
}
