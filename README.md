# serilog-sinks-signalr

[![Build status](https://ci.appveyor.com/api/projects/status/292m45fa26x5iyfs/branch/master?svg=true)](https://ci.appveyor.com/project/serilog/serilog-sinks-signalr/branch/master)

A Serilog sink that writes events to a SignalR Hub

## Configuration from hub server application

From within the SignalR server application with a hub named MyHub:

```csharp
var hubContext = GlobalHost.ConnectionManager.GetHubContext<MyHub>();

Log.Logger = new LoggerConfiguration()
.MinimumLevel.Verbose()
.WriteTo.SignalRServer(hubContext,
  Serilog.Events.LogEventLevel.Information,
  groupNames: new[] { "CustomGroup"}, // default is null
  userIds: new[] { "JaneD1234" }, // default is null
  excludedConnectionIds: new[] { "12345", "678910" }) // default is null
.CreateLogger();
```

## Configuration from other clients

From any client application with a hub hosted at `http://localhost:8080` and a hub implemented named `MyHub`:

```csharp
Log.Logger = new LoggerConfiguration()
.MinimumLevel.Verbose()
.WriteTo.SignalRHub("http://localhost:8080",
  Serilog.Events.LogEventLevel.Information,
  hub: "MyHub" // default is LogHub
  groupNames: new[] { "CustomGroup"}, // default is null
  userIds: new[] { "JaneD1234" }) // default is null
.CreateLogger();
```

### SignalR Server
Create a hub class with any name that ends in `Hub` or use the default name `LogHub`. Then create a method named `receiveLogEvent`, which is capable of accepting all data from the sink.
```csharp
public class MyHub : Hub
{
  public void receiveLogEvent(string[] groups, string[] userIds, Serilog.Sinks.SignalR.Data.LogEvent logEvent)
  {
    // send to all clients
    Clients.All.sendLogEvent(logEvent);
    // just the specified groups
	Clients.Groups(groups).sendLogEvent(logEvent);
    // just the specified users
    Clients.Users(users).sendLogEvent(logEvent);
  }
}
```

## Receiving the log event
Set up a SignalR client and subscribe to the `sendLogEvent` method.

```csharp
var connection = new HubConnection("http://localhost:8080");
var hubProxy = connection.CreateHubProxy("MyHub");

hubProxy.On<Serilog.Sinks.SignalR.Data.LogEvent>("sendLogEvent", (logEvent) =>
{
  Console.WriteLine(logEvent.RenderedMessage);
});
```