# DeepSeekSDK-NET

![NuGet Version](https://img.shields.io/nuget/v/Ater.DeepSeek.Core)

[DeepSeek](https://www.deepseek.com) API SDK specifically for .NET developers

[中文文档](./README_cn.md)

## 🚀 Features

- [x] List models
- [x] Chat & Chat streaming
- [x] Completions & Completions streaming (beta)
- [x] User balance
- [x] Local model support
- [x] ASP.NET Core integration support

## Usage Requirements

## Usage

Please go to [official website](https://platform.deepseek.com/), register and apply for DeepSeek's ApiKey

Supported .NET version: .NET8

### Install Nuget package

[Ater.DeepSeek.Core](https://www.nuget.org/packages/Ater.DeepSeek.Core)

```shell
dotnet add package Ater.DeepSeek.Core
```

### Instantiate `DeepSeekClient`

Two methods are provided for instantiation:

```csharp
public DeepSeekClient(string apiKey);
public DeepSeekClient(HttpClient http, string apiKey);
```

The first type only requires providing the 'apiKey' to create an instance;

The second method provides a `HttpClient` parameter, which is suitable for maintaining the `HttpClient` through the `HttpClientFactory` and then instance it.

> [!NOTE]
The default timeout for internal HttpClient is 120 seconds, which can be set before sending the request using the 'SetTimeout()' method, or by using the 'CancellationTokeSource' to set the timeout for specific requests.

> [!TIP]
> If you want to call a local model, try customizing `HttpClient` and setting `BaseAddress` to the local address.

### Calling method

`DeepSeekClient` class provides six asynchronous methods to call DeepSeek's API:

```csharp
Task<ModelResponse?> ListModelsAsync(CancellationToken cancellationToken);

Task<ChatResponse?> ChatAsync(ChatRequest request, CancellationToken cancellationToken);

Task<IAsyncEnumerable<Choice>?> ChatStreamAsync(ChatRequest request, CancellationToken cancellationToken);

Task<ChatResponse?> CompletionsAsync(CompletionRequest request, CancellationToken cancellationToken);

Task<IAsyncEnumerable<Choice>?> CompletionsStreamAsync(CompletionRequest request, CancellationToken cancellationToken);

Task<UserResponse?> GetUserBalanceAsync(CancellationToken cancellationToken);

```

### List Models Sample

```csharp
// Create an instance using the apiKey
var client = new DeepSeekClient(apiKey);

var modelResponse = await client.ListModelsAsync(new CancellationToken());
if (modelResponse is null)
{
    Console.WriteLine(client.ErrorMsg);
    return;
}
foreach (var model in modelResponse.Data)
{
    Console.WriteLine(model);
}
```

### Chat Examples

```csharp
// Create an instance using the apiKey
var client = new DeepSeekClient(apiKey);
// Construct the request body
var request = new ChatRequest
{
    Messages = [
        Message.NewSystemMessage("You are a language translator"),
        Message.NewUserMessage("Please translate 'They are scared! ' into English!")
    ],
    // Specify the model
    Model = Constant.Model.ChatModel
};

var chatResponse = await client.ChatAsync(request, new CancellationToken());
if (chatResponse is null)
{
    Console.WriteLine(client.ErrorMsg);
}
Console.WriteLine(chatResponse?.Choices.First().Message?.Content);
```

### Chat Examples (Stream)

```csharp
// Create an instance using the apiKey
var client = new DeepSeekClient(apiKey);
// Construct the request body
var request = new ChatRequest
{
    Messages = [
        Message.NewSystemMessage("You are a language translator"),
        Message.NewUserMessage("Please translate 'They are scared! ' into English!")
    ],
    // Specify the model
    Model = Constant.Model.ChatModel
};

var choices = client.ChatStreamAsync(request, new CancellationToken());
if (choices is null)
{
    Console.WriteLine(client.ErrorMsg);
    return;
}
await foreach (var choice in choices)
{
    Console.Write(choice.Delta?.Content);
}
Console.WriteLine();
```

### Local Model Examples

```csharp
// use local models api
var httpClient = new HttpClient
{
    // set your local api address
    BaseAddress = new Uri("http://localhost:5000"),
    Timeout = TimeSpan.FromSeconds(300),
};
// if have api key
// httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + "your_token");

var localClient = new DeepSeekClient(httpClient);
localClient.SetChatEndpoint("/chat");
localClient.SetCompletionEndpoint("/completions");

var res = await localClient.ChatAsync(new ChatRequest
{
    Messages = new List<Message>
    {
        Message.NewUserMessage("hello")
    }
}, new CancellationToken());

return res?.Choices.First().Message?.Content;
```

> [!TIP]
> More [usage example](https://github.com/niltor/DeepSeekSDK-NET/tree/dev/sample/Sample)

## ASP.NET Core Integration

### Install `Ater.DeepSeek.AspNetCore` package

```shell
dotnet add package Ater.DeepSeek.AspNetCore
```

### Usage in ASP.NET Core

```csharp
using DeepSeek.AspNetCore;
using DeepSeek.Core;
using DeepSeek.Core.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var apiKey = builder.Configuration["DeepSeekApiKey"];
builder.Services.AddDeepSeek(option =>
{
    option.BaseAddress = new Uri("https://api.deepseek.com");
    option.Timeout = TimeSpan.FromSeconds(300);
    option.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + apiKey);
});

var app = builder.Build();

app.MapGet("/test", async ([FromServices] DeepSeekClient client) =>
{
    var res = await client.ChatAsync(new ChatRequest
    {
        Messages = new List<Message>
        {
            Message.NewUserMessage("Why dotnet is good?")
        },
        MaxTokens = 200
    }, new CancellationToken());

    return res?.Choices.First().Message?.Content;
});

app.Run();
```

### Usage in ASP.NET Core (Stream)

```csharp
app.MapGet("/chat", async (HttpContext context, [FromServices] DeepSeekClient client, CancellationToken token) =>
{
    context.Response.ContentType = "text/text;charset=utf-8";
    try
    {
        var choices = client.ChatStreamAsync(new ChatRequest
        {
            Messages = new List<Message>
            {
                Message.NewUserMessage("Why dotnet is good?")
            },
            MaxTokens = 200
        }, token);

        if (choices != null)
        {
            await foreach (var choice in choices)
            {
                await context.Response.WriteAsync(choice.Delta!.Content);
            }
        }
    }
    catch (Exception ex)
    {
        await context.Response.WriteAsync("暂时无法提供服务" + ex.Message);
    }
    await context.Response.CompleteAsync();
});
```

> [!TIP]
> More [usage example](https://github.com/niltor/DeepSeekSDK-NET/tree/dev/sample/AspNetCoreSample)
