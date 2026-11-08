[1mdiff --git a/Controllers/ChamadosController.cs b/Controllers/ChamadosController.cs[m
[1mindex da205b7..d8939dc 100644[m
[1m--- a/Controllers/ChamadosController.cs[m
[1m+++ b/Controllers/ChamadosController.cs[m
[36m@@ -3,16 +3,20 @@[m [musing HelpDeskAPI.DAOs;[m
 using HelpDeskAPI.Models;[m
 using Microsoft.Data.Sqlite;[m
 using Microsoft.EntityFrameworkCore;[m
[32m+[m[32musing Microsoft.AspNetCore.Authorization;[m
[32m+[m[32musing System.Security.Claims;[m
 [m
 namespace HelpDeskAPI.Controllers[m
 {[m
     [ApiController][m
     [Route("[controller]")][m
[32m+[m[32m    [Authorize(Roles = "Cliente,Atendente")][m
     public class ChamadosController : ControllerBase[m
     {[m
         private const int CodigoRestricaoChaveEstrangeira = 787;[m
 [m
         private readonly ChamadoDao _chamadoDao;[m
[32m+[m[32m        private int? UsuarioId => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;[m
         public ChamadosController(ChamadoDao chamadoDao)[m
         {[m
             _chamadoDao = chamadoDao;[m
[36m@@ -21,26 +25,29 @@[m [mnamespace HelpDeskAPI.Controllers[m
 [m
         public async Task<IActionResult> Listar()[m
         {[m
[31m-[m
[32m+[m[32m            if (UsuarioId is not int usuarioId) return Forbid();[m
             var lista = await _chamadoDao.ListaTodosAsync();[m
[31m-            return Ok(lista);[m
[32m+[m[32m            return Ok(User.IsInRole("Atendente") ? lista : lista.Where(chamado => chamado.UsuarioId == usuarioId));[m
 [m
 [m
         }[m
         [HttpGet("{id}")][m
         public async Task<IActionResult> Buscar(int id)[m
         {[m
[32m+[m[32m            if (UsuarioId is not int usuarioId) return Forbid();[m
             var busca = await _chamadoDao.BuscarPorIdAsync(id);[m
[31m-            if (busca == null)[m
[32m+[m[32m            if (busca == null || (!User.IsInRole("Atendente") && busca.UsuarioId != usuarioId))[m
             {[m
                 return NotFound();[m
             }[m
             return Ok(busca);[m
         }[m
         [HttpPost][m
[32m+[m[32m        [Authorize(Roles = "Cliente")][m
         public async Task<IActionResult> Cadastrar([FromBody] Chamado chamado)[m
         {[m
[31m-[m
[32m+[m[32m            if (UsuarioId is not int usuarioId) return Forbid();[m
[32m+[m[32m            chamado.UsuarioId = usuarioId;[m
             try[m
             {[m
                 await _chamadoDao.AdicionarAsync(chamado);[m
[36m@@ -59,6 +66,7 @@[m [mnamespace HelpDeskAPI.Controllers[m
 [m
         }[m
         [HttpPut("{id}")][m
[32m+[m[32m        [Authorize(Roles = "Atendente")][m
 [m
         public async Task<IActionResult> Atualizar(int id, [FromBody] Chamado dadosAtualizados)[m
         {[m
[36m@@ -75,6 +83,7 @@[m [mnamespace HelpDeskAPI.Controllers[m
 [m
         }[m
         [HttpDelete("{id}")][m
[32m+[m[32m        [Authorize(Roles = "Atendente")][m
 [m
         public async Task<IActionResult> Excluir(int id)[m
         {[m
[36m@@ -99,10 +108,9 @@[m [mnamespace HelpDeskAPI.Controllers[m
 [m
         public async Task<IActionResult> BuscarPorStatusAsync(string status)[m
         {[m
[31m-[m
[32m+[m[32m            if (UsuarioId is not int usuarioId) return Forbid();[m
             var buscarStatus = await _chamadoDao.BuscarPorStatusAsync(status);[m
[31m-[m
[31m-            return Ok(buscarStatus);[m
[32m+[m[32m            return Ok(User.IsInRole("Atendente") ? buscarStatus : buscarStatus.Where(chamado => chamado.UsuarioId == usuarioId));[m
 [m
 [m
 [m
[36m@@ -111,10 +119,9 @@[m [mnamespace HelpDeskAPI.Controllers[m
 [m
         public async Task<IActionResult> BuscarPorPrioridadeAsync(string prioridade)[m
         {[m
[31m-[m
[32m+[m[32m            if (UsuarioId is not int usuarioId) return Forbid();[m
             var buscarPrioridade = await _chamadoDao.BuscarPorPrioridadeAsync(prioridade);[m
[31m-[m
[31m-            return Ok(buscarPrioridade);[m
[32m+[m[32m            return Ok(User.IsInRole("Atendente") ? buscarPrioridade : buscarPrioridade.Where(chamado => chamado.UsuarioId == usuarioId));[m
         }[m
     }[m
 }[m
[1mdiff --git a/Controllers/InteracoesController.cs b/Controllers/InteracoesController.cs[m
[1mindex c01dd78..1c984e2 100644[m
[1m--- a/Controllers/InteracoesController.cs[m
[1m+++ b/Controllers/InteracoesController.cs[m
[36m@@ -1,39 +1,54 @@[m
 ﻿using Microsoft.AspNetCore.Mvc;[m
 using HelpDeskAPI.DAOs;[m
 using HelpDeskAPI.Models;[m
[32m+[m[32musing Microsoft.AspNetCore.Authorization;[m
[32m+[m[32musing System.Security.Claims;[m
 [m
 [m
 namespace HelpDeskAPI.Controllers[m
 {[m
     [ApiController][m
     [Route("[controller]")][m
[32m+[m[32m    [Authorize(Roles = "Cliente,Atendente")][m
 [m
     public class InteracoesController : ControllerBase[m
     {[m
         private readonly InteracaoDao _interacaoDao;[m
[32m+[m[32m        private readonly ChamadoDao _chamadoDao;[m
[32m+[m[32m        private int? UsuarioId => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;[m
 [m
[31m-        public InteracoesController(InteracaoDao interacaoDao)[m
[32m+[m[32m        public InteracoesController(InteracaoDao interacaoDao, ChamadoDao chamadoDao)[m
         {[m
 [m
             _interacaoDao = interacaoDao;[m
[32m+[m[32m            _chamadoDao = chamadoDao;[m
 [m
         }[m
[32m+[m[32m        private async Task<bool> PodeAcessarChamado(int chamadoId, int usuarioId)[m
[32m+[m[32m        {[m
[32m+[m[32m            var chamado = await _chamadoDao.BuscarPorIdAsync(chamadoId);[m
[32m+[m[32m            return chamado != null && (User.IsInRole("Atendente") || chamado.UsuarioId == usuarioId);[m
[32m+[m[32m        }[m
         [HttpGet][m
 [m
         public async Task<IActionResult> Listar()[m
         {[m
[31m-[m
[32m+[m[32m            if (UsuarioId is not int usuarioId) return Forbid();[m
             var lista = await _interacaoDao.ListaTodosAsync();[m
[31m-            return Ok(lista);[m
[32m+[m[32m            if (User.IsInRole("Atendente")) return Ok(lista);[m
[32m+[m[32m            var chamados = await _chamadoDao.ListaTodosAsync();[m
[32m+[m[32m            var meusChamados = chamados.Where(chamado => chamado.UsuarioId == usuarioId)[m
[32m+[m[32m                .Select(chamado => chamado.Id).ToHashSet();[m
[32m+[m[32m            return Ok(lista.Where(interacao => meusChamados.Contains(interacao.ChamadoId)));[m
 [m
         }[m
         [HttpGet("{id}")][m
 [m
         public async Task<IActionResult> Buscar(int id)[m
         {[m
[31m-[m
[32m+[m[32m            if (UsuarioId is not int usuarioId) return Forbid();[m
             var buscar = await _interacaoDao.BuscarPorIdAsync(id);[m
[31m-            if (buscar == null)[m
[32m+[m[32m            if (buscar == null || !await PodeAcessarChamado(buscar.ChamadoId, usuarioId))[m
             {[m
                 return NotFound();[m
             }[m
[36m@@ -45,7 +60,9 @@[m [mnamespace HelpDeskAPI.Controllers[m
 [m
         public async Task<IActionResult> Cadastrar([FromBody] Interacao interacao)[m
         {[m
[31m-[m
[32m+[m[32m            if (UsuarioId is not int usuarioId) return Forbid();[m
[32m+[m[32m            if (!await PodeAcessarChamado(interacao.ChamadoId, usuarioId)) return NotFound();[m
[32m+[m[32m            interacao.UsuarioId = usuarioId;[m
             await _interacaoDao.AdicionarAsync(interacao);[m
 [m
             return CreatedAtAction([m
[36m@@ -59,7 +76,10 @@[m [mnamespace HelpDeskAPI.Controllers[m
 [m
         public async Task<IActionResult> Atualizar(int id, [FromBody] Interacao dadosAtualizados)[m
         {[m
[31m-[m
[32m+[m[32m            if (UsuarioId is not int usuarioId) return Forbid();[m
[32m+[m[32m            var interacao = await _interacaoDao.BuscarPorIdAsync(id);[m
[32m+[m[32m            if (interacao == null || !await PodeAcessarChamado(interacao.ChamadoId, usuarioId)) return NotFound();[m
[32m+[m[32m            if (interacao.UsuarioId != usuarioId) return Forbid();[m
             var atualizar = await _interacaoDao.AtualizarAsync(id, dadosAtualizados);[m
             if (!atualizar)[m
             {[m
[36m@@ -73,7 +93,10 @@[m [mnamespace HelpDeskAPI.Controllers[m
 [m
         public async Task<IActionResult> Deletar(int id)[m
         {[m
[31m-[m
[32m+[m[32m            if (UsuarioId is not int usuarioId) return Forbid();[m
[32m+[m[32m            var interacao = await _interacaoDao.BuscarPorIdAsync(id);[m
[32m+[m[32m            if (interacao == null || !await PodeAcessarChamado(interacao.ChamadoId, usuarioId)) return NotFound();[m
[32m+[m[32m            if (interacao.UsuarioId != usuarioId) return Forbid();[m
             var deletar = await _interacaoDao.ExcluirAsync(id);[m
             if (!deletar)[m
             {[m
[36m@@ -87,7 +110,8 @@[m [mnamespace HelpDeskAPI.Controllers[m
 [m
         public async Task<IActionResult> BuscarPorChamado(int chamadoId)[m
         {[m
[31m-[m
[32m+[m[32m            if (UsuarioId is not int usuarioId) return Forbid();[m
[32m+[m[32m            if (!await PodeAcessarChamado(chamadoId, usuarioId)) return NotFound();[m
             var BuscarChamado = await _interacaoDao.BuscarPorChamadoAsync(chamadoId);[m
             return Ok(BuscarChamado);[m
 [m
[36m@@ -96,9 +120,13 @@[m [mnamespace HelpDeskAPI.Controllers[m
 [m
         public async Task<IActionResult> BuscarPorUsuario(int usuarioId)[m
         {[m
[31m-[m
[32m+[m[32m            if (UsuarioId is not int meuId) return Forbid();[m
             var buscarUsuario = await _interacaoDao.BuscarPorUsuarioAsync(usuarioId);[m
[31m-            return Ok(buscarUsuario);[m
[32m+[m[32m            if (User.IsInRole("Atendente")) return Ok(buscarUsuario);[m
[32m+[m[32m            var chamados = await _chamadoDao.ListaTodosAsync();[m
[32m+[m[32m            var meusChamados = chamados.Where(chamado => chamado.UsuarioId == meuId)[m
[32m+[m[32m                .Select(chamado => chamado.Id).ToHashSet();[m
[32m+[m[32m            return Ok(buscarUsuario.Where(interacao => meusChamados.Contains(interacao.ChamadoId)));[m
 [m
 [m
         }[m
[1mdiff --git a/DAOs/ChamadoDao.cs b/DAOs/ChamadoDao.cs[m
[1mindex 2a8fe7f..34ab599 100644[m
[1m--- a/DAOs/ChamadoDao.cs[m
[1m+++ b/DAOs/ChamadoDao.cs[m
[36m@@ -40,8 +40,6 @@[m [mnamespace HelpDeskAPI.DAOs[m
                 return false;[m
             }[m
 [m
[31m-            find.Titulo = dadosAtualizados.Titulo;[m
[31m-            find.Descricao = dadosAtualizados.Descricao;[m
             find.Status = dadosAtualizados.Status;[m
             find.Prioridade = dadosAtualizados.Prioridade;[m
             await _contexto.SaveChangesAsync();[m
