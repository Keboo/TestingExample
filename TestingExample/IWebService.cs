namespace TestingExample;

public interface IWebService 
{
    Task<string> GetDataAsync();
    Task<string> GetXmlAsync();
}

public interface IJsonWebService
{
    Task<string> GetJsonAsync();
}

public class MyWebService : IWebService, IJsonWebService
{
    public async Task<string> GetDataAsync()
    {
        await Task.Delay(TimeSpan.FromSeconds(10));
        return "Hello World!";
    }

    public Task<string> GetJsonAsync()
    {
        throw new NotImplementedException();
    }

    public Task<string> GetXmlAsync()
    {
        throw new NotImplementedException();
    }
}