using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace Pokemon
{
    public class PokemonController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public async Task<string> Get(string key)
        {
            using (var client = new HttpClient())
            {
                string text = "";

                client.BaseAddress = new Uri("https://pokeapi.co/");

                HttpResponseMessage Res = await client.GetAsync("api/v2/pokemon/" + key);

                if (Res.IsSuccessStatusCode)
                {
                    
                    string jsonString = await Res.Content.ReadAsStringAsync();

                    if (jsonString == "Not Found")
                    {
                        return "";
                    }

                    var jsonData = (JObject)JsonConvert.DeserializeObject(jsonString);

                    string url = jsonData["species"]["url"].Value<string>();

                    Res = await client.GetAsync(url);
                    
                    if (Res.IsSuccessStatusCode)
                    {
                        jsonString = await Res.Content.ReadAsStringAsync();
                        jsonData = (JObject)JsonConvert.DeserializeObject(jsonString);

                        var flavors = jsonData["flavor_text_entries"].Value<JArray>();
                        
                        foreach (var flavor in flavors)
                        {
                            if (flavor["language"]["name"].Value<string>() == "en")
                            {
                                text = flavor["flavor_text"].Value<string>();
                                if (key.Equals(text.Substring(0, key.Length), StringComparison.CurrentCultureIgnoreCase))
                                {

                                    text = text.Replace("\n", " ");
                                    text = text.Replace("\f", " ");

                                    break;
                                }
                                
                            }
                        }

                        if (text != "")
                        {
                        
                            Res = await client.GetAsync("https://api.funtranslations.com/translate/shakespeare.json?text=" + text);
                            if (Res.IsSuccessStatusCode)
                            {
                                jsonString = await Res.Content.ReadAsStringAsync();
                                jsonData = (JObject)JsonConvert.DeserializeObject(jsonString);
                                text = jsonData["contents"]["translated"].Value<string>();
                                
                                
                            }
                            else
                            {
                                jsonString = await Res.Content.ReadAsStringAsync();
                                jsonData = (JObject)JsonConvert.DeserializeObject(jsonString);
                                text = jsonData["error"]["message"].Value<string>();
                            }
                        }

                        
                        
                    }

                }

                JObject resJson = new JObject();
                resJson["name"] = key;
                resJson["description"] = text;

                return resJson.ToString();
            }
            
        }

        // POST api/<controller>
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}