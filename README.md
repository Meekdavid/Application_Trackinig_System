# Application Tracking System (ATS) API Documentation

Welcome to the Application Tracking System API documentation. This API facilitates the management of job postings, applications, and user authentication within an organization.

## Summary

The **Application Tracking System (ATS)** API provides endpoints to manage job posts, applications, and user authentication securely. It supports features such as creating and managing job posts, applying for jobs, and authenticating users with JWT token-based security.

### Key Features

- **User Authentication and Registration:** Secure user registration and login processes with JWT token-based authentication to protect access to the system.
- **Job Post Management:** CRUD operations for managing job posts including creation, retrieval, update, and deletion.
- **Application Management:** Endpoints for handling job applications including creating and retrieving applications.
- **Real-Time Updates:** Utilizes WebSockets for real-time updates and notifications to keep users informed.
- **Input Validation:** Includes robust input validation to prevent XML, XSS, and SQL injection attacks.

## Getting Started

### Prerequisites

Before you begin, ensure you have the following installed:

- [.NET Core SDK](https://dotnet.microsoft.com/download)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/your/repository.git
   cd repository-folder
