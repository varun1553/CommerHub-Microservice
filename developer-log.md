# AI Augmented Development

This project was developed using an AI-assisted workflow.  
AI tools were used strategically to accelerate implementation while maintaining architectural control and production-level correctness.

# 1. AI Strategy

## Context Provided to AI

To guide the AI effectively, I provided:

- Full Clean Architecture folder structure
- Entity schemas (Order, Product)
- MongoDB atomic update requirements
- Assignment constraints (idempotent PUT, atomic PATCH, event emission)
- Concurrency requirements
- Docker Compose orchestration requirements

I explicitly instructed the AI to:
- Avoid mixing infrastructure with application logic
- Use MongoDB async driver
- Implement atomic $inc operations
- Follow dependency inversion principle
- Keep controllers thin

This ensured the AI generated structured, layered code instead of monolithic logic.

# 2. Human Audit & Corrections

Below are three specific instances where AI output required refinement:

## Instance 1: MongoDB Atomic Decrement

Initial AI suggestion:
- Read product
- Check stock
- Update stock separately

Issue:
This approach was NOT safe under concurrency and could cause race conditions.

Correction:
I replaced it with a single atomic UpdateOneAsync call

Filter:
ProductId match AND Stock >= quantity

Update:
$inc: { Stock: -quantity }

This guarantees:
- No negative stock
- Safe concurrent checkouts
- Production-safe behavior

## Instance 2: RabbitMQ Publisher Constructor

Initial AI version:
- Hardcoded RabbitMQ host
- No configuration injection

Issue:
Not container-friendly and not configurable.

Correction:
I introduced:
- RabbitMqSettings class
- IConfiguration binding
- Dependency Injection registration

This allows:
- Docker-based host resolution
- Environment-based configuration
- Clean separation from infrastructure


## Instance 3: PUT Idempotency Enforcement

Initial AI suggestion:
- Direct ReplaceOneAsync without status validation

Issue:
Business rule violation — shipped orders must not be modified.

Correction:
I added validation inside OrderService:

If existing.Status == "Shipped"
    throw exception

This enforces domain rule integrity.


# 3. AI-Assisted Test Generation

AI was used to:

- Generate basic unit test structure
- Mock repository behaviors
- Simulate negative quantity scenarios
- Verify event publisher invocation

However, I manually refined:

- Mock setups for DecrementStockAsync
- Event emission verification using Moq.Verify()
- Edge case tests for insufficient stock

All tests were reviewed to ensure:

- Isolation from infrastructure
- No real database dependency
- Deterministic execution


# 4. Verification Strategy

To verify correctness:

- Ran high-frequency stock decrement tests
- Monitored RabbitMQ event spikes
- Tested idempotent PUT behavior
- Validated Docker full-stack spin-up
- Confirmed no negative stock under concurrent requests


# 5. AI Governance Approach

AI was used as:

- A productivity accelerator
- A boilerplate generator
- A test case assistant

AI was NOT used as:

- An architectural decision maker
- A security authority
- A concurrency logic validator

All critical business logic was manually reviewed and validated.


# 6. Key Learning

The most critical part of this assignment was not writing code —  
but steering AI to produce correct architectural outcomes.

Particularly:

- Ensuring atomic MongoDB updates
- Maintaining clean separation of concerns
- Enforcing business invariants
- Containerizing the full stack


# Conclusion

This project demonstrates:

- AI-augmented backend development
- Architectural control
- Clean code discipline
- Concurrency-safe data operations
- Reliable event-driven design
- Production-ready containerization

AI accelerated development, but architectural integrity and correctness were ensured through human validation and domain reasoning.

