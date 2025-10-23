using PawNest.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

TimeZoneInfo.ClearCachedData();
var utcPlus7 = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddPawNestCore(builder.Configuration); 

var app = builder.Build();

app.UsePawNestPipeline(app.Environment);

app.Run();