using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TestingExample;

//Pretend this is a real EF Context
public class MyDbContext
{
    //...

    public virtual Task SaveChangesAsync()
    {
        return Task.CompletedTask;
    }
}

public partial class MainWindowViewModel : ObservableObject
{
    //This is using the source generators from CommunityToolkit.Mvvm to generate a RelayCommand
    //See: https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/generators/observableproperty
    //and: https://learn.microsoft.com/windows/communitytoolkit/mvvm/observableobject
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(IncrementCountCommand))]
    private int _count;

    [ObservableProperty]
    private string? _data;

    private readonly IWebService _webService;
    private readonly MyDbContext _context;

    public MainWindowViewModel(IWebService webService, MyDbContext context)
    {
        _webService = webService ?? throw new ArgumentNullException(nameof(webService));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    [RelayCommand]
    private async Task GetDataAsync()
    {
        string data1 = await _webService.GetDataAsync();
        string data2 = await _webService.GetDataAsync();
        Data = data1 + data2;
        //Do stuff with data
        await _context.SaveChangesAsync();
    }

    public async Task GetData2Async()
    {
        string data2 = await _webService.GetXmlAsync();
        string data1 = await _webService.GetDataAsync();
        Data = data1 + data2;
        //Do stuff with data
        await _context.SaveChangesAsync();
    }

    //This is using the source generators from CommunityToolkit.Mvvm to generate a RelayCommand
    //See: https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/generators/relaycommand
    //and: https://learn.microsoft.com/windows/communitytoolkit/mvvm/relaycommand
    [RelayCommand(CanExecute = nameof(CanIncrementCount))]
    private void IncrementCount()
    {
        Count++;
    }

    private bool CanIncrementCount() => Count < 5;

    [RelayCommand]
    private void ClearCount()
    {
        Count = 0;
    }
}
