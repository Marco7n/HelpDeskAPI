using HelpDeskAPI.Models;
using System.Collections.ObjectModel;

namespace HelpDeskAPI.DAOs
{
    public class ChamadoDao
    {
        int id;
        String titulo;
        String descricao;
        String Status;
        String propriedade;
        DateTime DataCriacao;
        Collection<Interacao> Interacoes = new Collection<Interacao>
    }
}
