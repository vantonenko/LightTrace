# Demo Web API for LightTrace Library

This is a demonstration ASP.NET Core Web API application that showcases the usage of the LightTrace library for performance monitoring and tracing.

## Overview

The demo application includes:

- **Controllers**: API endpoints for Users, Products, and Tracing
- **Business Layer**: Service classes with business logic and validation
- **Data Layer**: Repository classes simulating database operations
- **Models**: User and Product entities

## LightTrace Integration

The LightTrace library is integrated throughout the application layers to demonstrate:

1. **Method-level tracing**: Each significant method is wrapped with a `Tracer` using statement
2. **Nested tracing**: Shows how nested operations are tracked hierarchically
3. **Performance monitoring**: Tracks execution time for all traced operations
4. **Automatic reporting**: Background thread generates markdown reports

## API Endpoints

### Users API (`/api/users`)
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create new user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user
- `POST /api/users/{id}/validate-email` - Validate email address

### Products API (`/api/products`)
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `GET /api/products/category/{category}` - Get products by category
- `GET /api/products/categories` - Get all categories
- `POST /api/products` - Create new product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product
- `POST /api/products/{id}/calculate-discount` - Calculate discounted price

### Trace API (`/api/trace`)
- `GET /api/trace/report` - Get current trace data as JSON
- `GET /api/trace/report/file` - Get trace report file content
- `POST /api/trace/simulate-heavy-operation` - Simulate complex operation for testing

## Running the Application

1. Navigate to the project directory:
   ```
   cd src/DemoWebApi
   ```

2. Run the application:
   ```
   dotnet run
   ```

3. Open Swagger UI at: `https://localhost:7xxx/swagger`

## Trace Reports

The LightTrace library automatically generates trace reports:

- **Live Data**: Access via `/api/trace/report` endpoint
- **File Reports**: Markdown files written to temp directory (configurable via environment variables)
- **Background Reporting**: Automatic periodic updates every 15 seconds (configurable)

## Environment Variables

Configure LightTrace behavior using these environment variables:

- `LIGHT_TRACE_REPORT_INTERVAL`: Report generation interval in seconds (default: 15)
- `LIGHT_TRACE_REPORT_FOLDER`: Directory for report files (default: system temp folder)

## Example Usage

1. Make API calls to generate trace data:
   ```bash
   # Get all users
   curl https://localhost:7xxx/api/users
   
   # Create a new product
   curl -X POST https://localhost:7xxx/api/products \
     -H "Content-Type: application/json" \
     -d '{"name":"Test Product","description":"Test","price":29.99,"category":"Test"}'
   
   # Simulate heavy operation
   curl -X POST https://localhost:7xxx/api/trace/simulate-heavy-operation?durationMs=2000
   ```

2. View trace reports:
   ```bash
   # Get current trace data
   curl https://localhost:7xxx/api/trace/report
   
   # Get trace report file
   curl https://localhost:7xxx/api/trace/report/file
   ```

The trace reports will show the hierarchical execution flow and timing information for all traced operations.