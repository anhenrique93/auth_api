# Descrição do Microserviço de Autenticação:

Este é um serviço de autenticação desenvolvido em .NET 6, reunindo todas as funcionalidades essenciais para autenticação. Inclui métodos como registo, login, logout, delete, update e troca de password. Além disso, proporciona autenticação JWT com refresh token e integração com Swagger para facilitar os testes.

Na implementação, optei por utilizar o PostgreSQL, com todas as consultas e stored procedures documentadas no ficheiro database.sql.

Contudo, para quem preferir utilizar uma base de dados diferente, basta criar um novo repositorio seguindo o contrato presente na interface IRepository e IUserRepository disponibilizadas.

# Authentication Miccroservice Description
This is an authentication service developed in .NET 6, encompassing all essential authentication functionalities. It includes methods such as registration, login, logout, delete, update, and password change. Additionally, it offers JWT authentication with refresh token and integrates with Swagger for streamlined testing.

In this implementation, PostgreSQL was chosen, with all queries and stored procedures documented in the database.sql file.

However, for those who prefer using a different database, they only need to create a repository for that database based on the cotract provided by IRepository and IUserRepository interfaces.
