# 🎬 ProjectM Backend  

API de reviews de filmes desenvolvida em .NET 9.0 com arquitetura Minimal API e ASPIRE.
- 🏗️ Projeto em construção...
- [Repositório do frontend](https://github.com/Kaikeeksr/ProjectMFrontend)

## ⚙️ Funcionalidades  
- Autenticação JWT de usuários
- CRUD de usuarios e reviews usando MongoDB
- Sistema de avaliação
- Saúde da API e monitoramento (ASPIREDashboards)
- Orchestration com .NET Aspire

## 🚀 Começando

### Pré-requisitos
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- MongoDB (local ou Atlas))

### 👨‍💻 Instalação
```bash
git clone https://github.com/Kaikeeksr/ProjectMBackend
cd ProjectMBackend

# Restaurar pacotes
dotnet restore
```

## 🔌 Configuração
```
// Copiar o conteúdo do arquivo .env-example. Criar um .env e preencher as variáveis de ambiente
DB_PASS=
DB_USER=
JWT_KEY=
LOCALHOST=
```

### 🚀 Subindo a API
```bash
# Executar com Aspire
dotnet run --project ProjectM.AppHost
````

## 🌟 .NET Aspire
O projeto utiliza os componentes:
- `Aspire.Hosting` para orquestração
- Dashboards para monitoramento de:
  - Saúde dos serviços
  - Métricas de performance
  - Dependências

## 🛠 Stack Tecnológica
- **Plataforma**: .NET 9.0
- **Arquitetura**: Minimal APIs
- **Cloud-Native**: .NET Aspire
- **Banco de Dados**: MongoDB
- **Autenticação**: JWT + 

## 📊 Endpoints Principais
| Método | Endpoint          | Descrição                 |
|--------|-------------------|---------------------------|
| GET    | `/User/Insert`    | Criar uma conta           |
| POST   | `/User/Login`     | Login                     |
| POST   | `/Reviews/Insert` | Adicionar review          |
| GET    | `/Review/FindAll` | Buscar reviews de um user |

**Contato**: [E-mail](kaikeksr@outlook.com) | [LinkedIn](https://www.linkedin.com/in/kaikerocha/)
