using System.Text.Json.Serialization;

public class TestModel
{
    public int Id { get; set; }

    public string FullName { get; set; }

    [JsonIgnore]
    public int Age { get; set; }
}