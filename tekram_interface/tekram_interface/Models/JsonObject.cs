namespace tekram_interface.Models
{
    public class JsonObject
    {
        public string Text { get; set; }
        public Dictionary<string, string> Keyboard { get; set; }

        public JsonObject()
        {
            Keyboard = new Dictionary<string, string>();
        }
    }
}
