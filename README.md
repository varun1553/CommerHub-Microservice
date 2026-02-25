# CommerHub Microservice 

CommerHub is a high-performance e-commerce microservice built with .NET 10, MongoDB, RabbitMQ following clean Architechture principles and Domain-Driven Design. 
It is designed to be scalable, maintainable and contianer-ready. 


# Overview

This microservice manages the lifecycle of orders and real-time inventory adjustments. It is designed to handlehigh-traffic concurrency using atomic database 
operations and also provide reliable event driven communication. 


# Tech Stack :

Framework: .NET 10(ASP.NET Core API)
Database: MongoDB
Messaging: RabbitMQ
Testing: nUnit & Moq
Containerization: Docker & Docker Compose

# API Endpoints

## Orders

### POST /api/orders/checkout
Processes a new order:
- Validates quantity > 0
- Verifies stock
- Atomically decrements inventory
- Creates order
- Publishes OrderCreated event

### GET /api/orders/{id}
Retrieves an order by ID  
Returns 404 if not found

### PUT /api/orders/{id}
Replaces an existing order  
Blocked if status = "Shipped"

---

## Products

### PATCH /api/products/{id}/stock
Atomically adjusts stock
Prevents negative stock under high concurrency

---

# Concurrency & Atomic Operations

Stock updates use MongoDB atomic operations:

- Filter: ProductId match AND Stock >= quantity
- Update: $inc operator

This ensures:
- No race conditions
- No negative inventory
- Safe concurrent checkouts


# One-Step run (Docker Compose) 

The entire environment, including the API, MoongoDB and RabbitMQ is containerized for immediate execution. 

## Clone the repository

   * git clone https://github.com/varun1553/CommerHub-Microservice.git
   * cd CommerHub-Microservice

## Launch the stack 
    
    * docker-compose up --build
   
   -> The API will be available at http://localhost:5115
   -> MongoDB is exposed on 27017
   -> RabbitMQ Management UI is available at http://localhost/15672 (Guest/Guest)


# Testing 

Tests project includes nUnit test cases:
-> Validation Logic: Preventing Orders with negative quantities
-> Stock Integrity: Verifying correct decrement logic during high-concurrency checkouts
-> Event Emmission: Ensuring OrderCreated events are successfully published to RabbitMQ

To run tests use command: 
      * dotnet test
