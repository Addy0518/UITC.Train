using System.Net;
using Azure.Messaging;
using Lab.API.Exception_Handing.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab.API.Exception_Handing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly TestContext _context;

        public TestController(TestContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _context.Tests.FindAsync(id);
            if (result == null)
            {
                throw new NotFoundException("Product", id);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> TestCreate(Test test)
        {
            // try : 嘗試執行執行區塊內的程式碼 , 測試是否有錯誤
            try
            {
                _context.Add(test);
                await _context.SaveChangesAsync();

                return Ok();
            }
            // catch : 捕捉一切拋出的錯誤
            // FormatException : 格式錯誤
            catch (FormatException fex)
            {
                // throw : 扔出異常
                throw new Exception("格式錯誤!");
            }
            // Exception : 通常放在最後 , 抓一切沒抓到的錯誤
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            // 不管有無錯誤 , 最後都會執行
            finally { }
        }

        [HttpGet("error")]
        public IActionResult GetError()
        {
            throw new InvalidOperationException("有錯誤!");
        }
    }
}
