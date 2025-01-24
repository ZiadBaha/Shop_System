# Shop System - ASP.NET Core Web API Project
_________________________________________________________

## Introduction
In today’s fast-paced business environment, efficient store management is crucial for success. The Shop System is a cutting-edge web API solution designed to simplify inventory tracking, customer management, order processing, and more. By leveraging modern technologies and a secure, scalable architecture, this system caters to businesses of all sizes, ensuring streamlined operations and enhanced productivity.
_________________________________________________________

## Project Overview
The Shop System is a comprehensive ASP.NET Core Web API application that integrates Domain-Driven Design (DDD) principles and a layered architecture. It employs secure JWT authentication, efficient data mapping with AutoMapper, and robust database management using Entity Framework Core. With its feature-rich implementation, the system ensures seamless workflows for store owners, employees, and customers.
_________________________________________________________

## Technologies Used
- **ASP.NET Core 8**: Web API framework for building RESTful services.
- **C# Programming Language**: The backend logic of the application is written in C#.
- **Entity Framework Core**: ORM used for database operations.
- **ASP.NET MVC**: Used for building the web application's architecture.
- **ASP.NET Identity**: Provides user authentication and authorization functionality.
- **SQL Server** : Database engine for data persistence.
- **Dependency Injection**: Built-in DI for loose coupling of modules.
- **JWT Authentication**: For secure authorization.
- **AutoMapper**: For object-to-object mapping.
- **Swagger**: For API documentation.
- **Unit Testing**: XUnit or NUnit for testing the core business logic.
- **RESTful APIs**: Implemented to enable communication between different components of the application.
- **Logging (ILogger)**: Integrated for debugging and monitoring application behavior.
_________________________________________________________

## Project Structure
| **Sub-Project**        | **Purpose**                                                           | **Key Features**                                                  |
|------------------------|-----------------------------------------------------------------------|-------------------------------------------------------------------|
| **ShopSystem.Web**      | Handles the web application and API endpoints.                       | - Includes middleware, controllers, and Swagger documentation.   |
|                        |                                                                       | - Manages HTTP requests and responses for the web application.   |
| **ShopSystem.Core**     | Defines the business models, enums, and shared logic.                | - Serves as the foundation of the solution.                      |
|                        |                                                                       | - Contains core entities like `Product`, `Order`, and `Customer`.|
| **ShopSystem.Repository** | Manages data access using Entity Framework Core.                     | - Contains database migrations and repositories for CRUD operations. |
|                        |                                                                       | - Uses **Entity Framework Core** for data operations.            |
| **ShopSystem.Service**  | Implements business logic and application services.                  | - Connects the repository and web layers.                        |
|                        |                                                                       | - Implements service methods for handling complex business logic. |


_________________________________________________________

## Controllers and Endpoints

### 1. AccountController
* Manages user authentication, authorization, and account operations.
* Endpoints:

- **POST /register**  
Registers a new user, uploads a profile image, and assigns roles.
- **POST /login**                 
Authenticates a user with provided credentials.
- **POST /forgetPassword**        
Sends a password reset email to the user.
- **POST /verfiyOtp**            
Verifies a One-Time Password (OTP) for password recovery.
- **PUT /resetPassword**          
Resets the user password using a token.
- **GET /confirm-emai**          
Confirms a user's email address using a token.
- **GET /getUserInfo/{userId}**   
Retrieves information for a specific user.
- **GET /getUsers**               
Retrieves all users.
- **GET /usersCount**             
Returns the total number of registered users.
- **DELETE /deleteUser/{userId}** 
Deletes a user by ID.
- **PUT /updateUserInfo**         
Updates user account details.

### 2. CategoriesController
* Handles CRUD operations for product categories.
* Endpoints:

