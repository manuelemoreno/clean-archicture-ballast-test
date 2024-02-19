# Introduction
The test application is intended to demostrate Clean Architecture principles applied to NET 7 Web API applications. 

The Domain Entity chosen is **Fruit**. And the application name is "**AwesomeFruits**"

## User Stories:
From the testing statements, I created two user stories that I could facilitate the appliance of TDD.

### User Story #1
As a non-user, I want to create an account so that I can login to the fruits application. 

**Acceptance criterias**:
- An error is shown if the creation fields are missing.
- An error is shown if the username already exists.

### User Story #2
As an authorized user, I want to be able to create, retrieve, update and delete fruits. 

**Acceptance criterias**:
- An error is shown if the fruit fields are missing.
- An error is shown if the fruit name already exists.
- An erros is shown if the user is not authorized.

## TDD and Clean Architecture Approach
Using each user story a high level of unit tests were creating. Starting just by naming them. Then they were getting added as each layer of the clean architecture was constructed.

A structure of the app was put in place to follow clean architecture:
- **Domain Layer**: Here resides the domain entities used in the solution, as well as Domain Interfaces and Domain Exceptions. Does not depend on any other layer.
- **Application Layer**: Here is all the business logic of the applications. Using service pattern, the services are accessible by their interfaces. It has its own DTos and respective mapping classes. Only depends on the Domain Level.
- **Infrastructure Layer**: Here resides the data layer. Using repository pattern it service as the concrete classes for the domain interfaces. Only depends on the Domain Layer. In this opportunity we are implemente MongoDB as the data persistance.
- **WebApi**: In this API resides the logic for performing the CRUD operations on the web application "AwesomeFruits". It handles authentication via bearer tokens. Only authorized users can perform the different operations. 
- **WebApi Users**: In this API resides the logic creating users and generating access tokens for authentication to the Webapi. 

# Getting Started
Go to the folder **AwesomeFruits** and run docker compose in console
```bash
 docker-compose up -d
```

Example of a request in the WebApi Users Use __*POST*__ on /api/Auth/register to generate an bearer token:
```bash
{
  "userName": "testuser",
  "password": "testpass",
  "firstName": "mm",
  "lastName": "last",
  "email": "test@test.com"
}
```
If all is success, the response should return the bearer token
```bash
{
    "accessToken": [Bearer Token]
}
```

Use this token and add it to the Authorization header in the operations of the WebAPI.

Swagger is also available for both APIs. 

**Swagger for the WebAPI Users**:
http://localhost:8021/swagger/index.html

**Swagger for the WebAPI**:
http://localhost:8022/swagger/index.html

Run docker compose down to release resources after reviewing

```bash
 docker-compose down
```

