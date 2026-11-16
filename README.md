# HelpDeskAPI

API REST de um sistema de suporte técnico. A aplicação permite cadastrar usuários, autenticar clientes e atendentes, abrir chamados e registrar interações relacionadas aos chamados.

## Tecnologias

- **.NET 10 / ASP.NET Core**: hospeda a API HTTP e executa os controllers.
- **Entity Framework Core 10**: mapeia as classes C# para tabelas e executa consultas SQL.
- **SQLite**: banco local armazenado no arquivo `helpdesk.db`.
- **JWT Bearer**: autenticação por token e autorização por perfil.
- **BCrypt**: geração e verificação segura de hashes de senha.
- **OpenAPI**: descrição da API durante o desenvolvimento.

## Estrutura de pastas

```text
HelpDeskAPI/
├── Controllers/       Endpoints HTTP, validação de entrada e autorização
├── DAOs/              Consultas e operações de persistência com EF Core
├── Data/              DbContext e migrations do banco
│   └── Migrations/    Histórico versionado da estrutura do banco
├── DTOs/              Objetos de entrada e saída da API
├── Models/            Entidades de domínio mapeadas para o banco
├── Services/          Regras reutilizáveis, como autenticação e JWT
├── Properties/        Configurações de execução do Visual Studio
├── Program.cs         Inicialização, dependências e pipeline HTTP
├── appsettings*.json  Configurações por ambiente
├── *.http             Exemplos de chamadas HTTP
└── helpdesk.db        Banco SQLite local
```

## Responsabilidade de cada pasta

### `Controllers`
É a porta de entrada da API. Cada controller recebe uma requisição HTTP, verifica o usuário autenticado, valida regras de acesso, chama um DAO ou serviço e devolve um status HTTP com dados ou erro.

- `AuthController`: recebe credenciais e devolve um token JWT.
- `UsuariosController`: cadastra, consulta, atualiza e exclui usuários.
- `ChamadosController`: controla o ciclo de vida dos chamados.
- `InteracoesController`: controla mensagens relacionadas aos chamados.

### `DAOs`
DAO significa *Data Access Object*. Essa camada encapsula o acesso ao `HelpDeskContext`. Os controllers não montam consultas SQL diretamente: eles chamam métodos como `BuscarPorIdAsync`, `ListaTodosAsync` e `AdicionarAsync`.

### `Data`
Contém o `HelpDeskContext`, que representa a sessão do Entity Framework Core com o banco, e o histórico de migrations que cria ou altera tabelas, índices e relacionamentos.

### `DTOs`
DTO significa *Data Transfer Object*. Os DTOs definem quais campos podem entrar ou sair de determinados endpoints. Isso é importante para não receber ou retornar a entidade inteira sem controle. Em especial, `UsuarioRespostaDto` não possui `SenhaHash`.

### `Models`
Contém as entidades principais do domínio: `Usuario`, `Chamado` e `Interacao`. Essas classes possuem propriedades persistidas, relacionamentos e validações declarativas com DataAnnotations.

### `Services`
Contém lógica reutilizável que não pertence diretamente a um endpoint. `AuthService` consulta usuários, verifica BCrypt, cria claims JWT e gera hashes de senha.

### `Properties`
Contém configurações usadas pelo Visual Studio e pelo `dotnet run`, como portas HTTP/HTTPS e o ambiente `Development`.

## Descrição arquivo a arquivo

### Raiz

| Arquivo | Responsabilidade |
|---|---|
| `Program.cs` | Monta a aplicação, registra dependências, configura SQLite, JWT, migrations, tratamento de exceções e rotas. |
| `HelpDeskAPI.csproj` | Define que o projeto usa .NET 10 e lista os pacotes NuGet. |
| `HelpDeskAPI.slnx` | Arquivo da solução usado pelo Visual Studio para agrupar o projeto. |
| `appsettings.json` | Configuração padrão, conexão SQLite, níveis de log e chave JWT de desenvolvimento. |
| `appsettings.Development.json` | Sobrescritas de configuração para o ambiente de desenvolvimento. |
| `helpdesk.db` | Arquivo físico do banco SQLite local. Não contém código C#. |
| `WeatherForecast.cs` | Classe residual do template do ASP.NET Core; não participa dos fluxos do HelpDesk. |
| `HelpDeskAPI.http` | Requisição simples de exemplo gerada pelo template. |
| `HelpDeskAPI-Demo.http` | Roteiro de testes manuais para cadastro, login, chamados e interações. |
| `teste-git.txt` | Arquivo auxiliar de teste do repositório, sem participação na aplicação. |
| `.gitignore` | Define arquivos locais ou gerados que não devem ser versionados. |
| `.gitattributes` | Define normalização de finais de linha e comportamentos do Git. |

