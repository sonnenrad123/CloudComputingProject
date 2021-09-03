using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AgencijaWeb.Models
{
    public class Korisnik : TableEntity
    {
        public Korisnik()
        {
        }

        public Korisnik(string username, string ime, string prezime, int id)
        {
            this.username = username;
            this.ime = ime;
            this.prezime = prezime;
            this.id = id;
            PartitionKey = "Korisnik";
            RowKey = "Korisnik_" + id;
        }

        [Required]
        public string username { get; set; }
        [Required]
        public string ime { get; set; }
        [Required]
        public string prezime { get; set; }
        [Required]
        public int id { get; set; }
    }
}