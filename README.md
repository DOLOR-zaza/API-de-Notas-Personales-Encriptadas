🚀 API BACKEND1 – Notas Encriptadas + Autenticación JWT
Proyecto desarrollado por: Martin Cossio, Bladimir Mejia, Jesús Bibiano, Aaron Téllez

API REST profesional que permite registrar usuarios, iniciar sesión y crear notas personales encriptadas, utilizando:

🔐 JWT (JSON Web Tokens)

🔏 AES-256

🗄 SQLite + Entity Framework Core

📘 Swagger como documentación interactiva

🧱 Arquitectura limpia: controllers, services, DTOs, models

⭐ 1. ¿Qué hace esta API?

Esta API permite:

✔ Registrar usuarios
✔ Iniciar sesión y recibir un JWT
✔ Crear notas — el contenido se encripta antes de guardarse
✔ Obtener notas — se desencriptan automáticamente
✔ Actualizar / Eliminar notas
✔ Aislar datos por usuario (solo ves tus notas)

Perfecto para:

Demostrar seguridad backend

Criptografía real

Arquitectura de APIs modernas

Deploy profesional en Render

🔐 2. Autenticación JWT (JSON Web Tokens)
Flujo:

Usuario se registra → /api/Auth/register

Usuario inicia sesión → /api/Auth/login

API genera un JWT firmado

Usuario lo envía en cada petición protegida:

Authorization: Bearer <tu_token>

Con eso, solo usuarios autenticados pueden acceder a /api/Notes.

🔏 3. Encriptación AES-256 del contenido de las notas

Usamos:

Clave de 32 bytes → AES-256

IV de 16 bytes

AesEncryptionService.cs para encriptar/desencriptar

Ejemplo completo
📝 Request (lo que envía el usuario)
{
"title": "Mi primera nota",
"content": "Esta es información secreta."
}

🔐 Guardado en la base de datos (encriptado)
3Aa91xmZ8TqRNVvGk+8O1A5j2Q9n1rPV...

🔓 Respuesta desencriptada devuelta al usuario
{
"id": 1,
"title": "Mi primera nota",
"content": "Esta es información secreta."
}

📘 4. ¿Por qué preferimos Swagger sobre Postman?

| Característica                     | Swagger | Postman |
| ---------------------------------- | :-----: | :-----: |
| Se genera automáticamente          |    ✔    |    ✖    |
| Probar endpoints sin configuración |    ✔    |    ✖    |
| Documentación integrada            |    ✔    |    ✖    |
| Autorización JWT con un clic       |    ✔    |    ✔    |
| Ver DTOs y modelos directamente    |    ✔    |    ✔    |
| Perfecto para exposición en clase  |    ✔    |    ✖    |

Beneficios reales en nuestra presentación:

Ver JSONs automáticamente

Probar cualquier método con 1 clic

Insertar el JWT con un botón (“Authorize”)

Mostrar arquitectura y endpoints visualmente

🗄 5. Migración de Base de Datos: LocalDB → SQLite
❌ Por qué NO usamos LocalDB

No funciona en Render

Requiere SQL Server instalado

No es portable

✔ Por qué SÍ usamos SQLite

Un solo archivo .db

Súper liviano

Soporta EF Core

Funciona perfecto en Render

Ideal para demos y proyectos pequeños

🔧 Cadena de conexión final
"ConnectionStrings": {
"DefaultConnection": "Data Source=notes.db"
}

🛠 Crear base de datos
dotnet ef migrations add Initial
dotnet ef database update

Esto genera automáticamente notes.db.

📂 6. Arquitectura del Proyecto
API BACKEND1
│
├── Controllers
│ ├── AuthController.cs
│ ├── NotesController.cs
│
├── Data
│ ├── AppDbContext.cs
│
├── DTOs
│ ├── LoginDto.cs
│ ├── RegisterDto.cs
│ ├── NoteCreateDto.cs
│ ├── NoteUpdateDto.cs
│ ├── NoteResponseDto.cs
│
├── Models
│ ├── User.cs
│ ├── Note.cs
│
├── Services
│ ├── AesEncryptionService.cs
│ ├── IEncryptionService.cs
│
└── notes.db (SQLite Database)

📊 7. Diagrama UML (Mermaid)

Este diagrama sí funciona en GitHub.

classDiagram
class User {
int Id
string Username
string PasswordHash
List<Note> Notes
}

    class Note {
        int Id
        string Title
        string EncryptedContent
        int UserId
        User User
    }

    class AesEncryptionService {
        +Encrypt(string text) string
        +Decrypt(string cipher) string
    }

    class AuthController {
        +Register(RegisterDto)
        +Login(LoginDto)
    }

    class NotesController {
        +Create(NoteCreateDto)
        +GetAll()
        +GetById(int)
        +Update(int, NoteUpdateDto)
        +Delete(int)
    }

    User "1" --> "many" Note
    NotesController --> AesEncryptionService
    AuthController --> User

🚀 8. Cómo ejecutar el proyecto localmente
1️⃣ Restaurar dependencias
dotnet restore

2️⃣ Crear base de datos
dotnet ef database update

3️⃣ Ejecutar API
dotnet run

4️⃣ Abrir Swagger
http://localhost:5063/swagger

🔥 9. Endpoints Principales
🧑‍💻 Autenticación
POST /api/Auth/register

Crear usuario nuevo.

POST /api/Auth/login

Devuelve un JWT.

📝 Notas (requiere JWT)
POST /api/Notes

Crear nota (encriptada).

GET /api/Notes

Listar notas del usuario actual.

GET /api/Notes/{id}

Obtener nota desencriptada.

PUT /api/Notes/{id}

Modificar nota.

DELETE /api/Notes/{id}

Eliminar nota.

🌐 10. Cómo desplegar en Render
1️⃣ Subir proyecto a GitHub
2️⃣ En Render → “New Web Service”
3️⃣ Seleccionar tu repo
4️⃣ Build Command:
dotnet restore && dotnet build

5️⃣ Start Command:
dotnet API BACKEND1.dll

6️⃣ Variables de entorno:
ASPNETCORE_ENVIRONMENT = Production

7️⃣ Deploy 🚀

Render levantará Swagger automáticamente.
