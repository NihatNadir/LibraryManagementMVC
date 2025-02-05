# Library Management MVC - ASP.NET Core Application


This is a comprehensive library management system built with ASP.NET Core MVC and Entity Framework Core. It enables users to manage books, authors, orders, genres, and user authentication. The system uses JWT for secure authentication and follows a layered architecture with Dependency Injection, EF Core Tools & Design, and SQL Server for data storage.

## Features

- **CRUD Operations**: Manage books, authors, genres, and orders with Create, Read, Update, and Delete functionality.
- **JWT Authentication**: Secure login and registration using JWT tokens.
- **Book-Author Relationship**: Each book is associated with an author.
- **Book-Genre Association**: Books can belong to multiple genres.
- **Order Management**: Track book orders placed by users, including quantity and total amount.
- **User Role Management**: Different user roles (admin, regular users) with different access levels.
- **Data Protection**: Secure sensitive user data using Data Protection.

## Technologies Used

- **ASP.NET Core MVC**: For building the web application.
- **Entity Framework Core**: For ORM-based database interaction with SQL Server.
- **JWT (JSON Web Tokens)**: For secure user authentication.
- **SQL Server**: Used for database management.
- **Dependency Injection**: For service-oriented architecture.