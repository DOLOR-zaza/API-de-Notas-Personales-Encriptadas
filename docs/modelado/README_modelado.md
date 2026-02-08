# 📘 Modelado del Dominio – API de Notas Encriptadas

## 🎯 Dominio del sistema

El sistema es una **API de gestión de notas personales seguras**, donde los usuarios pueden:

- Crear notas privadas
- Proteger su contenido mediante encriptación
- Compartir notas con otros usuarios
- Controlar permisos de lectura

El dominio se centra en **usuarios, notas y relaciones de compartición**, priorizando la seguridad y la integridad de los datos.

---

## 🧩 Entidades principales

### User

Representa a un usuario del sistema.

Responsabilidades:

- Autenticarse
- Ser dueño de notas
- Compartir notas con otros usuarios

---

### Note

Entidad principal del negocio.

Responsabilidades:

- Almacenar información sensible
- Garantizar que el contenido se guarde encriptado
- Pertenecer a un solo usuario

---

### SharedNote

Entidad de relación (tabla puente).

Responsabilidades:

- Representar una nota compartida
- Indicar quién comparte la nota
- Indicar quién la recibe
- Definir permisos de lectura

---

## 🔗 Relaciones clave

- **User 1–N Note**
  - Un usuario puede tener muchas notas
- **User N–N Note (vía SharedNote)**
  - Un usuario puede compartir muchas notas
  - Una nota puede compartirse con varios usuarios

---

## 🧠 Decisiones de diseño clave

- Se separó `SharedNote` como entidad independiente para:
  - Registrar timestamps
  - Manejar permisos
  - Mantener normalización
- Se usó **SQLite** por portabilidad
- Se usó **Entity Framework Core** para ORM
- Se evitó texto plano en la base de datos

---

## 📏 Reglas de integridad del modelo

1. El `Username` del usuario es único
2. Una `Note` siempre pertenece a un `User`
3. Una nota compartida no puede eliminar al usuario receptor
4. `EncryptedContent` nunca es nulo
5. Una nota compartida solo puede leerse si `CanRead = true`

---

## 📌 Supuestos (Assumptions)

- El sistema es educativo / demo
- No se permite edición de notas compartidas
- El usuario dueño mantiene control total
- El sistema no maneja roles avanzados (admin)

---

## ✅ Cumplimiento del checklist

- ✔ 5+ entidades (User, Note, SharedNote)
- ✔ 1–N (User → Note)
- ✔ N–N (User ↔ Note vía SharedNote)
- ✔ ORM implementado (EF Core)
- ✔ Migraciones aplicadas
- ✔ Proyecto compila y corre
