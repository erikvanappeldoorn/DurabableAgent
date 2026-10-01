using Microsoft.Agents.AI;
using Microsoft.Agents.AI.DurableTask;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace MyDurableAgent;

public static class AgentOrchestration
{
    // Define a strongly-typed response structure for agent outputs
    public sealed record TextResponse(string Text);

    [Function("agent_orchestration_workflow")]
    public static async Task<Dictionary<string, string>> AgentOrchestrationWorkflow(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var input = context.GetInput<string>() ?? throw new ArgumentNullException(nameof(context), "Input cannot be null");
        
        DurableAIAgent mainAgent = context.GetAgent("MainAgent");
        AgentResponse<TextResponse> mainResponse = await mainAgent.RunAsync<TextResponse>(input);
        string agentResponse = mainResponse.Result.Text;
        
        DurableAIAgent norskAgent = context.GetAgent("NorskTranslator");
        DurableAIAgent dutchAgent = context.GetAgent("DutchTranslator");

        Task<AgentResponse<TextResponse>> norskTask = norskAgent.RunAsync<TextResponse>(agentResponse);
        Task<AgentResponse<TextResponse>> dutchTask = dutchAgent.RunAsync<TextResponse>(agentResponse);
        
        await Task.WhenAll(norskTask, dutchTask);
        
        TextResponse norskResponse = (await norskTask).Result;
        TextResponse dutchResponse = (await dutchTask).Result;
        
        var result = new Dictionary<string, string>
        {
            ["original"] = agentResponse,
            ["norsk"] = norskResponse.Text,
            ["dutch"] = dutchResponse.Text
        };

        return result;
    }
}