using DeepSeek.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using static DeepSeek.Core.Models.Usage;

namespace DeepSeek.Core.Json
{
    [JsonSourceGenerationOptions(
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    [JsonSerializable(typeof(ChatRequest))]
    [JsonSerializable(typeof(ResponseFormat))]
    [JsonSerializable(typeof(StreamOptions))]
    [JsonSerializable(typeof(Tool))]
    [JsonSerializable(typeof(ChatResponse))]
    [JsonSerializable(typeof(Choice))]
    [JsonSerializable(typeof(Content))]
    [JsonSerializable(typeof(Logprobs))]
    [JsonSerializable(typeof(TopLogprobs))]
    [JsonSerializable(typeof(Usage))]
    [JsonSerializable(typeof(CompletionTokensDetails))]
    [JsonSerializable(typeof(CompletionRequest))]
    [JsonSerializable(typeof(Message))]
    [JsonSerializable(typeof(ModelResponse))]
    [JsonSerializable(typeof(Model))]
    [JsonSerializable(typeof(UserResponse))]
    [JsonSerializable(typeof(UserBalance))]
    public partial class DeepSeekJsonContext : JsonSerializerContext
    {

    }
}
