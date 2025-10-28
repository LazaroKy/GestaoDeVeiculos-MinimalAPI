//using GestaoDeVeiculos_MinimalAPI;

IHostBuilder CreateHostBuilder(string[] args)
{
    return Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webbuilder =>
        {
            webbuilder.UseStartup<Startup>();
        });
}
CreateHostBuilder(args).Build().Run();