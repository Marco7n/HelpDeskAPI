using HelpDeskAPI.Models;
using System.Collections.ObjectModel;

namespace HelpDeskAPI.DAOs
{
    // TODO Marco: receber HelpDeskContext no construtor e implementar IChamadoDao.
    // Os campos abaixo sao o rascunho original; revisar sua necessidade ao escrever as operacoes.
    // Os dados do chamado pertencem ao Model Chamado; o DAO deve acessar o banco pelo contexto.
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
