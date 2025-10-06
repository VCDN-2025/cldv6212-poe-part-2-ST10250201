namespace ABCRetail.Functions.Shared;

public static class Config
{
    public static string Get(string name)
        => Environment.GetEnvironmentVariable(name)
           ?? throw new InvalidOperationException($"Missing setting: {name}");
}
