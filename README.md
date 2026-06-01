
# 🚀 Projeto: OniBus Express

API para consulta de rotas, busca de viagens e reserva de passagens de onibus.

---

## Regras de Negocio Implementadas:

- Não é possível reservar assento já ocupado.
- Não é possível reservar passagem para viagem já realizada.
- CPF precisa ter formato válido e dígito verificador correto.
- Código da reserva é único e legível, no formato `ABC-12345`.
- Cancelamento só é permitido até 2 horas antes da partida.


## Itens não implementados:
    - Construção de ambiente no docker.
    - Variaveis de ambiente para implementação no servidor.
    - Implementação de camada de segurança para expor API em ambiente produtivo. 




## 📌 Tecnologias utilizadas

- `.NET 10`: plataforma principal da API.
- `ASP.NET Core Controllers`: usado para expor endpoints HTTP de forma simples e organizada.
- `Entity Framework Core`: usado para mapear entidades C# para tabelas do SQL Server.
- `Microsoft.EntityFrameworkCore.SqlServer`: provider do EF Core para SQL Server.
- `Microsoft.AspNetCore.OpenApi`: geração do documento OpenAPI.
- `Swagger UI`: tela web para documentar e testar endpoints.
- `xUnit`: biblioteca de testes unitários.
- `Microsoft.EntityFrameworkCore.InMemory`: banco em memória usado nos testes de integração.

---

## 🏗️ Arquitetura 

O projeto foi separado em camadas para reduzir acoplamento e manter as responsabilidades claras:

- `OnibusExpress`: camada API. Contém controllers, Swagger e configuração HTTP.
- `OnibusExpress.Application`: camada de aplicação. Contém DTOs, validações e regras de negócio.
- `OnibusExpress.Domain`: camada de domínio. Contém entidades, contratos de repositório e exceções.
- `OnibusExpress.Infrastructure`: camada de infraestrutura. Contém EF Core, contexto do banco, repositórios e seed.
- `OnibusExpress.Tests`: testes unitários e de integração.

Decisões relevantes:

- Controllers não acessam o banco diretamente; eles chamam serviços da camada Application.
- Application depende de contratos do Domain, não de EF Core.
- Infrastructure implementa os contratos do Domain usando Entity Framework Core.
- As regras de negócio de reserva ficam centralizadas em `ReservasService`.
- O CPF é validado em um validador específico, separado dos controllers.
- O banco mantém índice único para impedir assento ativo duplicado na mesma viagem.

---

## ⚙️ Como rodar o projeto

Pre-requisitos:

- .NET 10 SDK
- SQL Server LocalDB ou SQL Server local
- Visual Studio, SSMS ou Azure Data Studio para consultar o banco, se desejar

Configure a connection string em `OnibusExpress/appsettings.Development.json`:

```json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=OnibusDb;Trusted_Connection=True;TrustServerCertificate=True"
```

Crie as tabelas no SQL Server executando:

```text
Scripts/create-sqlserver-tables.sql
```

Insira viagens de teste:

```text
Scripts/insert-test-viagens.sql
```

Restaure os pacotes e rode a API:

```powershell
dotnet restore OnibusExpress.slnx
dotnet run --project OnibusExpress
```

A API ficará disponível em:

```text
http://localhost:5286
```

---
## Como Rodar os Testes

Execute:

```powershell
dotnet restore OnibusExpress.slnx
dotnet test OnibusExpress.slnx
```

Os testes cobrem:

- Validação de CPF
- Bloqueio de assento já ocupado
- Cancelamento permitido até 2 horas antes da partida
- Bloqueio de cancelamento fora do prazo
- Geração de código de reserva único e legível
---

## Documentacao dos Endpoints

Com a API rodando em ambiente de desenvolvimento, acesse:

```text
http://localhost:5286/swagger
```

Endpoints principais:

| Metodo | Rota | Descricao |
| --- | --- | --- |
| GET | `/rotas` | Lista todas as rotas disponíveis |
| GET | `/viagens?origem={origem}&destino={destino}&data={data}` | Busca viagens por origem, destino e data |
| GET | `/viagens/{id}` | Consulta detalhes de uma viagem, incluindo assentos livres e ocupados |
| POST | `/reservas` | Cria uma reserva |
| GET | `/reservas/{codigo}` | Consulta uma reserva pelo código |
| DELETE | `/reservas/{codigo}` | Cancela uma reserva |

Exemplo de busca de viagens:

```text
GET http://localhost:5286/viagens?origem=Sao%20Paulo&destino=Rio%20de%20Janeiro&data=2026-06-02
```

Exemplo de reserva:

```json
{
  "nome": "Maria Silva",
  "cpf": "529.982.247-25",
  "email": "maria@email.com",
  "viagemId": 1,
  "assentoNumero": 7
}
```
---
### 🔹 Clonar o repositório

```bash
git clone https://github.com/ThaisAlmeidaDaSilva/OniBus-Express