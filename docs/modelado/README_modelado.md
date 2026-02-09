# Dominio y decisiones de modelado

Este documento explica el **dominio**, las **entidades**, las **relaciones** y las **reglas** del sistema *API BACKEND1*.

---

## 1) Dominio: ¿qué resuelve el sistema?

El sistema implementa una API REST donde cada usuario puede:

- Registrarse e iniciar sesión (JWT).
- Crear notas personales (contenido **encriptado con AES-256**).
- Compartir notas con otros usuarios **de forma controlada** (solo lectura u otros permisos).
- Consultar “notas compartidas por mí” y “notas compartidas conmigo”.
- Registrar acciones importantes en una bitácora (**auditoría**).

**Problema que resuelve:** evita guardar información sensible en texto plano y agrega control de acceso y trazabilidad para la compartición.

---

## 2) Entidades y propósito (≥ 5)

1. **User**
   - Representa a una cuenta del sistema.
   - Tiene credenciales (password hasheado) y metadata de auditoría (CreatedAt/UpdatedAt).

2. **Note** *(entidad principal del negocio)*
   - Representa una nota propiedad de un usuario.
   - Guarda `EncryptedContent` (cifrado) y timestamps.

3. **SharedNote** *(entidad puente / relación)*
   - Modela la compartición de una nota:
     - quién comparte (`SharedByUserId`)
     - a quién se comparte (`SharedWithUserId`)
     - qué nota (`NoteId`)
     - bajo qué permiso (`PermissionId`)
   - Permite la relación **N–N** entre usuarios y notas.

4. **Permission**
   - Catálogo de permisos para compartir (ej. `READ`).
   - Evita “booleans sueltos” y hace el modelo extensible (ej. READ/WRITE/ADMIN en el futuro).

5. **AuditLog**
   - Bitácora con acciones relevantes (ej. REGISTER, LOGIN, CREATE_NOTE, SHARE_NOTE).
   - Ayuda a rastrear eventos para debugging o auditoría.

---

## 3) Relaciones (cardinalidades)

- **User 1–N Note**
  - Un usuario puede tener muchas notas.
  - Cada nota pertenece a un único usuario.

- **User N–N Note** (vía **SharedNote**)
  - Un usuario puede recibir muchas notas compartidas.
  - Una nota puede compartirse con muchos usuarios.

- **Permission 1–N SharedNote**
  - Un permiso puede aplicarse a muchas comparticiones.

- **User 1–N AuditLog**
  - Un usuario genera muchas entradas de auditoría.

- **Note 0–N AuditLog**
  - Algunas acciones se ligan a una nota (crear/editar/compartir), otras no (login).

---

## 4) Reglas mínimas del modelo (obligatorias) — cómo cumplimos

✅ **5+ entidades:** User, Note, SharedNote, Permission, AuditLog  
✅ **Timestamps:** CreatedAt/UpdatedAt en User, Note, SharedNote; CreatedAt en AuditLog  
✅ **Relación 1–N:** User → Notes; User → AuditLogs; Permission → SharedNotes  
✅ **Relación N–N:** User ↔ Note (a través de SharedNote)  
✅ **Regla de integridad:** Username único; EncryptedContent NOT NULL; Permission.Code único; FKs consistentes

---

## 5) Decisiones clave (por qué así)

- **SharedNote como entidad puente**: hace la compartición explícita y extensible (permisos, auditoría, timestamps).
- **Permission como catálogo**: permite crecer el sistema sin cambiar la tabla de compartición (solo agregas permisos).
- **AuditLog**: agrega trazabilidad y evidencia de acciones (útil para seguridad y debugging).
- **SQLite**: facilita despliegue y demos (un archivo), compatible con EF Core.

---

## 6) Supuestos (assumptions)

- El contenido se guarda **siempre cifrado** en la BD (solo se desencripta al responder).
- El permiso mínimo es **READ**.
- Una nota se puede compartir a múltiples usuarios.
- Se recomienda evitar duplicados: una nota no debería compartirse 2 veces al mismo usuario (constraint/índice único).
- El “dueño” de la nota es el único que puede compartirla.

