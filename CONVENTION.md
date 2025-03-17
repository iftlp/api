# .NET Conventions

## 1. Naming Conventions

### 1.1 Class Naming
- Use PascalCase.
- Name the class explicitly and meaningfully.
```csharp
public class CustomerManager
{
    // Code
}
```

### 1.2 Method Naming
- Use PascalCase.
- Name the method based on its action.
```csharp
public void CalculateTotal()
{
    // Code
}
```

### 1.3 Variable and Field Naming
- Use camelCase for local variables and parameters.
- Use `_camelCase` for private fields.
```csharp
private string _customerName;
public void SetCustomerName(string customerName)
{
    _customerName = customerName;
}
```

### 1.4 Constant Naming
- Use PascalCase.
- Prefix the declaration with `const`.
```csharp
public const int MaxRetryCount = 5;
```

### 1.5 Interface Naming
- Prefix the name with `I`.
```csharp
public interface IRepository
{
    void Save();
}
```

### 1.6 Enumeration Naming
- Use PascalCase.
```csharp
public enum OrderStatus
{
    Pending,
    Shipped,
    Delivered
}
```

## 2. Comments and Documentation

### 2.1 Using XML Comments
- Always document public classes and methods.
```csharp
/// <summary>
/// Customer management class.
/// </summary>
public class CustomerManager
{
    /// <summary>
    /// Adds a customer.
    /// </summary>
    /// <param name="customer">Customer object to add.</param>
    public void AddCustomer(Customer customer)
    {
        // Add code
    }
}
```

### 2.2 Code Comments
- Use `//` comments to explain complex code blocks.
```csharp
// Check if the customer already exists
if (customerList.Contains(customer))
{
    return;
}
```

## 3. Unit Tests (TUs)

### 3.1 Using NUnit/XUnit
- Use a framework like NUnit or XUnit.
- Name tests following `[MethodUnderTest]_Condition_ExpectedResult`.
```csharp
using Xunit;

public class CustomerManagerTests
{
    [Fact]
    public void AddCustomer_WithValidCustomer_ShouldAddSuccessfully()
    {
        // Arrange
        var customerManager = new CustomerManager();
        var customer = new Customer("John Doe");
        
        // Act
        customerManager.AddCustomer(customer);
        
        // Assert
        Assert.Contains(customer, customerManager.GetAllCustomers());
    }
}
```
