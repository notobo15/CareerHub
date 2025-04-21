using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RecruitmentApp.Models;

namespace RecruitmentApp.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AddressController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("provinces")]
        public IActionResult GetProvinces()
        {

            var districts = _context.Provinces
                .Select(d => new { d.Code, d.FullName })
                .ToList();

            return Ok(districts);
        }

        /// <summary>
        /// Lấy danh sách Quận/Huyện theo Tỉnh/Thành phố
        /// </summary>
        [HttpGet("districts")]
        public IActionResult GetDistricts(string provinceCode)
        {
            if (string.IsNullOrEmpty(provinceCode))
                return BadRequest("Mã tỉnh không hợp lệ");

            var districts = _context.Districts
                .Where(d => d.ProvinceId == provinceCode)
                .Select(d => new { d.Code, d.FullName })
                .ToList();

            return Ok(districts);
        }

        /// <summary>
        /// Lấy danh sách Phường/Xã theo Quận/Huyện
        /// </summary>
        [HttpGet("wards")]
        public IActionResult GetWards(string districtCode)
        {
            if (string.IsNullOrEmpty(districtCode))
                return BadRequest("Mã quận không hợp lệ");

            var wards = _context.Wards
                .Where(w => w.DistrictId == districtCode)
                .Select(w => new { w.Code, w.FullName })
                .ToList();

            return Ok(wards);
        }
    }
}
