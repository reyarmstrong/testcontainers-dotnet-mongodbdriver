using MongoDB.Driver;

namespace TestContainersPoC;

public class CustomerService
{
    private readonly IMongoCollection<Customer> _customerCollection;

    public CustomerService(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);    
        _customerCollection = database.GetCollection<Customer>("Customers");
    }

    public async Task CreateCustomer(Customer customer)
    {
        await _customerCollection.InsertOneAsync(customer);
    }

    public async Task CreateCustomers(List<Customer> customers)
    {
        await _customerCollection.InsertManyAsync(customers);
    }

    public async Task<List<Customer>> GetCustomersAsync()
    {
        return await _customerCollection.Find(c => true).ToListAsync();
    }

    public async Task<Customer?> GetCustomerByIdAsync(long customerId)
    {
        return await _customerCollection.Find(c => c.Id == customerId).FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateCustomerAsync(long customerId, Customer customer)
    {
        var result = await _customerCollection.ReplaceOneAsync(c => c.Id == customerId, customer);
        return result.IsAcknowledged && result.ModifiedCount == 1;
    }

    public async Task<bool> DeleteCustomerAsync(long customerId)
    {
        var result = await _customerCollection.DeleteOneAsync(c => c.Id == customerId);
        return result.IsAcknowledged && result.DeletedCount == 1;
    }
}