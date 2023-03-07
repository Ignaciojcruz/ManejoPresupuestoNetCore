using System.Diagnostics.Eventing.Reader;

namespace ManejoPresupuestoNetCore.Models
{
    public class PaginacionViewModel
    {
        public int Pagina { get; set; } = 1;
        private int recordsPorPagina = 10;
        private readonly int cantidadMaximaRecordsPorPagina = 50;

        public int RecordsPorPagina
        {
            get { return recordsPorPagina; }
            set { recordsPorPagina = (value > cantidadMaximaRecordsPorPagina) ? cantidadMaximaRecordsPorPagina : value;}
        }

        public int RecordsASaltar => RecordsPorPagina * (Pagina - 1);

    }
}
