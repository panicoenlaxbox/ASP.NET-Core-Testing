Vídeo del meetup
https://www.youtube.com/watch?v=9GGzuQoSuTA 

Repo con lo explicado en el meetup pero por ahora por escrito
https://github.com/Xabaril/ManualEffectiveTestingHttpAPI 

Repo con el uso de Moq
https://github.com/panicoenlaxbox/TestDoubles

Crear e inicializar un contexto en EF Core
http://panicoenlaxbox.blogspot.com/2017/11/crear-e-inicializar-un-contexto-en-ef.html

Post de Jimmy Bogard (respawn)
Strategies for isolating the database in tests
https://lostechies.com/jimmybogard/2013/06/18/strategies-for-isolating-the-database-in-tests/

_Tasks > Generate Scripts... > Select specific database objects > Advanced > Script USE DATABASE = False, Types of data to script  = Data only_

## Host

https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-5.0

El Host se suele construir en Program.cs, desde Main con una llamada a `CreateHostBuilder`.
```csharp
CreateHostBuilder(args).Build().Run();
``` 

Esto llama a:

```csharp
public static IHostBuilder CreateHostBuilder(string[] args) =>
	Host.CreateDefaultBuilder(args)...
```

El host de web ya está deprecado sólo se mantiene por retrocompatibilidad:

```csharp
public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
	WebHost.CreateDefaultBuilder(args)
```

https://docs.microsoft.com/en-us/aspnet/core/fundamentals/?view=aspnetcore-5.0&tabs=windows#host

Podemos pensar en el host como un wrapper de la aplicación, cuyas responsabilidades son configurar DI, Logging, Configuration y arrancar todas las instancias de `IHostedService` registradas (si una de ellas es un servicio web arrancará un servidor HTTP). También gestionar el ciclo de vida de estos servicios y asegura un apagado controlado (graceful shutdown).

Lo que tiene que quedar claro que Host <> Server https://docs.microsoft.com/en-us/aspnet/core/fundamentals/?view=aspnetcore-5.0&tabs=windows#servers

Y por otro lado está nuestra aplicación.

## ASP.NET-Core-Testing

- `Api.Host` es el host web.
- `Api.Tests` tendrá su [propio host](https://github.com/panicoenlaxbox/ASP.NET-Core-Testing/blob/master/Api.Tests/Infrastructure/Fixtures/IntegrationFixtureBase.cs) que usará además TestServer como servidor.
- `Api` es nuestra aplicación, donde configuramos parte de nuestro fichero `Startup`, pero no todo, el resto se hace en cada host ya que primero carga la parte común y luego cada uno agrega lo específico según host (web o test).


