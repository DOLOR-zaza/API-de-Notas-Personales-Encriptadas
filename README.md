🚀 API BACKEND1 – Notas Encriptadas + Autenticación JWT

Proyecto desarrollado por: Martin Cossio, Bladimir Mejía, Jesús Bibiano, Aaron Téllez

API REST profesional que permite registrar usuarios, iniciar sesión y crear notas personales encriptadas utilizando:

🔐 JWT (JSON Web Tokens)

🔏 AES-256

🗄️ SQLite + Entity Framework Core

📘 Swagger como documentación interactiva

🧱 Arquitectura limpia: controllers, services, DTOs, models

⭐ 1. ¿Qué hace esta API?

La API permite:

✔ Registrar usuarios

✔ Iniciar sesión y obtener un JWT

✔ Crear notas (el contenido se encripta antes de guardarse)

✔ Consultar notas (se desencriptan automáticamente)

✔ Actualizar / Eliminar notas

✔ Aislar notas por usuario (solo ves tus notas)

Perfecto para:

Seguridad backend

Demostrar criptografía real

Proyectos escolares y profesionales

🔐 2. Autenticación JWT
¿Cómo funciona?

1️⃣ El usuario se registra → /api/Auth/register
2️⃣ Inicia sesión → /api/Auth/login
3️⃣ La API genera un token JWT
4️⃣ Las rutas protegidas requieren enviar:

Authorization: Bearer <token>

Esto garantiza que solo usuarios autorizados pueden ver o crear notas.

🔏 3. Encriptación AES-256 del contenido de las notas
Flujo:

El usuario envía texto plano

El servicio AesEncryptionService aplica AES-256

Se almacena contenido cifrado en SQLite

Al leer la nota, la API la desencripta automáticamente

Ejemplo

Entrada del usuario (request JSON):

{
"title": "Mi primera nota",
"content": "Esta es información secreta."
}

Contenido en la base de datos (encriptado):

3Aa91xmZ8TqRNVvGk+8O1A5j2Q9n1rPV...

Respuesta devuelta al usuario (desencriptada):

{
"id": 1,
"title": "Mi primera nota",
"content": "Esta es información secreta."
}

📘 4. ¿Por qué Swagger y no Postman?
Característica Swagger Postman
Se genera desde la API automáticamente ✔ ✖
Probar endpoints sin configuración extra ✔ ✖
Documentación integrada ✔ ✖
Autorizar JWT con 1 clic ✔ ✔
Ver DTOs y modelos ✔ ✔
Beneficios reales:

Ver JSON de ejemplo automáticamente

Probar cada endpoint con un botón

Agregar JWT sin escribir headers

Ideal para presentar en clase

🗄️ 5. Base de datos: Migración de LocalDB a SQLite
❌ ¿Por qué NO LocalDB?

No funciona en Render

Requiere SQL Server instalado

No es portable

✅ ¿Por qué SÍ SQLite?

Un solo archivo .db

Perfecto para demos

Funciona en Render sin configuración

Totalmente compatible con EF Core

Cadena de conexión:
"ConnectionStrings": {
"DefaultConnection": "Data Source=notes.db"
}

Crear la base de datos:
dotnet ef migrations add Initial
dotnet ef database update

📂 6. Arquitectura del proyecto
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
└── notes.db (SQLite)

📊 7. Diagrama UML (Mermaid)
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
+Encrypt(text)
+Decrypt(cipher)
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

🚀 8. Ejecutar el proyecto localmente
dotnet restore
dotnet ef database update
dotnet run

Abrir Swagger:

👉 http://localhost:5063/swagger

🔥 9. Endpoints principales
🧑‍💻 Autenticación

POST /api/Auth/register

POST /api/Auth/login → devuelve JWT

📝 Notas (requiere JWT)

POST /api/Notes

GET /api/Notes

GET /api/Notes/{id}

PUT /api/Notes/{id}

DELETE /api/Notes/{id}

🌐 10. Despliegue en Render
Build Command
dotnet restore && dotnet build

Start Command
dotnet API BACKEND1.dll

Variables de entorno
ASPNETCORE_ENVIRONMENT = Production

Render levantará Swagger automáticamente.
