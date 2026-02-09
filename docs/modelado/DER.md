# Diagrama Entidad–Relación (DER)

Este DER describe el **modelo de datos** del proyecto **API BACKEND1** (notas encriptadas con autenticación JWT y compartición controlada).

## 🧩 Entidades 

- **User**: usuario del sistema.
- **Note**: nota privada del usuario (contenido encriptado).
- **SharedNote**: entidad puente para compartir notas (**N–N** entre usuarios y notas).
- **Permission**: catálogo de permisos (p. ej. READ).
- **AuditLog**: bitácora/auditoría de acciones (registro, login, creación/compartición de notas, etc.).

---


```mermaid
erDiagram
    USER {
        int Id PK
        string Username "UNIQUE, NOT NULL"
        string PasswordHash "NOT NULL"
        datetime CreatedAt "NOT NULL"
        datetime UpdatedAt "NOT NULL"
    }

    NOTE {
        int Id PK
        string Title "NOT NULL"
        string EncryptedContent "NOT NULL"
        int UserId FK
        datetime CreatedAt "NOT NULL"
        datetime UpdatedAt "NOT NULL"
    }

    PERMISSION {
        int Id PK
        string Code "UNIQUE, NOT NULL (ej: READ)"
        string Description "NOT NULL"
    }

    SHARED_NOTE {
        int Id PK
        int NoteId FK
        int SharedByUserId FK
        int SharedWithUserId FK
        int PermissionId FK
        datetime SharedAt "NOT NULL"
        datetime CreatedAt "NOT NULL"
        datetime UpdatedAt "NOT NULL"
    }

    AUDIT_LOG {
        int Id PK
        int UserId FK
        string Action "NOT NULL (ej: LOGIN, CREATE_NOTE)"
        int NoteId FK "NULLABLE"
        datetime CreatedAt "NOT NULL"
    }

    %% Relaciones
    USER ||--o{ NOTE : "tiene"
    NOTE ||--o{ SHARED_NOTE : "se comparte via"
    USER ||--o{ SHARED_NOTE : "comparte (SharedByUserId)"
    USER ||--o{ SHARED_NOTE : "recibe (SharedWithUserId)"
    PERMISSION ||--o{ SHARED_NOTE : "define permiso"
    USER ||--o{ AUDIT_LOG : "genera"
    NOTE ||--o{ AUDIT_LOG : "opcional"
```

---

## ✅ Restricciones / reglas de integridad (ejemplos)

- `User.Username` es **único** (no se permiten usuarios duplicados).
- `Note.EncryptedContent` es **NOT NULL** (la nota nunca se guarda en texto plano).
- `Permission.Code` es **único** (evita duplicar permisos).
- `SharedNote` referencia llaves foráneas válidas (`NoteId`, `SharedByUserId`, `SharedWithUserId`, `PermissionId`).
- Recomendado: índice compuesto para evitar compartir duplicado:
  - `(NoteId, SharedWithUserId)` **UNIQUE** (una nota no se comparte dos veces al mismo usuario).

