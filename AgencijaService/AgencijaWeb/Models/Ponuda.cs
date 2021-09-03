using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AgencijaWeb.Models
{
    public class Ponuda:TableEntity
    {
        public Ponuda()
        {
        }

        public Ponuda(string ime, string opis, double cena, int id)
        {
            this.ime = ime;
            this.opis = opis;
            this.cena = cena;
            this.id = id;
            PartitionKey = "Ponuda";
            RowKey = "Ponuda_" + id;
            
        }

        [Required]
        public string ime { get; set; }
        [Required]
        public string opis { get; set; }
        [Required]
        public double cena { get; set; }
        [Required]
        public int id { get; set; }
    }
}