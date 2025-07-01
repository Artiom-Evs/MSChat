namespace MSChat.WebBFF.Configuration;

public class CorsConfiguration
{
    public const string SectionName = "Cors";
    
    public List<CorsPolicy> Policies { get; set; } = [];
}

public class CorsPolicy
{
    public string Name { get; set; } = string.Empty;
    public List<string> Origins { get; set; } = new();
    public List<string> Methods { get; set; } = new();
    public List<string> Headers { get; set; } = new();
    public bool AllowCredentials { get; set; }
    public bool AllowAnyOrigin { get; set; }
    public bool AllowAnyMethod { get; set; }
    public bool AllowAnyHeader { get; set; }
}