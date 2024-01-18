using Moq.AutoMock.Resolvers;

namespace TestingExample.Tests;

[ConstructorTests(typeof(MainWindowViewModel))]
public partial class ExampleTests
{
    //SUT = System Under Test

    [Fact]
    //               Act                     _   Arrange       _   Assert
    public void IncrementCountCommandOnExecute_WithSetCount_IncrementsCount()
    {
        // Arrange
        MainWindowViewModel sut = new(Mock.Of<IWebService>(), Mock.Of<MyDbContext>());

        int beforeCount = sut.Count;

        // Act
        sut.IncrementCountCommand.Execute(null);

        // Assert
        //Assert.Equal(0, beforeCount);
        Assert.Equal(beforeCount + 1, sut.Count);
    }

    [Fact]
    //               Act                     _   Arrange       _   Assert
    public void IncrementCountCommandOnExecute_WithSetCount_IncrementsCount_AM()
    {
        // Arrange
        //ServiceCollection collection = new();
        //collection.AddSingleton<IWebService>(_ => Mock.Of<IWebService>());
        //collection.AddSingleton<MyDbContext>();
        //ServiceProvider provider = collection.BuildServiceProvider();

        //MainWindowViewModel vm = provider.GetRequiredService<MainWindowViewModel>();

        AutoMocker mocker = new();
        MainWindowViewModel sut = mocker.CreateInstance<MainWindowViewModel>();

        int beforeCount = sut.Count;

        // Act
        sut.IncrementCountCommand.Execute(null);

        // Assert
        //Assert.Equal(0, beforeCount);
        Assert.Equal(beforeCount + 1, sut.Count);
    }

    [Fact]
    public void GetDataCommandOnExecute_RetrievesData()
    {
        // Arrange
        string expectedData1 = "My result";
        string expectedData2 = " Hello World!";
        AutoMocker mocker = new();
        
        mocker.Use(new Mock<IWebService>(MockBehavior.Strict));
        mocker.SetupSequence<IWebService, Task<string>>(x => x.GetDataAsync())
            .ReturnsAsync("My result")
            .ReturnsAsync(" Hello World!");

        //These are equal
        mocker.Use(mocker.CreateInstance<MyDbContext>());
        mocker.With<MyDbContext>();

        MainWindowViewModel sut = mocker.CreateInstance<MainWindowViewModel>();

        // Act
        sut.GetDataCommand.Execute(null);

        // Assert
        Assert.Equal(expectedData1 + expectedData2, sut.Data);
        //Assert.Equal(1, service.GetDataAsyncCallCount);
        //Validate the mock was invoked
        mocker.Verify<IWebService>(x => x.GetDataAsync(), Times.Exactly(2));
        mocker.Get<IWebService>();
        //IWebService foo = Mock.Of<IWebService>();
        //Type t1 = typeof(IWebService);
        //Type t2 = foo.GetType();
    }

    private class TodoItemResolver : IMockResolver
    {
        public void Resolve(MockResolutionContext context)
        {
            context.Value = null;
            //sets something
        }
    }

    [Fact]
    public async Task GetData2Async_RetrievesData()
    {
        // Arrange
        string expectedData1 = "My result";
        string expectedData2 = " Hello World!";

        Mock<IWebService> mockWebService = new(MockBehavior.Strict);
        bool getXmlCalled = false;
        bool getJsonCalled = false;
        mockWebService.Setup(x => x.GetXmlAsync())
            .Callback(() =>
            {
                Assert.False(getXmlCalled);
                Assert.False(getJsonCalled);
                getXmlCalled = true;
            })
            .ReturnsAsync(expectedData2);
        mockWebService.Setup(x => x.GetDataAsync())
            .Callback(() =>
            {
                Assert.True(getXmlCalled);
                Assert.False(getJsonCalled);
                getJsonCalled = true;
            })
            .ReturnsAsync(expectedData1);

        MainWindowViewModel sut = new(mockWebService.Object, Mock.Of<MyDbContext>());

        // Act
        await sut.GetData2Async();

        // Assert
        Assert.Equal(expectedData1 + expectedData2, sut.Data);

        mockWebService.VerifyAll();
    }

    [Fact]
    public void GetDataCommandOnExecute_PersistsData()
    {
        // Arrange
        Mock<MyDbContext> contextMock = new();

        MainWindowViewModel sut = new(Mock.Of<IWebService>(), contextMock.Object);

        // Act
        sut.GetDataCommand.Execute(null);

        // Assert
        contextMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    private class TestingWebService : IWebService
    {
        public int GetDataAsyncCallCount { get; private set; }
        public Task<string> GetDataAsync()
        {
            GetDataAsyncCallCount++;
            return Task.FromResult<string>(default(string));
            //return Task.FromResult("My result");
        }

        public Task<string> GetXmlAsync()
        {
            throw new NotImplementedException();
        }
    }
}
