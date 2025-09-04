# 🛠 User Management API

A minimal ASP.NET Core Web API for managing users.  

---

## 📋 Activities Overview

This API supports the following activities:

1. **Authentication** – Secure access to endpoints using an API key.
2. **List Users** – Retrieve all registered users.
3. **Get User by ID** – Retrieve a single user’s details.
4. **Create User** – Add a new user with validation checks.
5. **Update User** – Modify an existing user’s details.
6. **Delete User** – Remove a user from the system.
7. **Error Handling** – Consistent JSON error responses for invalid requests.
8. **Logging** – Console logging for all requests and key actions.

---

## 🔑 Authentication

All endpoints (except `/`) require an **API key** in the request header.

## 🧪 How to Test
Using VS Code REST Client or Postman or other browser. Can use **requests.http** file for testing.


