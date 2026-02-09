# UML_Clases — Diagrama de Clases (UML)

> **Nota:** este UML representa **solo el dominio (entidades)**.  
> No incluye controladores ni DTOs, porque la consigna pide clases del dominio.

```mermaid
classDiagram
    class User {
        +int Id
        +string Username
        +string PasswordHash
        +DateTime CreatedAt
        +DateTime UpdatedAt
        +List~Note~ Notes
        +List~AuditLog~ AuditLogs
    }

    class Note {
        +int Id
        +string Title
        +string EncryptedContent
        +int UserId
        +DateTime CreatedAt
        +DateTime UpdatedAt
    }

    class SharedNote {
        +int Id
        +int NoteId
        +int SharedByUserId
        +int SharedWithUserId
        +int PermissionId
        +DateTime SharedAt
        +DateTime CreatedAt
        +DateTime UpdatedAt
    }

    class Permission {
        +int Id
        +string Code
        +string Description
    }

    class AuditLog {
        +int Id
        +int UserId
        +string Action
        +int? NoteId
        +DateTime CreatedAt
    }

    %% Multiplicidades
    User "1" --> "0..*" Note : owns
    Note "1" --> "0..*" SharedNote : shared via
    User "1" --> "0..*" SharedNote : shares (by)
    User "1" --> "0..*" SharedNote : receives (with)
    Permission "1" --> "0..*" SharedNote : grants
    User "1" --> "0..*" AuditLog : logs
    Note "0..1" --> "0..*" AuditLog : referenced
```

## Lectura rápida

- **User → Note (1–N)**: notas propias.
- **User ↔ Note (N–N)**: compartición mediante **SharedNote**.
- **Permission** define el nivel de acceso de una compartición.
- **AuditLog** registra acciones del usuario (y opcionalmente liga una nota).
