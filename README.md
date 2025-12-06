# 🚀 API BACKEND1 – Notas Encriptadas + Autenticación JWT

Proyecto desarrollado por: **Martin Cossio, Bladimir Mejía, Jesús Bibiano, Aaron Téllez**

API REST profesional que permite registrar usuarios, iniciar sesión y crear notas personales encriptadas, utilizando:

- 🔐 **JWT (JSON Web Tokens)**
- 🔏 **AES-256**
- 🗄 **SQLite + Entity Framework Core**
- 📘 **Swagger como documentación interactiva**
- 🧱 Arquitectura limpia: controllers, services, DTOs, models

---

# ⭐ 1. ¿Qué hace esta API?

Esta API permite:

- ✔ Registrar usuarios
- ✔ Iniciar sesión y recibir un JWT
- ✔ Crear notas — el contenido se encripta antes de guardarse
- ✔ Obtener notas — se desencriptan automáticamente
- ✔ Actualizar / Eliminar notas
- ✔ Aislar datos por usuario (solo ves tus notas)

Perfecto para:

- Demostrar seguridad backend
- Criptografía real
- Arquitectura de APIs modernas

---

# 🔐 2. Autenticación JWT

### ¿Cómo funciona?

1️⃣ El usuario se registra → `/api/Auth/register`  
2️⃣ Inicia sesión → `/api/Auth/login`  
3️⃣ La API genera un **token JWT**  
4️⃣ Las rutas protegidas requieren enviar:

Authorization: Bearer <token>

yaml
Copiar código

Con esto solo usuarios autenticados pueden consultar o crear notas.

---

# 🔏 3. Encriptación AES-256 del contenido de las notas

La API usa **AES-256** con:

- Clave de 32 bytes
- IV de 16 bytes
- Resultado en Base64

### 🔁 Flujo de encriptación:

1. El usuario manda un texto plano
2. `AesEncryptionService` lo encripta
3. SQLite guarda el texto cifrado
4. Al consultarlo, el backend lo **desencripta automáticamente**

---

### Ejemplo

**Entrada del usuario (request JSON):**

```json
{
  "title": "Mi primera nota",
  "content": "Esta es información secreta."
}
Contenido guardado en la base de datos (encriptado):

Copiar código
3Aa91xmZ8TqRNVvGk+8O1A5j2Q9n1rPV...
Respuesta devuelta al usuario (desencriptada):

json
Copiar código
{
  "id": 1,
  "title": "Mi primera nota",
  "content": "Esta es información secreta."
}
📘 4. ¿Por qué preferimos Swagger sobre Postman?
Característica	Swagger	Postman
Se genera desde la API automáticamente	✔	✖
Probar endpoints sin configuración	✔	✖
Documentación interactiva	✔	✖
Autorización JWT con un clic	✔	✔
Ver DTOs y modelos	✔	✔

Beneficios reales:
Ver JSON de ejemplo automáticamente

Probar cada endpoint con un botón

Agregar JWT sin escribir headers

Ideal para exponer la API en presentación

🗄 5. Base de datos: Migración de LocalDB a SQLite
❌ ¿Por qué NO usamos LocalDB?
No funciona en Render

Requiere SQL Server instalado

No es portable

✅ ¿Por qué SÍ usamos SQLite?
Un solo archivo .db

Perfecto para demos y proyectos pequeños

Compatible con EF Core

Funciona en Render

🔧 Cadena de conexión:
json
Copiar código
"ConnectionStrings": {
  "DefaultConnection": "Data Source=notes.db"
}
🛠 Crear la base de datos:
sql
Copiar código
dotnet ef migrations add Initial
dotnet ef database update
📂 6. Arquitectura del proyecto
pgsql
Copiar código
API BACKEND1
│
├── Controllers
│   ├── AuthController.cs
│   ├── NotesController.cs
│
├── Data
│   ├── AppDbContext.cs
│
├── DTOs
│   ├── LoginDto.cs
│   ├── RegisterDto.cs
│   ├── NoteCreateDto.cs
│   ├── NoteUpdateDto.cs
│   ├── NoteResponseDto.cs
│
├── Models
│   ├── User.cs
│   ├── Note.cs
│
├── Services
│   ├── AesEncryptionService.cs
│   ├── IEncryptionService.cs
│
└── notes.db  (SQLite Database)
📊 7. Diagrama UML (Mermaid)
mermaid
Copiar código
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
🚀 8. Ejecutar el proyecto localmente
pgsql
Copiar código
dotnet restore
dotnet ef database update
dotnet run
Abrir Swagger:
👉 http://localhost:5063/swagger

🔥 9. Endpoints principales
🧑‍💻 Autenticación
POST /api/Auth/register
Crear usuario.

POST /api/Auth/login
Obtener JWT.

📓 Notas (requiere JWT)
POST /api/Notes
Crear nota encriptada.

GET /api/Notes
Listar notas del usuario.

GET /api/Notes/{id}
Obtener nota desencriptada.

PUT /api/Notes/{id}
Modificar nota.

DELETE /api/Notes/{id}
Eliminar nota.

🌐 10. Desplegar en Render
1️⃣ Subir repo a GitHub
2️⃣ Render → New Web Service
3️⃣ Seleccionar el repositorio
4️⃣ Build Command:

nginx
Copiar código
dotnet restore && dotnet build
5️⃣ Start Command:

nginx
Copiar código
dotnet API BACKEND1.dll
6️⃣ Variables de entorno:

ini
Copiar código
ASPNETCORE_ENVIRONMENT = Production
7️⃣ Deploy 🚀

Render levantará Swagger automáticamente.
```
