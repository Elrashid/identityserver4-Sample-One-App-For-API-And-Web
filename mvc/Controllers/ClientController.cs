using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System;

namespace App2.Controllers
{
    public class ClientController : Controller
    {

        public async Task<IActionResult> index()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            Console.Write(accessToken);

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var content = await client.GetStringAsync("http://localhost:5011/api/secure");
            Console.Write(content);
            ViewData["Json"] = JArray.Parse(content).ToString();
            return View();
        }
    }
}
