# 📚 User Management API (Backend .NET + MySQL)

API REST para gestión de **Usuarios** y **Estudiantes**, desarrollada con arquitectura **DDD + Clean Architecture**, usando **Entity Framework Core** con base de datos **MySQL** y autenticación mediante **JWT**.

---

## 🚀 Tecnologías utilizadas

| Componente | Tecnología |
|-----------|------------|
| Lenguaje | C# (.NET 9) |
| Backend API | ASP.NET Core Web API |
| Arquitectura | Domain Driven Design (DDD) + Clean Architecture |
| ORM | Entity Framework Core |
| Base de datos | **MySQL** (con migraciones y code-first) |
| Autenticación | JSON Web Tokens (JWT) |
| Documentación API | Swagger / OpenAPI |

---

## 📁 Estructura del Proyecto

```
Api-Cross-test/
│
├─ userManagement.Api/            → Controladores (exposición HTTP)
├─ userManagement.Application/    → Servicios, DTOs y lógica de aplicación
├─ userManagement.Domain/         → Entidades + Interfaces (repositorios)
└─ userManagement.Infrastructure/ → EF Core + Repositorios + DbContext
```

- `Domain`: Modelo de negocio (Entidades `User` y `Student`)
- `Application`: DTOs y Servicios (`AuthService`, `UserService`, `StudentService`)
- `Infrastructure`: Persistencia con **EF Core + MySQL**
- `Api`: Exposición de endpoints via controllers

---

## 🔧 Configuración base de datos (MySQL)

En el archivo `Program.cs` la API se conecta a MySQL mediante `DbContext`:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection")))
);
```

En tu `appsettings.json` debes poner la cadena de conexión:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;database=userManagementDB;user=root;password=TU_PASSWORD;"
  }
}
```

### 🏗 Migraciones EF Core

```sh
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## 🔐 Autenticación (JWT)

Flujo:
1. El usuario se registra o inicia sesión
2. La API valida credenciales
3. Se genera token JWT
4. Para consumir endpoints protegidos, el FRONT debe enviar el token:

```
Authorization: Bearer <token>
```

---

## 🧠 Entidades principales

| Entidad | Campos |
|---------|--------|
| **User** | `Id`, `UserName`, `Email`, `PasswordHash`, `Role` |
| **Student** | `Id`, `Name`, `LastName`, `Email`, `Age` |

---

# 🧪 Endpoints disponibles

## ✅ AUTH CONTROLLER  
`Base URL: /api/Auth`

| Método | Endpoint | Body | Descripción |
|--------|----------|------|-------------|
| `POST` | `/register` | `{ "userName": "", "email": "", "password": "" }` | Registrar un usuario |
| `POST` | `/login` | `{ "email": "", "password": "" }` | Devuelve un **JWT** |

📌 Respuesta del login:
```json
{
  "token": "<jwt-token>",
  "email": "user@mail.com",
  "username": "user"
}
```

---

## 👤 USER CONTROLLER  
`Base URL: /api/User`  
🔒 Requiere token JWT

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `GET` | `/list` | Lista todos los usuarios |
| `GET` | `/get/{id}` | Obtiene usuario por id |

---

## 🎓 STUDENT CONTROLLER  
`Base URL: /api/Student`  
🔒 Requiere token JWT

| Método | Endpoint | Body | Descripción |
|--------|----------|------|-------------|
| `POST` | `/create` | `{ "name": "", "lastName": "", "email": "", "age": 0 }` | Crear estudiante |
| `GET` | `/list` | — | Lista estudiantes |
| `GET` | `/get/{id}` | — | Obtener estudiante |
| `PUT` | `/update/{id}` | (mismo body que create) | Actualizar estudiante |
| `DELETE` | `/delete/{id}` | — | Eliminar estudiante |

---




## 🏃‍♂️ Como ejecutar localmente

```sh
git clone https://github.com/illuminaki/Api-Cross-test.git
cd Api-Cross-test
dotnet restore
dotnet run --project userManagement.Api
```

Swagger estará disponible en:

```
https://localhost:5096/swagger
```

---

## ✨ Características destacadas

✔ Uso de DDD y Clean Architecture  
✔ JWT + Roles para autorización  
✔ MySQL + EF Core + Migraciones  
✔ Endpoints listos para frontend (CORS habilitado)
