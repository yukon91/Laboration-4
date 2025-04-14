using Northwind.DataContext.SqlServer;
using Northwind.EntityModels;

#region  Konfigurera web server host och tj�nster
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddNorthwindContext();
builder.Services.AddRequestDecompression(); //l�gger till compresson tj�nster (Accept-Encoding i DevTools)


var app = builder.Build();
#endregion

#region Konfiguration av HTTP pipeline och routing
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.Use(async (HttpContext context, Func<Task> next) =>
{
    RouteEndpoint? rep = context.GetEndpoint() as RouteEndpoint;

    if (rep is not null)
    {
        WriteLine($"Endpoint name: {rep.DisplayName}");
        WriteLine($"Endpoint route pattern: {rep.RoutePattern.RawText}");
    }
    if (context.Request.Path == "/bonjour")
    {
        // ifall att vi har ett match p� URL path, detta avbryter pipeline,
        //delegate skickar inte anrop till n�sta middleware
        await context.Response.WriteAsync("Bonjour Monde!");
        return;
    }
    //vi kan modifera request och response h�r innan anropet g�r vidare
    await next();

    //vi kan modifera response h�r efter att vi har kalla next delegate 
    //och innan den skickas till klienten
});

app.UseHttpsRedirection();
app.UseRequestDecompression(); //anrop till att anv�nda decompresson middleware i pipeline

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapRazorPages();

app.MapGet("/hello", () => $"Enviroment is {app.Environment.EnvironmentName}");
#endregion

//k�rs web servern
app.Run();
WriteLine("Detta exekveras efter att web server har stoppats!");