- **GET /**
Retrieves all categories with pagination, sorting, and filtering options.
- **GET /{id}**
Retrieves a category by its ID.
- **POST /**
Creates a new category.
- **PUT /{id}**
Updates an existing category.
- **DELETE /delete-multiple**
Deletes multiple categories by their IDs.

### 3. CustomersController
* Facilitates customer management and financial operations.
* Endpoints:

- **GET /**
Retrieves all customers with pagination, filtering, and sorting.
- **GET /{id}**
Retrieves a customer by ID.
- **POST /**
Creates a new customer.
- **PUT /{id}**
Updates an existing customer's details.
- **DELETE /delete-multiple**
Deletes multiple customers by their IDs.
- **GET /{customerId}/debt**
Calculates and retrieves a customer's total debt.
- **GET /{customerId}/orders**
Retrieves all orders associated with a customer.
- **GET /{customerId}/payments**
Retrieves all payments made by a customer.

### 4. ProductsController
* Manages the shop's product inventory.
* Endpoints:

- **GET /**
Retrieves all products with pagination and filtering options.
- **GET /{id}**
Retrieves a product by ID.
- **POST /AddProducts**
Adds multiple products in bulk.
- **PUT /{id}**
Updates a product's details.
- **PATCH /{productId}/stock**
Updates the stock level for a product.
- **GET /category/{categoryId}**
Retrieves products belonging to a specific category.
- **GET /{productId}/stock**
Retrieves the available stock for a product.


### 5. OrderController
* Handles order creation, management, and related tasks like invoices.
* Endpoints:

- **POST /**
Creates a new order.
- **GET /{id:int}**
Retrieves an order by its ID.
- **GET /**
Retrieves all orders with pagination and filtering options.
- **PUT /{id:int}**
Updates an existing order.
- **DELETE /delete-multiple**
Deletes multiple orders.
- **GET /{id:int}/calculate-total**
Calculates the total value of an order.
- **GET /{id:int}/invoice**
Retrieves the invoice for an order.
- **GET /generate/{orderId}**
Generates a downloadable PDF invoice for an order.
- **POST /print/{orderId}**
Sends an invoice to a specified printer.


### 6. PaymentsController
* Handles all payment-related operations.
* Endpoints:

- **GET /**
Retrieves all payments with pagination.
- **GET /{id:int}**
Retrieves a specific payment by ID.
- **POST /**
Records a new payment.
- **PUT /{id:int}**
Updates an existing payment.
- **DELETE /delete-multiple**
Deletes multiple payments.
- **GET /customer/{customerId:int}**
Retrieves payments made by a specific customer.
- **GET /order/{orderId:int}**
Retrieves payments made for a specific order.


### 7. MerchantController
* Manages merchant data and transactions.
* Endpoints:

- **GET /**
Retrieves all merchants with pagination and filtering.
- **GET /{id}**
Retrieves a specific merchant by ID.
- **POST /**
Creates a new merchant.
- **PUT /{id}**
Updates an existing merchant's details.
- **DELETE /delete-multiple**
Deletes multiple merchants by their IDs.
- **GET /{merchantId}/purchases**
Retrieves all purchases made by a merchant.

### 8. ExpensesController
* Tracks and calculates shop expenses.
* Endpoints:

- **GET /**
Retrieves all expenses with pagination and filtering.
- **GET /{id}**
Retrieves a specific expense by ID.
- **POST /**
Records a new expense.
- **PUT /{id}**
Updates an existing expense.
- **DELETE /delete-multiple**
Deletes multiple expenses.
- **GET /total**
Calculates the total expenses by category.

### 9. PurchaseController
- Manages purchase records and related data.
- Endpoints:

- **GET /**
Retrieves all purchases with pagination and filtering.
- **GET /{id}**
Retrieves a specific purchase by ID.
- **POST /**
Creates a new purchase record.
- **PUT /{id}**
Updates an existing purchase record.
- **DELETE /delete-multiple**
Deletes multiple purchase records.
- **GET /{purchaseId}/products**
Retrieves all products included in a purchase.
- **GET /{purchaseId}/total**
Calculates the total value of a purchase.
_________________________________________________________

| **Model Name**           | **Namespace**                        | **Purpose**                                                                                         | **Key Properties**                                                                                                                                 |
|--------------------------|--------------------------------------|-----------------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------|
| **Login**                | ShopSystem.Core.Models.Account      | Represents the data for user login.                                                                 | - `Email` (string) <br> - `Password` (string)                                                                                                      |
| **MailSettings**         | ShopSystem.Core.Models.Account      | Configures email settings for the system (SMTP).                                                     | - `Email` (string) <br> - `Password` (string) <br> - `SmtpServer` (string) <br> - `Port` (int) <br> - `DisplayedName` (string)                        |
| **Register**             | ShopSystem.Core.Models.Account      | Handles user registration and validation.                                                           | - `FirstName`, `LastName` (string) <br> - `Email` (string) <br> - `Password`, `ConfirmPassword` (string) <br> - `Role` (UserRole)                    |
| **ResetPassword**       | ShopSystem.Core.Models.Account      | Represents the data for resetting user passwords.                                                   | - `Email` (string) <br> - `Password`, `ConfirmPassword` (string)                                                                                   |
| **VerifyOtp**            | ShopSystem.Core.Models.Account      | Represents the data for OTP verification.                                                           | - `Email` (string) <br> - `Otp` (string)                                                                                                           |
| **BaseEntity**           | ShopSystem.Core.Models.Entites      | Provides the base class for entities with common properties.                                        | - `Id` (int)                                                                                                                                     |
| **Category**             | ShopSystem.Core.Models.Entites      | Represents a product category.                                                                      | - `Name` (string) <br> - `Products` (ICollection<Product>)                                                                                          |
| **Customer**             | ShopSystem.Core.Models.Entites      | Represents a customer, tracking their orders, payments, and outstanding balance.                   | - `Name`, `Phone` (string) <br> - `Orders`, `Payments` (ICollection<Order>, ICollection<Payment>) <br> - `MoneyOwed` (decimal)                       |
| **Expense**              | ShopSystem.Core.Models.Entites      | Represents an expense in the shop system.                                                           | - `Amount` (decimal) <br> - `Date` (DateTime) <br> - `Category` (ExpenseCategory?) <br> - `Info` (string)                                          |
| **Merchant**             | ShopSystem.Core.Models.Entites      | Represents a merchant in the system, with related purchase data.                                    | - `Name`, `Phone`, `Address` (string) <br> - `Purchases` (ICollection<Purchase>) <br> - `OutstandingBalance` (decimal)                              |
| **Order**                | ShopSystem.Core.Models.Entites      | Represents a customer order, including the order date and items.                                    | - `OrderDate` (DateTime) <br> - `CustomerId`, `UserId` (int, string) <br> - `TotalAmount`, `TotalDiscount` (decimal)                               |
| **OrderItem**            | ShopSystem.Core.Models.Entites      | Represents an item within an order.                                                                  | - `OrderId`, `ProductId` (int) <br> - `Quantity`, `Discount` (int, decimal) <br> - `Product` (Product)                                            |
| **Payment**              | ShopSystem.Core.Models.Entites      | Represents a payment made by a customer for an order or invoice.                                    | - `CustomerId` (int) <br> - `Amount` (decimal) <br> - `Info` (string) <br> - `Date` (DateTime)                                                    |
| **Product**              | ShopSystem.Core.Models.Entites      | Represents a product in the shop, with details like price, quantity, and category.                 | - `Name` (string) <br> - `Quantity` (int?) <br> - `SellingPrice`, `PurchasePrice` (decimal) <br> - `UniqueNumber` (string)                          |
| **Purchase**             | ShopSystem.Core.Models.Entites      | Represents a merchant’s purchase, tracking items and total amounts.                                | - `MerchantId` (int) <br> - `OrderDate` (DateTime) <br> - `TotalAmount` (decimal) <br> - `Products` (ICollection<PurchaseItem>)                       |
| **PurchaseItem**         | ShopSystem.Core.Models.Entites      | Represents an item within a merchant's purchase.                                                    | - `PurchaseId`, `ProductId` (int) <br> - `Quantity`, `PricePerUnit` (int, decimal)                                                                  |
| **AppUser**              | ShopSystem.Core.Models.Identity     | Extends IdentityUser to include custom user properties.                                              | - `UserRole` (UserRole) <br> - `FirstName`, `LastName` (string) <br> - `Image` (string) <br> - `Orders` (ICollection<Order>)                          |


_________________________________________________________

## NuGet Packages 
This document provides an overview of the NuGet packages utilized in the Shop System Project. These packages are crucial for various functionalities, including database management, API development, authentication, PDF generation, and email communication.

| Package Name                                         | Version  | Purpose                                                                 |
|-----------------------------------------------------|----------|-------------------------------------------------------------------------|
| Microsoft.AspNetCore.Hosting                        | 2.2.7    | Facilitates ASP.NET Core application hosting.                          |
| Microsoft.EntityFrameworkCore.Design               | 8.0.11   | Design-time tools for Entity Framework Core, such as migrations.       |
| Swashbuckle.AspNetCore                              | 6.4.0    | Provides Swagger/OpenAPI documentation for APIs.                       |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore   | 8.0.11   | Adds Identity support with Entity Framework Core for user management.  |
| Microsoft.AspNetCore.Mvc.Core                      | 2.2.5    | Core library for building MVC applications and APIs.                   |
| Microsoft.Extensions.Hosting.Abstractions          | 8.0.1    | Provides hosting and lifecycle management abstractions.                |
| AutoMapper                                          | 13.0.1   | Simplifies object-to-object mapping between DTOs and models.           |
| iText7                                              | 8.0.5    | Generates and manipulates PDF documents.                               |
| iText7.BouncyCastleAdapter                          | 8.0.5    | Adds cryptographic features like signing to iText7.                    |
| MailKit                                             | 4.8.0    | Handles email sending and receiving in .NET.                           |
| Microsoft.EntityFrameworkCore.SqlServer            | 8.0.11   | SQL Server provider for Entity Framework Core.                         |
| Microsoft.EntityFrameworkCore.Tools                | 8.0.11   | Tools for Entity Framework Core, like scaffolding and migrations.      |
| MimeKit                                             | 4.8.0    | Supports MIME message creation and manipulation for emails.            |
| Otp.NET                                             | 1.4.0    | Generates and validates One-Time Passwords (OTP) for 2FA.              |
| System.Drawing.Common                               | 8.0.11   | Provides basic drawing functionalities like image manipulation.         |
| Microsoft.AspNetCore.Authentication.JwtBearer       | 8.0.11   | Enables token-based authentication using JWT.                          |
| Microsoft.Extensions.Configuration.Abstractions     | 8.0.0    | Provides abstractions for managing app configurations.                 |

