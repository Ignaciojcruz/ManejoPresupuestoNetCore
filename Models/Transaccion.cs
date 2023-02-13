﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuestoNetCore.Models
{
    public class Transaccion
    {
        public int Id { get; set; }
        
        
        public int UsuarioId { get; set; }

        
        [Display(Name = "Fecha Transacción")]
        [DataType(DataType.Date)]
        public DateTime FechaTransaccion { get; set; } = DateTime.Today;
        
        
        public double Monto { get; set; }

        
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar una categoría")]
        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }

        
        [StringLength(maximumLength: 1000, ErrorMessage = "La nota no puede pasar de {1} caracteres")]
        public string Nota { get; set; }

        
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar una cuenta")]
        [Display(Name ="Cuenta")]
        public int CuentaId { get; set; }
                


        [DisplayName("Tipo operación")]
        public TipoOperacion TipoOperacionId { get; set; } = TipoOperacion.Ingreso;
    }
}
