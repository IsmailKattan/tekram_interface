using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using tekram_interface.Models;

namespace tekram_interface.Controllers
{
    public class JsonController : Controller
    {
        private readonly string jsonFilePath = "Data.json";
        private readonly string imageJsonFilePath = "Image.json";
        public IActionResult Index()
        {
            string jsonData = System.IO.File.ReadAllText(jsonFilePath);

            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, JsonObject>>(jsonData);

            return View(jsonObject);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(string objectName, JsonObject model, string[] keys, string[] values, string imagePath)
        {
            string jsonFilePath = "Data.json";
            string jsonData = System.IO.File.ReadAllText(jsonFilePath);
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, JsonObject>>(jsonData);

            string imageJsonData = System.IO.File.ReadAllText(imageJsonFilePath);
            var imageObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(imageJsonData);

            model.Text = model.Text.Replace("\r\n", "\n");

            model.Keyboard = new Dictionary<string, string>();
            for (int i = 0; i < keys.Length; i++)
            {
                if (!string.IsNullOrEmpty(keys[i]) && !string.IsNullOrEmpty(values[i]))
                {
                    model.Keyboard.Add(keys[i], values[i]);
                }
            }

            jsonObject.Add(objectName, model);

            if (!imageObject.ContainsKey(objectName))
            {
                imageObject.Add(objectName, imagePath);
            }
            else
            {
                imageObject[objectName] = imagePath;
            }

            string updatedJsonData = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
            string updatedImageJsonData = JsonConvert.SerializeObject(imageObject, Formatting.Indented);

            System.IO.File.WriteAllText(jsonFilePath, updatedJsonData);
            System.IO.File.WriteAllText(imageJsonFilePath, updatedImageJsonData);

            return RedirectToAction("Index");
        }

        public IActionResult Update(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                string jsonData = System.IO.File.ReadAllText(jsonFilePath);
                var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, JsonObject>>(jsonData);

                string imageJson = System.IO.File.ReadAllText(imageJsonFilePath);
                var imageObject = JsonConvert.DeserializeObject<Dictionary<string,string>>(imageJson);

                if (jsonObject.ContainsKey(id)) 
                {
                    if(imageObject.ContainsKey(id))
                    {
                        var image = imageObject[id];
                        ViewBag.image = image;
                    }
                    ViewBag.id = id;
                    var model = jsonObject[id];
                    return View(model);
                }
                else
                {
                    return NotFound();
                }
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult Update(string id,JsonObject updatedModel,string Text, string[] keys,string[] values, string imagePath)
        {
            string jsonData = System.IO.File.ReadAllText(jsonFilePath);
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<String, JsonObject>>(jsonData);

            string imageJsonData = System.IO.File.ReadAllText(imageJsonFilePath);
            var imageObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(imageJsonData);

            if (jsonObject.ContainsKey(id))
            {
                updatedModel.Text = updatedModel.Text.Replace("\r\n", "\n");

                for (int i = 0; i < keys.Length; i++)
                {
                    if (!string.IsNullOrEmpty(keys[i]) && !string.IsNullOrEmpty(values[i]))
                    {
                        updatedModel.Keyboard.Add(keys[i], values[i]);
                    }
                }
                jsonObject[id] = updatedModel;

                string updatedJsonData = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
                System.IO.File.WriteAllText(jsonFilePath, updatedJsonData);

                if (imageObject.ContainsKey(id))
                {
                    imageObject[id] = imagePath;
                    string updatedImageJsonData = JsonConvert.SerializeObject(imageObject, Formatting.Indented);
                    System.IO.File.WriteAllText(imageJsonFilePath, updatedImageJsonData);
                }

                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }

        }

        // Helper method to load JSON object from file
        private Dictionary<string, JsonObject> LoadJsonObjectFromFile()
        {
            string jsonData = System.IO.File.ReadAllText(jsonFilePath);
            return JsonConvert.DeserializeObject<Dictionary<string, JsonObject>>(jsonData);
        }

        // Helper method to save JSON object to file
        private void SaveJsonObjectToFile(Dictionary<string, JsonObject> jsonObject)
        {
            string updatedJsonData = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
            System.IO.File.WriteAllText(jsonFilePath, updatedJsonData);
        }

        public IActionResult Delete(string id)
        {
            string jsonFilePath = "Data.json";
            string jsonData = System.IO.File.ReadAllText(jsonFilePath);
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, JsonObject>>(jsonData);

            if (jsonObject.ContainsKey(id))
            {
                jsonObject.Remove(id);

                string updatedJsonData = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);

                System.IO.File.WriteAllText(jsonFilePath, updatedJsonData);

                string imageJsonFilePath = "Image.json";
                if (System.IO.File.Exists(imageJsonFilePath))
                {
                    string imageJsonData = System.IO.File.ReadAllText(imageJsonFilePath);
                    var imageObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(imageJsonData);
                    if (imageObject.ContainsKey(id))
                    {
                        imageObject.Remove(id);
                        string updatedImageJsonData = JsonConvert.SerializeObject(imageObject, Formatting.Indented);
                        System.IO.File.WriteAllText(imageJsonFilePath, updatedImageJsonData);
                    }
                }

                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
        }
    }
}

