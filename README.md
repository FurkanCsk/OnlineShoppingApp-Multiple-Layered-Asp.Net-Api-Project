# Creating the README content in English and formatting it as a markdown file.

readme_content = """# Multiple Layered ASP.NET API Project - Online Shopping Platform

## Project Overview

This project is an ASP.NET Core Web API developed for an online shopping platform. The project is built with a multi-layered architecture using the Entity Framework Code First approach.

## Project Requirements

### Layered Architecture
The project consists of the following three layers:

1. **Presentation Layer**: This layer contains all the controllers for the API.
2. **Business Layer**: This layer contains the business logic of the application.
3. **Data Access Layer**: This layer manages database operations using Entity Framework, including repository and unit of work classes.

### Data Models

The project includes the following data models:

- **User**:
  - `Id` (int, primary key)
  - `FirstName` (string)
  - `LastName` (string)
  - `Email` (string, unique)
  - `PhoneNumber` (string)
  - `Password` (string, encrypted with Data Protection)
  - `Role` (Enum - Admin or Customer)

- **Product**:
  - `Id` (int, primary key)
  - `ProductName` (string)
  - `Price` (decimal)
  - `StockQuantity` (int)

- **Order**:
  - `Id` (int, primary key)
  - `OrderDate` (DateTime)
  - `TotalAmount` (decimal)
  - `UserId` (int)

- **OrderProduct**:
  - `OrderId` (int)
  - `ProductId` (int)
  - `Quantity` (int)

### Authentication and Authorization
- **ASP.NET Core Identity** or a custom identity service is used to perform user authentication.
- **JWT (JSON Web Token)** is implemented for authorization mechanisms.

### Features
- **Middleware**: Custom middleware is used to log incoming requests throughout the application.
- **Action Filter**: An action filter is implemented to allow API access during specific time periods.
- **Model Validation**: Necessary model validation rules are applied for users and products.
- **Dependency Injection**: Service management is handled through dependency injection.
- **Data Protection**: User passwords are securely stored using Data Protection.
- **Global Exception Handling**: A global exception handling mechanism is implemented to catch all errors and return appropriate responses.

### API Endpoints
The project supports the following HTTP methods:
- **GET**
- **POST**
- **PUT**
- **PATCH**
- **DELETE**