### `Controllers`

| Arquivo | Responsabilidade |
|---|---|
| `AuthController.cs` | Expõe `POST /Auth/login`; chama `AuthService` e retorna `401` para credenciais inválidas ou o token em `200`. |
| `UsuariosController.cs` | Expõe cadastro e operações de usuários; transforma entidades em `UsuarioRespostaDto` para não vazar a senha. |
| `ChamadosController.cs` | Expõe listagem, busca, criação, atualização, exclusão e filtros por status/prioridade. Clientes veem seus chamados; atendentes podem administrar chamados. |
| `InteracoesController.cs` | Expõe listagem, busca, criação, atualização, exclusão e filtros por chamado/usuário. Também verifica acesso ao chamado pai. |

### `DAOs`

| Arquivo | Responsabilidade |
|---|---|
| `UsuarioDao.cs` | Persiste usuários e consulta por ID, nome e perfil. O método legado `BuscarPorStatusAsync` filtra a propriedade `Perfil`, pois `Usuario` não tem `Status`. |
| `ChamadoDao.cs` | Persiste chamados, atualiza status/prioridade e consulta por ID, status e prioridade. |
| `InteracaoDao.cs` | Persiste interações e consulta por ID, chamado e usuário. |

### `Data`

| Arquivo | Responsabilidade |
|---|---|
| `HelpDeskContext.cs` | Declara os `DbSet`s e configura índice único de e-mail, datas padrão e chaves estrangeiras. |
| `Migrations/20261107185717_CriarBancoHelpDesk.cs` | Migration executável que cria as tabelas `Usuarios`, `Chamados` e `Interacoes`, índices e relacionamentos. |
| `Migrations/20261107185717_CriarBancoHelpDesk.Designer.cs` | Arquivo gerado pelo EF Core com o modelo usado pela migration. Não deve ser editado manualmente. |
| `Migrations/HelpDeskContextModelSnapshot.cs` | Snapshot gerado pelo EF Core para comparar o modelo atual com futuras migrations. |

### `Models`

| Arquivo | Responsabilidade |
|---|---|
| `Usuario.cs` | Entidade de usuário, com nome, e-mail, hash de senha e perfil. |
| `Chamado.cs` | Entidade de suporte, com título, descrição, status, prioridade, autor e interações. |
| `Interacao.cs` | Entidade de mensagem, com texto, data, chamado e usuário autor. |

### `DTOs`

| Arquivo | Responsabilidade |
|---|---|
| `UsuarioCadastroDto.cs` | Valida nome, e-mail e senha no cadastro. |
| `UsuarioAtuliazacaoDtos.cs` | Valida nome e e-mail na atualização de usuário. O nome do arquivo mantém a grafia existente no projeto. |
| `UsuarioRespostaDto.cs` | Define a resposta pública de usuário sem `SenhaHash`. |
| `UsuarioLoginDto.cs` | Modelo auxiliar para dados de login; o controller atualmente usa o record `LoginRequest`. |

### `Services` e `Properties`

| Arquivo | Responsabilidade |
|---|---|
| `Services/AuthService.cs` | Verifica credenciais, cria claims JWT e gera hashes BCrypt. |
| `Properties/launchSettings.json` | Define URLs locais HTTP/HTTPS e o ambiente de desenvolvimento. |

## Fluxo completo: do cliente ao banco

