\# Architecture Guardrails



Follow these architecture principles strictly:



• Use Clean Architecture.

• Business logic must not be placed in controllers.

• External dependencies must depend on abstractions.

• Infrastructure concerns must not leak into the domain layer.



If existing code violates these principles, highlight the violation and propose a compliant refactoring.

