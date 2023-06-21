using ComponentsHub.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            List<ParsedComponent> parsedComponents = new List<ParsedComponent>();

            using (HttpClient client = new HttpClient())
            {
                // TODO Move to separate class or file(json??)
                string url = "https://office.promelec.ru/php/ajax-goods-price.php";
                // string requestData = "Item_ID=139728&ajax=true";
                List<string> requestData = new List<string>();

                var components = ComponentModelCollection.ParseJSON(@"./Properties/config.json");

                for (int i = 0; i < components.components.Count; i++)
                {
                    requestData.Add($"Item_ID={components.components[i].id}&ajax=true");
                }

                for (int i = 0; i < requestData.Count; i++)
                {
                    try
                    {
                        HttpContent content = new StringContent(requestData[i], Encoding.UTF8, "application/x-www-form-urlencoded");
                        HttpResponseMessage response = await client.PostAsync(url, content);

                        string responseString = await response.Content.ReadAsStringAsync();
                        parsedComponents.Add(ParseHtmlPromelec(responseString));
                        parsedComponents[i].PartNumber = components.components[i].name;

                    }
                    catch (Exception ex)
                    {
                        ViewBag.ParsedHtml = "Error: " + ex.Message;
                    }
                }
            }
            return View(parsedComponents);
        }

        public ParsedComponent ParseHtmlPromelec(string data)
        {
            ParsedComponent component = new ParsedComponent();
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(data);

            try
            {
               var rawParsedHtml = htmlDoc.DocumentNode.SelectNodes("//table/tbody/tr[@price_unit='1']/td")
                                                    .ToArray();
                component.Amount = rawParsedHtml[2].InnerText;

            }
            catch
            {
                component.Amount = "ОШИБКА";
            }

            try
            {
                var rawParsedHtml = htmlDoc.DocumentNode.SelectNodes("//*[@id='pb1']/td[3]").ToArray();
                                                     
                component.Price = rawParsedHtml[0].InnerText;

            }
            catch
            {
                component.Price = "ОШИБКА";
            }

            return component;
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
