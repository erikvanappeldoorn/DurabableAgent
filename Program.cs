using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting.AzureFunctions;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using OpenAI;
using OpenAI.Chat;

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") 
    ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT environment variable is not set");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT") ?? "gpt-5-mini";

// Create the Azure OpenAI client
AzureOpenAIClient client = new(new Uri(endpoint), new DefaultAzureCredential());
ChatClient chatClient = client.GetChatClient(deploymentName);

// Create the main agent from the first tutorial
AIAgent mainAgent = chatClient.AsAIAgent(
    instructions: "You are a helpful assistant that can answer questions and provide information.",
    name: "MainAgent");

// Create translation agents
AIAgent norskAgent = chatClient.AsAIAgent(
    instructions: "You are a translator. Translate the following text to Norwegian. Return only the translation, no explanations.",
    name: "NorskTranslator");

AIAgent dutchAgent = chatClient.AsAIAgent(
    instructions: "You are a translator. Translate the following text to Dutch. Return only the translation, no explanations.",
    name: "DutchTranslator");        

// Build and configure the Functions host
using IHost app = FunctionsApplication
    .CreateBuilder(args)
    .ConfigureFunctionsWebApplication()
    .ConfigureDurableAgents(options =>
    {
        // Register all agents for use in orchestrations and HTTP endpoints
        options.AddAIAgent(mainAgent);
        options.AddAIAgent(norskAgent);
        options.AddAIAgent(dutchAgent);
    })
    .Build();

app.Run();