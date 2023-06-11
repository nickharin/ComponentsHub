using ComponentsHub.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ComponentsHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<ActionResult> Index()
        {
            using (HttpClient client = new HttpClient())
            {
                // TODO Move to separate class or file(json??)
                string url = "https://office.promelec.ru/php/ajax-goods-price.php";
                string requestData = "Item_ID=139728&ajax=true";

                try
                {
                    HttpContent content = new StringContent(requestData, Encoding.UTF8, "application/x-www-form-urlencoded");
                    HttpResponseMessage response = await client.PostAsync(url, content);

                    string responseContent = await response.Content.ReadAsStringAsync();

                    HtmlDocument htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(responseContent);

                    //TODO parse html
                    var rawParsedHtml = htmlDoc.DocumentNode.SelectNodes("//table/tbody/tr[@data-tooltip='Поставка со склада']/td").ToArray();
                    //Pass the parsed HTML to the view
                    //ViewBag.ParsedHtml = htmlDoc.DocumentNode.OuterHtml;
                    var parsedAmount = rawParsedHtml[2].InnerText;
                    ViewBag.ParsedAmount = parsedAmount;
                }
                catch (Exception ex)
                {
                    ViewBag.ParsedHtml = "Error: " + ex.Message;
                }
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
