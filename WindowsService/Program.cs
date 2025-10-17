
using System.Text.Json;
using WindowsService.WindowsService;

var summaries = ServiceSummary.Load();
var simpleSummaries = ServiceSimpleSummary.Load();

var json1 = JsonSerializer.Serialize(summaries,
    new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    });
var json2 = JsonSerializer.Serialize(simpleSummaries,
    new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    });

Console.WriteLine(json1);
Console.WriteLine(json2);

Console.ReadLine();
