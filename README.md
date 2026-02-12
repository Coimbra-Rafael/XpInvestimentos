XpInvestimentos - Backend .NET
Este projeto foi desenvolvido como parte de um desafio técnico baseado nas diretrizes de preparação para a entrevista da XP Investimentos. O objetivo é demonstrar competências em arquitetura de sistemas, processamento assíncrono e integração com serviços de infraestrutura.

🚀 Tecnologias e Arquitetura
Runtime: .NET (C#)

Banco de Dados: SQL Server

Mensageria: RabbitMQ

Containerização: Docker & Docker Compose

Padrões: Alinhado com boas práticas de desenvolvimento backend (Clean Architecture/DDD).

🛠️ Pré-requisitos
Para rodar o projeto, você precisará de:

.NET SDK

Docker Desktop

⚙️ Configuração da Infraestrutura
O projeto utiliza Docker Compose para orquestrar as dependências de infraestrutura. É obrigatório rodar o compose para que a aplicação tenha acesso ao banco de dados e ao broker de mensagens.

1. Subir os Serviços (RabbitMQ & SQL Server)
Na raiz do repositório, execute:

Bash
docker-compose up -d
Nota: O SQL Server será iniciado na porta 1433 e o painel de gerenciamento do RabbitMQ estará disponível em http://localhost:15672 (guest/guest).

2. Executar a Aplicação
Após os containers estarem em estado healthy, execute:

Bash
dotnet restore
dotnet run --project NomeDoProjeto.Api
📌 Contexto do Projeto
A implementação segue os requisitos detalhados no PDF de preparação técnica da XP, focando em:

Consistência de dados.

Escalabilidade através de filas (RabbitMQ).

Persistência robusta em SQL Server.

📂 Estrutura de Pastas
API: Endpoints e controllers.

Application: Regras de negócio e comandos.

Infrastructure: Configurações do DbContext e publishers/consumers do RabbitMQ.

Domain: Entidades e interfaces fundamentais.

Autor
Rafael Coimbra

LinkedIn
    https://www.linkedin.com/in/rafael-coimbra-03897019a/

GitHub
    https://github.com/Coimbra-Rafael
