# StreamFlow - Modular monolith
This is the repository of a modular monolith for the fictitious application "StreamFlow" from the subject "Arquitectura de Sistemas" at the Universidad Católica del Norte. All the necessary tools and commands to run the project are described below.

## Pre-requisites
- [.NET SDK](https://dotnet.microsoft.com/es-es/download) (version 9.0.4)
- [MySQL](https://www.mysql.com/) (version 8.0.29) 
- [PostgreSQL](https://www.postgresql.org/) (version 15.6.0)
- [MariaDB](https://mariadb.org/) (version 10.7.4)
- [MongoDB](https://www.mongodb.com/) (version 5.0.3)
- [Git](https://git-scm.com/) (version 2.49.0)

## Installation and configuration

1. **Clone the repository**
```bash
git clone https://github.com/NachoXx25/Monolito-Modular.git
```

2. **Navigate to the project directory**
```bash
cd Monolito-Modular
```

3. **Restore project dependencies**
```bash
dotnet restore
```

4. **Create a ```.env``` file on the root of the project and fill the environment variables**
```bash
cp .env.example .env
```

In the ```.env``` file, replace:

- ```your_jwt_secret_key``` with your JWT secret key
- ```your_db``` with the name of each database (except for MongoDB)
- ```your_user```,```your_password``` with the credentials for each database
- ```mysql_port```, ```mariadb_port```, ```postgresql_port```, ```mongodb_port``` with the port for each database
- ```your_mongodb_db_name``` with your MongoDB database name

Once you have replaced everything, save the changes and move on to the next step.


5. **Run the project**
```bash
dotnet run
```

The server will start on port **5024** and the **seeders** will be created automatically. Access the API via http://localhost:5024.

## Table of contents
As mentioned in the previous section, the seeders are loaded automatically with the ```dotnet run``` command.

The seeder contains:
- 152 users (150 randomly generated and 2 identified users, administrator and customer respectively). This data will be added to the Auth, User and Bill contexts.
- 350 bills
- 451 videos (450 randomly generated and 1 identified test video)

## Authors
- [@Sebastián Núñez](https://github.com/2kSebaNG)
- [@Ignacio Valenzuela](https://github.com/NachoXx25)