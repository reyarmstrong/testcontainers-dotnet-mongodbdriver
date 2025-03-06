using TestContainersPoC;
using Testcontainers.MongoDb;

namespace TestContainersPoCTests;

public class CustomerServiceTests : IAsyncLifetime
{
    private readonly MongoDbContainer _mongoDbContainer = new MongoDbBuilder()
        .WithImage("mongo:8.0.4")
        .WithPortBinding(27017)
        .WithUsername("admin")
        .WithPassword("admin")
        .Build();

    private CustomerService _sut;

    public async Task InitializeAsync()
    {
        await _mongoDbContainer.StartAsync();
        var connectionString = _mongoDbContainer.GetConnectionString();
        _sut = new CustomerService(connectionString, "customers-mock");
    }

    public async Task DisposeAsync()
    {
        await _mongoDbContainer.StopAsync();
    }

    [Fact]
    public async Task CreateCustomer_ShouldCreateCustomer()
    {
        // Arrange
        var newCustomer = new Customer
        {
            Id = 1234,
            Name = "John Doe",
        };
        
        // Act
        await _sut.CreateCustomer(newCustomer);
        var customer = await _sut.GetCustomerByIdAsync(newCustomer.Id);
        
        // Assert
        Assert.NotNull(customer);
        Assert.Equal(newCustomer.Id, customer.Value.Id);
    }

    [Fact]
    public async Task CreateCustomers_ShouldCreateCustomers()
    {
        // Arrange
        var newCustomers = new List<Customer>
        {
            new() { Id = 1234, Name = "John Doe" },
            new() { Id = 3456, Name = "Jack Doe" },
            new() { Id = 3457, Name = "Jane Doe" },
        };
        
        // Act
        await _sut.CreateCustomers(newCustomers);
        var customers = await _sut.GetCustomersAsync();
        
        // Assert
        Assert.Equal(newCustomers.Count, customers.Count);
    }

    [Fact]
    public async Task UpdateCustomer_ShouldUpdateCustomer()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1234,
            Name = "John Doe",
        };
        var updatedCustomer = new Customer
        {
            Id = 1234,
            Name = "Jack Doe",
        };
        
        await _sut.CreateCustomer(customer);
        
        // Act
        var customerWasUpdated = await _sut.UpdateCustomerAsync(customer.Id, updatedCustomer);
        
        // Assert
        Assert.True(customerWasUpdated);
    }

    [Fact]
    public async Task DeleteCustomer_ShouldDeleteCustomer()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1234,
            Name = "Jane Doe",
        };
        
        await _sut.CreateCustomer(customer);
        
        // Act
        var customerWasDeleted = await _sut.DeleteCustomerAsync(customer.Id);
        
        // Arrange
        Assert.True(customerWasDeleted);
    }

    [Fact]
    public async Task GetCustomers_ShouldReturnAllCustomers()
    {
        // Arrange
        var newCustomers = new List<Customer>
        {
            new() { Id = 1234, Name = "John Doe" },
            new() { Id = 3456, Name = "Jack Doe" },
            new() { Id = 3457, Name = "Jane Doe" },
        };
        
        // Act
        await _sut.CreateCustomers(newCustomers);
        var customers = await _sut.GetCustomersAsync();
        
        Assert.Equal(newCustomers.Count, customers.Count);
    }

    [Fact]
    public async Task GetCustomerById_ShouldReturnFoundCustomer()
    {
        // Arrange
        var newCustomers = new List<Customer>
        {
            new() { Id = 1234, Name = "John Doe" },
            new() { Id = 3456, Name = "Jack Doe" },
            new() { Id = 3457, Name = "Jane Doe" },
        };
        
        await _sut.CreateCustomers(newCustomers);
        
        // Act
        var customer = await _sut.GetCustomerByIdAsync(3456);
        var linqCustomer = newCustomers.First(c => c.Id == 3456);
        
        // Assert
        Assert.NotNull(customer);
        Assert.Equal(customer.Value.Name, linqCustomer.Name);
    }
}