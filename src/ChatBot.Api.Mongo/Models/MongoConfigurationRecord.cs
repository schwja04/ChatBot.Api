using System.ComponentModel.DataAnnotations;

namespace ChatBot.Api.Mongo.Models;

public class MongoConfigurationRecord
{
    public static readonly string RootKey = "Mongo";

    [Required]
    public string ConnectionString { get; set; } = null!;

    [Required]
    public string DatabaseName { get; set; } = null!;

    public string? Username { get; set; }

    public string? Password { get; set; }
}
