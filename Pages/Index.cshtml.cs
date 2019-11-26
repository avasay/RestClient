using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RestAPIClient.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string RequestMethod { get; set; }

        [BindProperty]
        public string Data { get; set; }

        [BindProperty]
        public string BaseUrl { get; set; }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPost()
        {
            string responseContent = "[]";
            try
            {
                Uri baseURL = new Uri(BaseUrl);

                HttpClient client = new HttpClient();

                // Any parameters? Get value, and then add to the client 
                string key = HttpUtility.ParseQueryString(baseURL.Query).Get("key");
                if (key != "")
                {
                    client.DefaultRequestHeaders.Add("api-key", key);
                }

            
                if (RequestMethod.Equals("GET"))
                {
                    HttpResponseMessage response = await client.GetAsync(baseURL.ToString());

                    if (response.IsSuccessStatusCode)
                    {
                        responseContent = await response.Content.ReadAsStringAsync();                      
                    }
                }

                else if (RequestMethod.Equals("POST"))
                {
                    JObject jObject = JObject.Parse(Data);

                    var stringContent = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(baseURL.ToString(), stringContent);

                    if (response.IsSuccessStatusCode)
                    {
                        responseContent = await response.Content.ReadAsStringAsync();

                    }
                }

                return RedirectToPage("Response", new { result = responseContent });
                
            }
            catch(ArgumentNullException uex)
            {
                return RedirectToPage("Error", new { msg = uex.Message + " | URL missing or invalid." });
            }
            catch (JsonReaderException jex)
            {
                return RedirectToPage("Error", new { msg = jex.Message + " | Json data could not be read." });
            }
            catch (Exception ex)
            {
                return RedirectToPage("Error", new { msg = ex.Message + " | Are you missing some Json keys and values? Please check your Json data." });
            }


        }
    }
}
