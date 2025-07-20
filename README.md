
# Eshift_1

A .NET Core MVC web application using MySQL as the database. This guide explains how to set up and run the project locally.

---

## 🚀 Local Setup

### Prerequisites

Make sure you have the following installed:

- [.NET 6 SDK or later](https://dotnet.microsoft.com/en-us/download)
- [MySQL Server 8.x](https://dev.mysql.com/downloads/mysql/)
- (Optional) [Visual Studio 2022+](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- (Optional) [Node.js & npm](https://nodejs.org/) — if front-end packages are used

---

### 📥 1. Clone the Repository

```bash
git clone https://github.com/Hannahlene/Eshift_1.git
cd Eshift_1
```

---

### 🛠️ 2. Configure the Database

1. Open MySQL and create a new database:

```sql
CREATE DATABASE EshiftDb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

2. Open `appsettings.json` and update the connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=EshiftDb;User=root;Password=YourPassword;"
}
```

> Replace `YourPassword` with your actual MySQL root password or configured user credentials.

---

### 📦 3. Apply Entity Framework Migrations

If migrations are already included in the project, run:

```bash
dotnet ef database update
```

If not, generate initial migrations:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

> Make sure EF CLI tools are installed:  
> `dotnet tool install --global dotnet-ef`

---

### ▶️ 4. Run the Application

Start the app locally:

```bash
dotnet run
```

By default, the application runs at:

- `http://localhost:5000` or  
- `https://localhost:5001`

Open it in your browser.

---

### 🧪 5. Optional Setup Steps

- Add seed data or initial admin users if your project includes them.
- Configure `launchSettings.json` if needed for custom ports or profiles.

---

## 🧩 Troubleshooting

- **Database not connecting**: Check if MySQL is running and credentials are correct.
- **EF errors**: Ensure `Microsoft.EntityFrameworkCore.Design` is included and tools are installed.
- **Port issues**: You can run on a different port:
  ```bash
  dotnet run --urls "http://localhost:6000"
  ```

---

## ✅ To-Do or Contribute

- Add authentication or authorization (if not yet implemented)
- Improve front-end components
- Add tests

---

## 📄 License

This project is Own By Hannahlene

---

## 🙋‍♀️ Author

Developed by [Hannahlene](https://github.com/Hannahlene)
