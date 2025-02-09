namespace GeminiAI.Models
{
    public class GeminiContent
    {
        // La propiedad 'Contents' contiene una lista de objetos Content
        public List<Content> Contents { get; set; } = new List<Content>();
    }

    public class Part
    {
        // Cada objeto Part tiene un texto
        public string Text { get; set; }
    }

}
