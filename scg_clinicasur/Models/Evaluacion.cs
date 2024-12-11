﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace scg_clinicasur.Models
{
    [Table("evaluacion")]
    public class Evaluacion
    {
        [Key]
        public int id_evaluacion { get; set; }

        [Required(ErrorMessage = "Selecciona una capacitación")]
        [ForeignKey("Capacitacion")]
        [Column("id_capacitacion")]
        public int id_capacitacion { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [Column(TypeName = "TEXT")]
        public string descripcion { get; set; }

        [Required(ErrorMessage = "El tiempo de prueba es obligatorio")]
        [StringLength(255)]
        public string tiempo_prueba { get; set; }

        [Required(ErrorMessage = "El archivo es obligatorio")]
        [StringLength(255)]
        public string? archivo { get; set; }

        [ForeignKey("Usuario")]
        public int? id_usuario { get; set; }
        public Usuario? Usuario { get; set; }
        public virtual Capacitacion? Capacitacion { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime fecha_creacion { get; set; }
    }
}