using System.ComponentModel.DataAnnotations;

namespace MSChat.Shared.Configuration.Models;

public class GrpcSettings
{
    public const string Position = "Kestrel:Grpc";

    [Required]
    [Url]
    public string Url { get; init; } = "";
}
