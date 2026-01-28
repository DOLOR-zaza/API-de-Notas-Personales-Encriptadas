# 📊 Diagrama Entidad–Relación (DER)

```mermaid
erDiagram
    USER {
        int Id PK
        string Username UNIQUE
        string PasswordHash
    }

    NOTE {
        int Id PK
        string Title
        string EncryptedContent
        int UserId FK
    }

    SHARED_NOTE {
        int Id PK
        int NoteId FK
        int SharedByUserId FK
        int SharedWithUserId FK
        bool CanRead
        datetime SharedAt
    }

    USER ||--o{ NOTE : owns
    USER ||--o{ SHARED_NOTE : shares
    NOTE ||--o{ SHARED_NOTE : is_shared
```