```text
Cliente HTTP
	|
	| 1. Envia JSON e, quando necessário, Authorization: Bearer <JWT>
	v
ASP.NET Core / Kestrel
	|
	| 2. HTTPS redirection
	| 3. Autenticação JWT popula HttpContext.User
	| 4. Autorização avalia [Authorize] e roles
	v
Controller
	|
	| 5. Model binding converte JSON em DTO/Model
	| 6. ApiController/DataAnnotations validam o corpo
	| 7. Controller verifica regras de negócio e visibilidade
	v
Service ou DAO
	|
	| 8. AuthService trata hash/JWT quando é autenticação
	| 9. DAO usa HelpDeskContext e LINQ/EF Core
	v
HelpDeskContext / Entity Framework Core
	|
	| 10. EF Core traduz LINQ em SQL e acompanha entidades
	| 11. SaveChangesAsync persiste alterações
	v
SQLite - helpdesk.db
	|
	| 12. Índices, chaves estrangeiras e defaults são aplicados
	v
Resposta HTTP
	|
	| 13. Controller retorna 200, 201, 204, 400, 401, 403 ou 404
	| 14. Exceções de persistência passam pelo middleware global em Program.cs
	v
Cliente recebe JSON ou ProblemDetails
```

## Fluxo de autenticação

1. O cliente chama `POST /Auth/login` com e-mail e senha.
2. `AuthController` chama `AuthService.LoginAsync`.
3. `AuthService` busca o usuário por e-mail no SQLite.
4. BCrypt compara a senha recebida com `SenhaHash`.
5. Em caso de sucesso, o serviço cria um JWT com `NameIdentifier`, `Email` e `Role`.
6. O cliente envia o token nas chamadas protegidas:

```text
Authorization: Bearer <token>
```

7. O middleware JWT valida assinatura e validade do token.
8. Os controllers usam `User.FindFirstValue(...)` e `User.IsInRole(...)` para aplicar permissões.

## Fluxo de cadastro de chamado

1. O cliente autenticado envia título, descrição, status e prioridade.
2. `ChamadosController` verifica os campos e valores permitidos.
3. O `UsuarioId` é obtido do token, não do corpo recebido.
4. `ChamadoDao.AdicionarAsync` adiciona a entidade ao contexto.
5. `SaveChangesAsync` grava a linha em `Chamados`.
6. O banco gera o ID e a data de criação.
7. A API retorna `201 Created` com o chamado criado.

## Fluxo de uma interação

1. O cliente envia `POST /api/interacoes` com `Mensagem` e `ChamadoId`.
2. O controller verifica se a mensagem existe e se o usuário pode acessar o chamado.
3. O autor é definido pelo `NameIdentifier` do JWT.
4. `InteracaoDao` salva a mensagem com as chaves estrangeiras.
5. Em consultas, Clientes recebem apenas dados dos próprios chamados; Atendentes podem consultar o conjunto permitido pela rota.

## Banco e relacionamentos

```text
Usuarios (1) -------- (N) Chamados
	|                         |
	|                         +---- (N) Interacoes
	|
	+------------------------ (N) Interacoes
```

- `Usuarios.Email` possui índice único.
- `Chamados.UsuarioId` referencia `Usuarios.Id`.
- `Interacoes.ChamadoId` referencia `Chamados.Id`.
- `Interacoes.UsuarioId` referencia `Usuarios.Id`.
- Os relacionamentos usam `DeleteBehavior.NoAction`, portanto o banco não apaga registros dependentes automaticamente.
- Tentar duplicar e-mail ou excluir registro com dependências pode gerar conflito tratado pelo middleware global.

## Tratamento de erros

- Validação de entrada: o `[ApiController]` e DataAnnotations retornam `400 Bad Request`.
- Credenciais inválidas: `401 Unauthorized`.
- Token válido sem permissão de role: `403 Forbidden`.
- Registro inexistente ou fora do escopo de acesso: normalmente `404 Not Found`.
- E-mail duplicado e conflitos de chave estrangeira SQLite: `409 Conflict` com `ProblemDetails`.
- Exceções não previstas: `500 Internal Server Error` com mensagem genérica, sem detalhes do banco.

## Executando o projeto

```powershell
dotnet restore
dotnet run
```

As URLs locais são definidas em `Properties/launchSettings.json`:

- HTTP: `http://localhost:5078`
- HTTPS: `https://localhost:7235`

Ao iniciar, `Program.cs` executa `Database.MigrateAsync()`. Assim, migrations pendentes são aplicadas automaticamente ao `helpdesk.db`.

## Observação de segurança

A chave JWT presente em `appsettings.json` é uma chave de desenvolvimento. Em produção, ela deve ser fornecida por variável de ambiente ou secret manager, nunca mantida como segredo fixo no repositório. A listagem de usuários deve ser protegida com `[Authorize(Roles = "Atendente")]` caso a regra de negócio exija que Clientes não possam listar usuários.
