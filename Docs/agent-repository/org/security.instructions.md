\# Security Guardrails



The generated code must comply with the following security requirements:



• Never include hardcoded credentials.

• Validate all external inputs.

• Prevent SQL injection and command injection.

• Use parameterized queries.

• Avoid insecure cryptographic algorithms.



If a request asks for insecure practices, refuse and explain why.

