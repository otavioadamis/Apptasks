# Project Name

**Project Management App**

## Description

The Project Management App is a web application built using .NET 6 and MongoDB as the database. It allows administrators to create projects, define teams of users for those projects, create tasks, and assign them to users. The application uses JWT Bearer for authentication, BCrypt to encrypt passwords, and custom authorization methods to manage roles (admin, user). It follows the MVC architecture and utilizes inversion of control (Inversive Dependency Injection) and DTOs.

## Features

- User management
- Project creation and management
- Task assignment
- Authentication and authorization
- Background email service for task deadline notifications
- Docker containerization

## Technologies Used

- .NET 6
- MongoDB
- JWT Bearer Authentication
- BCrypt
- NCrontab
- Docker
- MVC Architecture
- Inversive Dependency Injection

## Email Service

The project includes an email service for sending task deadline notifications. It consists of two main components:

**EmailService**: This service sends email notifications to users. It uses MailKit for email handling and communicates with the database to retrieve user information.
    
**BackgroundEmailService**: A background service that runs at specific intervals to check for upcoming task deadlines and send email notifications.

## Getting Started

### Prerequisites

- [.NET 6](https://dotnet.microsoft.com/download/dotnet/6.0)
- [MongoDB](https://www.mongodb.com/try/download/community)
- Docker (if running the application in containers)

### Installation

    1. Clone the repository: `git clone https://github.com/otavioadamis/Apptasks.git`
    2. Navigate to the project directory: `cd Apptasks`
    3. Restore dependencies: `dotnet restore`
    4. Configure the application settings in `appsettings.json`.
    5. Build the project: `dotnet build`
    6. Run the application: `dotnet run`

### Docker Containerization

To run the application and MongoDB in Docker containers:

    1. Build the Docker images: `docker-compose build`
    2. Start the containers: `docker-compose up`

## Usage

    1. Open your API testing framework (insomnia, postman, for example) and navigate to `https://localhost:7198` (or `https://localhost:8080` if running in docker container).
    2. Sign up as an Admin User to create projects, assign tasks, and manage teams.

## Custom Exception Handling

The application includes a GlobalHandler middleware to handle exceptions gracefully. It provides custom error messages and ensures that unhandled exceptions do not expose sensitive information.

## Contributing

We welcome contributions from the community. If you'd like to contribute, please follow these steps:

    1. Fork the repository.
    2. Create a new branch for your feature or bug fix: `git checkout -b feature/   my-feature` or `git checkout -b bugfix/fix-issue`.
    3. Make your changes.
    4. Commit your changes: `git commit -m "Add your message here"`
    5. Push your branch: `git push origin feature/my-feature` or `git push origin bugfix/fix-issue`.
    6. Create a pull request on GitHub.

## Contact

- Otavio Adamis otavio.adamis01@gmail.com






