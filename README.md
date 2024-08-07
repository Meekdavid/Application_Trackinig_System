# Application Tracking System

## Introduction

Welcome to the Application Tracking System, a web application designed to facilitate the registration and management of Job Posts by coordinators, employers, recruiters, and candidates. 

### Coordinator Login
- **Username:** coordinator@gmail.com
- **Password:** Meekdavid

## Tools Used

- **Programming Languages:** C# (ASP.NET MVC), JavaScript, CSS, HTML
- **Database:** Microsoft SQL (SQL Database)

## How to Execute the Application
The application is hosted on [ATS](https://102.210.194.25/application_trackinig_system/).

### Execute Locally

Follow the steps below to run the application on your local machine.

### Prerequisites

Before you begin, ensure you have met the following requirements:

- [Install .NET Core](https://dotnet.microsoft.com/download)

### Installation

1. Clone this repository:
   ```shell
   git clone https://github.com/Meekdavid/Application_Trackinig_System
   ```
2. Navigate to the Project Directory:
   ```shell
   cd Application_Trackinig_System
   ```
3. Restore Dependencies:
   ```shell
   dotnet restore
   ```
4. Build Projects:
   ```shell
   dotnet build
   ```
5. Run the API locally:
   ```shell
   dotnet run
   ```
   The application will be available at:
   - `http://localhost:5254/Home` (HTTP)
   - `https://localhost:7269/Home` (HTTPS)

   Alternatively, load the project/solution in Visual Studio and run.

## Security Features Implemented

1. **JWT Token Authentication**
2. **Role-Based Authorization**
   - Ensures authenticated users view appropriate resources.
3. **Secure Cipher Suites**
   - TLS 1.1 and TLS 1.2 implemented.
4. **Input Validations**
   - Guards against attacks such as XML and SQL injection.
5. **Maximum Length for Input Fields**
   - Prevents Buffer Overflow and Denial-of-Service (DoS) attacks.
6. **Security Headers Policies**

<details>
  <summary><b>🛠️&nbsp;&nbsp;More&nbsp;About&nbsp;Project</b></summary>
  
### Author  
  * David Mboko | [Youtube](https://www.youtube.com/@davidmboko6502/featured) | [LinkedIn](https://www.linkedin.com/mwlite/in/david-mboko-25bb9019b) | [Academia](https://aksu.academia.edu/DavidMboko) |

### Resources
- [Click to View](https://dotnet.microsoft.com/en-us/learn)
</details>

Thank you for using the Application Tracking System. We hope it helps streamline your application management process!
