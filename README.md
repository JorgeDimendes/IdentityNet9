# 🔐 ASP.NET Core Web API - Identity, DTOs & Services

Uma Web API desenvolvida para estudo e prática de **autenticação**, **autorização** e **gerenciamento de usuários/roles** com **ASP.NET Core Identity**, aplicando boas práticas de arquitetura como a camada de **Services** e **DTOs**.

## 📌 Sobre o Projeto

O objetivo principal deste projeto é demonstrar a implementação do ASP.NET Core Identity de forma desacoplada das entidades de domínio e controllers, promovendo um código limpo, testável e de fácil manutenção.

### 🎯 Principais Aprendizados e Conceitos Aplicados

- **ASP.NET Core Identity**: Configuração e gerenciamento de usuários, senhas e claims.
- **Roles & Claims**: Autenticação e controle de acesso baseado em papéis (ex: `Admin`, `User`).
- **Data Transfer Objects (DTOs)**: Separação das requisições/respostas HTTP do modelo interno da aplicação.
- **Service Pattern**: Camada de serviço intermediária responsável pelas regras de negócio e comunicação com o `UserManager` / `RoleManager`.
- **JWT (JSON Web Tokens)** *(se aplicável)*: Geração e validação de tokens para autenticação.

## 🛠️ Tecnologias Utilizadas

- **Linguagem**: C# (.NET 10.0)
- **Framework**: ASP.NET Core Web API
- **Segurança**: ASP.NET Core Identity / Entity Framework Core Identity
- **Documentação**: Scalar / OpenAPI
