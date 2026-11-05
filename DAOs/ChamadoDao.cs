using HelpDeskAPI.Models;
using System.Collections.ObjectModel;

namespace HelpDeskAPI.DAOs
{
    // TODO Lucas: receber HelpDeskContext no construtor e implementar as operacoes de ChamadoDao.
    // Os campos abaixo sao o rascunho original; revisar sua necessidade ao escrever as operacoes.
    // Os dados do chamado pertencem ao Model Chamado; o DAO deve acessar o banco pelo contexto.
    // TODO Lucas: implementar as consultas por situacao e prioridade e integra-las ao controller.
    public class ChamadoDao
    {
        int id;
        String titulo;
        String descricao;
        String Status;
        String propriedade;
        DateTime DataCriacao;
        Collection<Interacao> Interacoes = new Collection<Interacao>();
    }
}
