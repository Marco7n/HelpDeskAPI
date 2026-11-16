# HelpDeskAPI

API REST em ASP.NET Core com EF Core, MySQL e autenticação JWT. Usa .NET 10. Os testes foram feitos com MySQL 8.4.

## Rodar no Visual Studio

1. Abra `HelpDeskAPI.slnx`.
2. Com o MySQL Server em execução, abra `helpdesk.mysql.sql` no MySQL Workbench e execute o script inteiro. Ele cria o banco `helpdesk` e as tabelas que ainda estiverem faltando.
3. No Gerenciador de Soluções, clique com o botão direito no projeto **HelpDeskAPI** e selecione **Gerenciar Segredos de Usuário**. Em `secrets.json`, adicione a conexão com seu usuário, sua senha e a porta do MySQL.

   ```json
   {
     "ConnectionStrings": {
       "HelpDesk": "Server=127.0.0.1;Port=3306;Database=helpdesk;User ID=SEU_USUARIO;Password=SUA_SENHA;"
     }
   }
   ```

4. Selecione o perfil **https** e pressione **F5**. A API escuta em `https://localhost:7235`.
5. Abra `HelpDeskAPI-Demo.http` e envie as solicitações na ordem de cadastro, login, criação de chamado e criação de interação. Se a conta de exemplo já existir, comece pelo login ou troque o e-mail para criar outra conta.

Em desenvolvimento, a API gera uma chave JWT temporária quando `Jwt:Key` estiver ausente. Se reiniciar a API, faça login de novo para obter um token. Em outros ambientes, configure uma chave própria fora do repositório, por exemplo pela variável de ambiente `Jwt__Key`.

Confira os dados da demonstração no Workbench com estas consultas.

```sql
SELECT Id, Titulo, Status, UsuarioId FROM helpdesk.Chamados;
SELECT Id, Mensagem, ChamadoId, UsuarioId FROM helpdesk.Interacoes;
```
