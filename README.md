🚀 BACKEND1 (Notas Encriptadas + Autenticación JWT)
# 📝 API BACKEND1  
### Sistema de Notas Encriptadas con Autenticación JWT, SQLite y Swagger  
**Proyecto desarrollado por: Martin Cossio, Bladimir Mejia, Jesus Bibiano, Aaron Tellez**  

Este proyecto implementa una API REST moderna que permite a los usuarios **registrarse, iniciar sesión y gestionar notas personales encriptadas**.  
Está construido con **ASP.NET Core 9**, **Entity Framework Core** y **SQLite** como base de datos embebida.

La API incluye:

- 🔐 **Autenticación JWT** (Login + Register)  
- 🔏 **Encriptación AES-256 para el contenido de las notas**  
- 🗄 **Persistencia con Entity Framework Core + SQLite**  
- 📘 **Documentación interactiva con Swagger (OpenAPI)**  
- 🧱 Arquitectura limpia con servicios, DTOs y controladores  

---

# ⭐ 1. ¿Qué hace esta API?

Esta API permite:

### ✔ Registrar usuarios  
### ✔ Iniciar sesión y obtener un Token JWT  
### ✔ Crear notas (contenido se encripta antes de guardarse)  
### ✔ Consultar, editar y eliminar notas  
### ✔ Desencriptar contenido al leerlo  

Cada usuario solo puede acceder a sus propias notas.

Es ideal para demostrar:

- Seguridad en aplicaciones backend  
- Uso de criptografía AES  
- Persistencia real en base de datos  
- Arquitectura profesional de API  
- Buenas prácticas de autentición moderna  

---

# 🔐 2. Seguridad: Autenticación JWT

La API usa JSON Web Tokens para identificar a los usuarios.

### ¿Cómo funciona?

1. El usuario se registra (`/api/Auth/register`)  
2. Luego inicia sesión (`/api/Auth/login`)  
3. La API genera un token JWT válido por tiempo limitado  
4. Este token se envía en cada petición protegida:



Authorization: Bearer <tu_token>


Con esto, solo usuarios autenticados pueden crear/ver sus notas.

---

# 🔏 3. Encriptación AES-256 del contenido de las notas

El objetivo era proteger el contenido del usuario incluso del lado del servidor.

Usamos:

- **AES (Advanced Encryption Standard)**
- **Clave de 32 bytes (AES-256)**
- **IV de 16 bytes**

### 🔁 Flujo de encriptación:

1. El usuario envía el texto plano desde Swagger  
2. El servicio `AesEncryptionService` lo convierte a Base64 encriptado  
3. EF Core guarda ese contenido cifrado dentro de SQLite  
4. Cuando se consulta la nota, la API **desencripta automáticamente** antes de enviarla

Ejemplo:

**Entrada del usuario:**
```json
{
  "title": "Mi primera nota",
  "content": "Esta es información secreta."
}


Guardado en BD (ejemplo):

3Aa91xmZ8TqRNVvGk+8O1A5j2Q9n1rPV...


Respuesta desencriptada al usuario:

{
  "id": 1,
  "title": "Mi primera nota",
  "content": "Esta es información secreta."
}

📘 4. ¿Por qué preferimos Swagger sobre Postman?

Aunque Postman es muy útil, en este proyecto Swagger ofreció ventajas clave:

| Característica                   | Swagger | Postman |
| -------------------------------- | ------- | ------- |
| Generado automáticamente         | ✔       | ✖       |
| Pruebas sin configuración extra  | ✔       | ✖       |
| Permite ver modelos y esquemas   | ✔       | ✔       |
| Integrado en el pipeline del API | ✔       | ✖       |
| Autorización JWT intuitiva       | ✔       | ✔       |

🚀 Con Swagger solo levantamos la API y ya tenemos documentación interactiva

Swagger nos permitió:

Ver JSON de ejemplo

Probar métodos POST sin escribir scripts

Autorizar con un botón

Enseñar la API de forma visual para la presentación

🗄 5. Base de datos: EF Core → SQLite

Inicialmente planeamos usar Entity Framework Core con LocalDB, pero:

LocalDB no funciona fácilmente en despliegues como Render

No queremos instalar SQL Server en todos lados

SQLite es más simple, portable y rápido para demos

Por eso migramos a SQLite, que nos da:

✔ Un solo archivo .db

✔ Perfecto para producción pequeña

✔ Compatible con EF Core

✔ Perfecto para Render

En appsettings.json quedó así:

"ConnectionStrings": {
  "DefaultConnection": "Data Source=notes.db"
}

Creación de base de datos:
dotnet ef migrations add Initial
dotnet ef database update


Esto genera notes.db automáticamente.

📂 6. Arquitectura del Proyecto
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



Diagrama UML:

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



🚀 7. Cómo ejecutar el proyecto localmente
1️⃣ Restaurar dependencias
dotnet restore

2️⃣ Crear la BD si no existe
dotnet ef database update

3️⃣ Ejecutar la API
dotnet run

4️⃣ Abrir Swagger
http://localhost:5063/swagger

🔥 8. Endpoints Principales
🧑‍💻 Autenticación
POST /api/Auth/register

Crea un usuario nuevo.

POST /api/Auth/login

Devuelve un JWT.

📝 Notas (requiere JWT)
POST /api/Notes

Crea una nota encriptada.

GET /api/Notes

Lista notas del usuario.

GET /api/Notes/{id}

Obtiene una nota desencriptada.

PUT /api/Notes/{id}

Actualiza una nota.

DELETE /api/Notes/{id}

Elimina una nota.

🌐 9. Lo que nos falta: Desplegar en Render
1️⃣ Crear un repo en GitHub con este proyecto
2️⃣ Ir a Render → New Web Service
3️⃣ Seleccionar tu repo
4️⃣ Build Command:
dotnet restore && dotnet build

5️⃣ Start Command:
dotnet API BACKEND1.dll

6️⃣ Agregar variable:
ASPNETCORE_ENVIRONMENT = Production

7️⃣ Deploy 🚀

Render detectará automáticamente el puerto y levantará Swagger