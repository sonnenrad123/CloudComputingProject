using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AgencijaWeb.Models
{
    public class Agencija:TableEntity
    {
        public Agencija()
        {
        }

        public Agencija(string ime, string adresa, string grad, string drzava, int id)
        {
            this.ime = ime;
            this.adresa = adresa;
            this.grad = grad;
            this.drzava = drzava;
            this.id = id;
            PartitionKey = "Agencija";
            RowKey = "Agencija_" + id;
        }

        [Required]
        public string ime { get; set; }
        [Required]
        public string adresa { get; set; }
        [Required]
        public string grad { get; set; }
        [Required]
        public string drzava { get; set; }
        [Required]
        public int id { get; set; }

        
    }
}