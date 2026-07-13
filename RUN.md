```bash
SYSTEM_KEY=$(az functionapp keys list --name $(azd env get-value AZURE_FUNCTION_NAME) --resource-group $(azd env get-value AZURE_RESOURCE_GROUP) --query "systemKeys.durabletask_extension" -o tsv)
```

```bash
curl -X POST "https://$(azd env get-value AZURE_FUNCTION_NAME).azurewebsites.net/runtime/webhooks/durabletask/orchestrators/agent_orchestration_workflow?code=$SYSTEM_KEY" \
  -H "Content-Type: application/json" \
  -d '"\"Which ingredients do I need to make Stroopwafels?\""'
```
