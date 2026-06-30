# 🚦 TrafficFlow

![.NET](https://img.shields.io/badge/.NET-10-512BD4?style=for-the-badge&logo=dotnet)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-5C2D91?style=for-the-badge)
![MongoDB](https://img.shields.io/badge/MongoDB-Database-47A248?style=for-the-badge&logo=mongodb)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

---

# 📖 Descripción

**TrafficFlow** es una aplicación web desarrollada con **ASP.NET Core MVC** que permite consultar información relacionada con el estado del tráfico, registrar eventos de emergencia y administrar el historial y los lugares favoritos de los usuarios.

El proyecto integra información de diferentes servicios para ofrecer una plataforma centralizada donde los usuarios pueden visualizar información relevante sobre movilidad, consultar datos meteorológicos relacionados con el tráfico y generar reportes.

---

# 🎯 Objetivos

- Centralizar información relacionada con el tráfico.
- Registrar eventos de emergencia.
- Consultar el historial de eventos.
- Gestionar ubicaciones favoritas.
- Integrar información meteorológica.
- Facilitar la consulta mediante una interfaz web intuitiva.

---

# ✨ Características

- 🚦 Consulta del estado del tráfico.
- 🌦 Integración con información del clima.
- 🚑 Registro de emergencias.
- ❤️ Gestión de ubicaciones favoritas.
- 📜 Historial de consultas.
- 📄 Generación de documentos PDF.
- 💾 Persistencia de datos mediante MongoDB.
- 🖥 Arquitectura MVC.

---

# 🛠 Tecnologías utilizadas

- ASP.NET Core MVC
- .NET 10
- C#
- MongoDB
- MongoDB.Driver
- Razor Views
- HTML5
- CSS3
- Bootstrap
- JavaScript
- iText7
- SharpCompress
- Snappier

---

# 📂 Arquitectura

El proyecto sigue el patrón **MVC (Model-View-Controller)**.

```
TrafficFlow
│
├── Controllers
├── Models
├── Services
├── Views
├── wwwroot
├── Properties
├── appsettings.json
└── Program.cs
```

---

# 📁 Estructura del proyecto

## Controllers

Contienen la lógica que recibe las solicitudes HTTP y coordina la interacción entre las vistas y los servicios.

Ejemplos encontrados:

- HomeController
- EmergencyController
- FavoritesController
- HistoryController

---

## Models

Representan las entidades del sistema.

Entre ellas se encuentran modelos para:

- Emergencias
- Historial
- Favoritos
- Información meteorológica
- Ubicaciones
- Datos del tráfico

---

## Services

Contienen toda la lógica de negocio.

Se encargan de:

- Acceder a MongoDB.
- Consumir servicios externos.
- Procesar información.
- Generar respuestas para los controladores.

---

## Views

Implementadas mediante Razor Pages.

Se encargan de mostrar toda la interfaz de usuario.

---

## wwwroot

Archivos estáticos:

- CSS
- JavaScript
- Imágenes
- Bootstrap

---

# 🗄 Base de datos

El proyecto utiliza **MongoDB** como base de datos NoSQL.

Las colecciones almacenan información relacionada con:

- Historial
- Emergencias
- Favoritos
- Datos consultados

---

# ⚙ Requisitos

Antes de ejecutar el proyecto debes tener instalado:

- Visual Studio 2022 o superior
- .NET SDK 10
- MongoDB
- Git

---

# 🚀 Instalación

## 1. Clonar el repositorio

```bash
git clone https://github.com/Almarales24/TrafficFlow.git
```

---

## 2. Entrar al proyecto

```bash
cd TrafficFlow
```

---

## 3. Restaurar paquetes

```bash
dotnet restore
```

---

## 4. Configurar MongoDB

Editar el archivo:

```
appsettings.json
```

Agregar la configuración correspondiente.

Ejemplo:

```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "TrafficFlow"
  }
}
```

---

## 5. Ejecutar

```bash
dotnet run
```

o desde Visual Studio:

```
F5
```

---

# 🧩 Funcionalidades principales

## Gestión de Emergencias

Permite registrar y consultar eventos de emergencia.

Incluye:

- Registro
- Consulta
- Almacenamiento
- Administración

---

## Historial

Permite visualizar todas las consultas realizadas.

---

## Favoritos

Permite guardar ubicaciones para acceder rápidamente a ellas.

---

## Información Meteorológica

Integra datos del clima para complementar la información del tráfico.

---

## Generación de PDF

El sistema permite generar documentos PDF mediante la librería **iText7**.

---

# 📄 Flujo de funcionamiento

```
Usuario

     │

     ▼

Controladores MVC

     │

     ▼

Servicios

     │

     ▼

MongoDB

     │

     ▼

Respuesta al usuario
```

---

# 📸 Capturas

Puedes agregar imágenes aquí.

## Página principal

```
/images/home.png
```

---

## Consulta de tráfico

```
/images/trafico.png
```

---

## Emergencias

```
/images/emergencias.png
```

---

## Favoritos

```
/images/favoritos.png
```

---

# 📈 Futuras mejoras

- Integración con mapas en tiempo real.
- Notificaciones automáticas.
- Geolocalización.
- Aplicación móvil.
- Autenticación mediante OAuth.
- Panel administrativo.
- Estadísticas.
- Dashboard interactivo.

---

# 👨‍💻 Autor

**Almarales24**

GitHub:

https://github.com/Almarales24

---

# 📄 Licencia

Este proyecto se distribuye bajo la licencia MIT.

Consulta el archivo **LICENSE** para más información.
