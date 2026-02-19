# Global Exception Handling in .NET 8

## Overview

This project demonstrates a modern, production-grade exception handling strategy for ASP.NET Core 8 APIs. It implements multiple exception handling patterns and shows best practices for managing errors across a distributed system.

---

## Table of Contents

1. [Architecture](#architecture)
2. [Exception Hierarchy](#exception-hierarchy)
3. [Implementation Approaches](#implementation-approaches)
4. [How to Use](#how-to-use)
5. [Exception Types](#exception-types)
6. [Error Response Format](#error-response-format)
7. [Environment-Aware Error Details](#environment-aware-error-details)
8. [Best Practices](#best-practices)
9. [Examples](#examples)

---

## Architecture

The project uses a **layered exception handling approach** with two complementary methods:

### 1. Modern Approach (.NET 8+): `IExceptionHandler`
- Registered in the middleware pipeline
- Globally handles unhandled exceptions
- Supports chaining multiple handlers
- Standardized error responses

### 2. Legacy Approach: Custom Middleware
- Pre-.NET 8 pattern for reference
- Catches exceptions during request processing
- Returns `ProblemDetails` response format

---

## Exception Hierarchy

````````

### Benefits of This Structure:
- **Type-safe exception handling** based on application domain
- **Automatic HTTP status code mapping**
- **Consistent error response format**
- **Easy to extend** with new exception types

---

## Implementation Approaches

### Approach 1: Modern Way (.NET 8+) - `IExceptionHandler`

**File:** `Handlers/GlobalExceptionHandler.cs`

````````

**Advantages:**
- ✅ Built into .NET 8
- ✅ Handler chaining support
- ✅ Dependency injection out of the box
- ✅ Cleaner and more testable
- ✅ Async-first design

**Registration in `Program.cs`:**

````````

---

### Approach 2: Legacy Way - Custom Middleware

**File:** `ExceptionHandlingMiddleware.cs`

````````

**Advantages:**
- Works with all .NET Core versions
- Simple and straightforward
- Direct control over middleware order

**Use case:** When targeting .NET 6 or earlier versions.

---

## How to Use

### 1. Setup in `Program.cs`

````````

### 2. Throw Custom Exceptions in Your Code

````````

### 3. Handlers Process the Exception

- Exception is caught by the exception handler middleware
- Appropriate handler processes it based on exception type
- Standard error response is returned to the client

---

## Exception Types

### `AppException`
**Base class** for all application exceptions.

**Properties:**
- `Message` - Error description
- `StatusCode` - HTTP status code to return

**Usage:**


````````

### `BadRequestException`
Thrown when the request is **invalid or malformed**.

**Returns:** HTTP 400 Bad Request

**Usage:**


````````

### `NotFoundException`
Thrown when a **requested resource does not exist**.

**Returns:** HTTP 404 Not Found

**Usage:**


````````

### `ValidationException`
Thrown when request data fails to pass **validation rules**.

**Returns:** HTTP 422 Unprocessable Entity

**Usage:**


````````

## Error Response Format

All exceptions return a **ProblemDetails** response:

````````json
{
  "errors": [
    {
      "message": "Error description",
      "status": 400,
      "code": "BadRequest",
      "details": "Additional error details"
    }
  ]
}
````````

**Fields:**
- **type** - RFC 7231 problem type URI
- **title** - Short error summary
- **status** - HTTP status code
- **detail** - Detailed error message
- **instance** - Request path
- **traceId** - Unique identifier for tracing
- **timestamp** - When the error occurred

---

## Environment-Aware Error Details

The handler **exposes different details** based on environment:

### Development Environment

- Shows complete exception messages
- Useful for debugging

### Production Environment

- Hides detailed exception messages
- Shows generic error information

**Configuration:** Set `ASPNETCORE_ENVIRONMENT` to switch environments.

---

## Best Practices

- **Use specific exception types** for different error conditions
- **Log exceptions** for troubleshooting
- **Return consistent error responses** to API consumers
- **Avoid exposing sensitive information** in error details
- **Test exception handling** scenarios thoroughly

---

## Examples

### Example 1: Not Found Error

**Request:**
````````markdown
GET /api/weatherforecast/9999 HTTP/1.1
Host: localhost
Accept: application/json
````````

# Response
````````markdown
HTTP/1.1 404 Not Found
Content-Type: application/json
````````json
{
  "errors": [
    {
      "message": "Weather forecast not found.",
      "status": 404,
      "code": "NotFound",
      "details": "The requested resource does not exist."
    }
  ]
}
````````

---

### Example 2: Validation Error

**Request:**
````````markdown
POST /api/weatherforecast HTTP/1.1
Host: localhost
Content-Type: application/json

{
  "date": "invalid-date-format",
  "temperatureC": "twenty",
  "summary": "Warm"
}
````````

# Response
````````markdown
HTTP/1.1 422 Unprocessable Entity
Content-Type: application/json
````````json
{
  "errors": [
    {
      "message": "The date field is not a valid date.",
      "status": 422,
      "code": "ValidationError",
      "details": "Date: 'invalid-date-format' is not a valid date."
    },
    {
      "message": "The temperatureC field must be a number.",
      "status": 422,
      "code": "ValidationError",
      "details": "TemperatureC: 'twenty' is not a number."
    }
  ]
}
````````

---

### Example 3: Bad Request Error

**Request:**
````````markdown
POST /api/weatherforecast HTTP/1.1
Host: localhost
Content-Type: application/json

{
  "temperatureC": 25
}
````````

# Response
````````markdown
HTTP/1.1 400 Bad Request
Content-Type: application/json
````````json
{
  "errors": [
    {
      "message": "Invalid request format.",
      "status": 400,
      "code": "BadRequest",
      "details": "The request body is not valid JSON."
    }
  ]
}
````````

---

## Security Enhancements

- Only exposes messages from `AppException`
- Hides sensitive system exception details
- Security best practice

---

## Handler Chaining

Multiple handlers are **executed in order**:

1. **NotFoundExceptionHandler** → Handles only `NotFoundException`
2. **GlobalExceptionHandler** → Catches everything else

Each handler returns:
- **`true`** - Exception was handled, stop processing
- **`false`** - Exception not handled, try next handler

---

## Best Practices

### ✅ DO:

1. **Throw specific exceptions** instead of generic `Exception`
2. **Log exceptions with context**
3. **Use meaningful error messages**
4. **Return appropriate HTTP status codes**
5. **Handle exceptions at the right layer**
   - Service layer: Business logic validation
   - Controller layer: HTTP-specific handling (rarely needed with global handlers)

### ❌ DON'T:

1. **Catch and swallow exceptions silently**
2. **Expose sensitive information in production**
   - Database connection strings
   - System paths
   - Internal implementation details
3. **Create too many exception types**
   - Keep to domain-specific exceptions
   - Reuse standard HTTP status codes
4. **Use exceptions for normal control flow**


---

## Examples

### Example 1: Not Found Error

**Request:**


**Code:**
[HttpGet("{id}")] public IActionResult GetProduct(int id) { var product = _productService.GetById(id); if (product is null) throw new NotFoundException($"Product {id} not found");
return Ok(product);

**Response:**
HTTP/1.1 404 Not Found Content-Type: application/json
{ "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5", "title": "Resource Not Found", "status": 404, "detail": "Product 999 not found", "instance": "/api/products/999", "traceId": "0HN0N5GKFL8PC:00000001" }



---

### Example 2: Bad Request Error

**Request:**

**Code:**
[HttpPost] public IActionResult CreateProduct(CreateProductRequest request) { if (string.IsNullOrEmpty(request.Name)) throw new BadRequestException("Product name is required");
if (request.Price <= 0)
    throw new BadRequestException("Product price must be greater than zero");

var product = _productService.Create(request);
return Created($"/api/products/{product.Id}", product);


**Response:**
HTTP/1.1 400 Bad Request Content-Type: application/json
{ "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1", "title": "Invalid argument provided", "status": 400, "detail": "Product name is required", "instance": "/api/products", "traceId": "0HN0N5GKFL8PC:00000002" }


---

## Summary

| Aspect | Details |
|--------|---------|
| **Modern Approach** | `IExceptionHandler` (.NET 8+) |
| **Legacy Approach** | Custom Middleware |
| **Base Exception** | `AppException` |
| **Common Exceptions** | `BadRequestException`, `NotFoundException` |
| **Response Format** | `ProblemDetails` (RFC 7807) |
| **Error Details** | Environment-aware (dev vs. prod) |
| **Handler Order** | Chain of responsibility pattern |
| **HTTP Status Codes** | Auto-mapped from exception type |

---

## References

- [RFC 7807 - Problem Details for HTTP APIs](https://tools.ietf.org/html/rfc7807)
- [RFC 9110 - HTTP Semantics](https://tools.ietf.org/html/rfc9110)
- [ASP.NET Core Exception Handling (.NET 8)](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.diagnostics.iexceptionhandler)
- [ProblemDetails](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.problemdetails)