# 🚀 API BACKEND1 – Notas Encriptadas + Autenticación JWT

Proyecto desarrollado por: **Martin Cossio, Bladimir Mejía, Jesús Bibiano, Aaron Téllez**

API REST que permite registrar usuarios, iniciar sesión y crear notas personales encriptadas, utilizando:

- 🔐 JWT (JSON Web Tokens)
- 🔏 AES-256
- 🗄️ SQLite + Entity Framework Core
- 📘 Swagger como documentación interactiva
- 🧱 Arquitectura limpia (controllers, services, DTOs, models)

---

## ⭐ 1. ¿Qué hace esta API?

Esta API permite:

- Registrar usuarios
- Iniciar sesión y recibir un token JWT
- Crear notas (el contenido se encripta antes de guardarse)
- Obtener notas (la API las desencripta antes de devolverlas)
- Actualizar y eliminar notas
- Aislar las notas por usuario (solo ves tus notas)

Es ideal para:

- Mostrar seguridad backend
- Demostrar criptografía simétrica (AES)
- Proyectos escolares / demo profesionales

---

## 🔐 2. Autenticación JWT

Flujo básico:

1. El usuario se registra en `/api/Auth/register`
2. Inicia sesión en `/api/Auth/login`
3. La API devuelve un **JWT**
4. Para acceder a rutas protegidas se envía:

   Authorization: Bearer <tu_token>

Solo usuarios autenticados pueden consumir los endpoints de notas.

---

## 🔏 3. Encriptación AES-256 de las notas

Se usa un servicio `AesEncryptionService` que:

- Toma el texto plano (`content`)
- Lo encripta con **AES-256** usando una clave de 32 bytes y un IV de 16 bytes
- Guarda el resultado en la base de datos como texto encriptado (Base64)
- Al leer la nota, desencripta y devuelve el contenido original

### Ejemplo

Entrada del usuario (request JSON):

    {
      "title": "Mi primera nota",
      "content": "Esta es información secreta."
    }

Contenido guardado en la BD (encriptado, ejemplo):

    3Aa91xmZ8TqRNVvGk+8O1A5j2Q9n1rPV...

Respuesta que ve el usuario:

    {
      "id": 1,
      "title": "Mi primera nota",
      "content": "Esta es información secreta."
    }

---

## 📘 4. ¿Por qué usamos Swagger en lugar de Postman?

Aunque Postman es muy bueno, para este proyecto Swagger nos dio ventajas:

- Se genera automáticamente a partir del código de la API
- Permite probar endpoints sin instalar nada extra
- Muestra los modelos (DTOs) y esquemas de forma clara
- Tiene botón “Authorize” para meter el JWT una sola vez
- Es perfecto para exponer el proyecto en clase (todo visual)

En resumen: solo con levantar la API ya tienes documentación viva y un “Postman” integrado.

---

## 🗄️ 5. Base de datos: de LocalDB a SQLite

### Por qué NO usamos LocalDB

- No funciona bien en plataformas como Render
- Requiere SQL Server instalado
- No es portable

### Por qué SÍ usamos SQLite

- Es un solo archivo `notes.db`
- Ideal para demos y proyectos pequeños
- Es compatible con Entity Framework Core
- Funciona en Render sin configuración adicional

Cadena de conexión en `appsettings.json`:

    "ConnectionStrings": {
      "DefaultConnection": "Data Source=notes.db"
    }

Para crear la base de datos y aplicar la migración inicial:

    dotnet ef migrations add Initial
    dotnet ef database update

Esto genera automáticamente `notes.db`.

---

## 📂 6. Arquitectura del proyecto

Estructura general:

    API BACKEND1
    │
    ├── Controllers
    │   ├── AuthController.cs
    │   └── NotesController.cs
    │
    ├── Data
    │   └── AppDbContext.cs
    │
    ├── DTOs
    │   ├── LoginDto.cs
    │   ├── RegisterDto.cs
    │   ├── NoteCreateDto.cs
    │   ├── NoteUpdateDto.cs
    │   └── NoteResponseDto.cs
    │
    ├── Models
    │   ├── User.cs
    │   └── Note.cs
    │
    ├── Services
    │   ├── AesEncryptionService.cs
    │   └── IEncryptionService.cs
    │
    └── notes.db  (SQLite)

---

## 🚀 7. Ejecutar el proyecto localmente

1. Restaurar dependencias:

   dotnet restore

2. Crear / actualizar la base de datos:

   dotnet ef database update

3. Ejecutar la API:

   dotnet run

4. Abrir Swagger en el navegador:

   http://localhost:5063/swagger

---

## 🔥 8. Endpoints principales

### Autenticación

- **POST** `/api/Auth/register`
  Crea un usuario nuevo.

- **POST** `/api/Auth/login`
  Devuelve un JWT que se usará en los demás endpoints protegidos.

### Notas (requiere JWT en el header)

- **POST** `/api/Notes`
  Crea una nota encriptada.

- **GET** `/api/Notes`
  Lista las notas del usuario autenticado.

- **GET** `/api/Notes/{id}`
  Devuelve una nota desencriptada por id.

- **PUT** `/api/Notes/{id}`
  Actualiza una nota existente.

- **DELETE** `/api/Notes/{id}`
  Elimina una nota.

---

## 🌐 9. Despliegue en Render (resumen)

1. Subir el proyecto a GitHub.
2. En Render: “New Web Service” y conectar el repo.
3. Build command:

   dotnet restore && dotnet build

4. Start command:

   dotnet API BACKEND1.dll

5. Variables de entorno:

   ASPNETCORE_ENVIRONMENT = Production

Render levantará la API y podrás usar Swagger en la URL pública de tu servicio.

---

## 🧩 Diagrama UML

```mermaid
classDiagram
    class User {
        int Id
        string Username
        string PasswordHash
        List~Note~ Notes
    }

    class Note {
        int Id
        string Title
        string EncryptedContent
        int UserId
        User User
    }

    class AesEncryptionService {
        +Encrypt(text) string
        +Decrypt(cipher) string
    }

    class AuthController {
        +Register()
        +Login()
    }

    class NotesController {
        +Create()
        +GetAll()
        +GetById()
        +Update()
        +Delete()
    }

    User "1" --> "many" Note
    NotesController --> AesEncryptionService
    AuthController --> User

Relaciones principales:

- Un `User` tiene muchas `Note`
- `NotesController` usa `AesEncryptionService`
- `AuthController` trabaja con `User`

```
