using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using System.Collections.ObjectModel;
using SQLiteNetExtensions.Attributes;

namespace TestAppKontur.Models
{
    public class ContactQuery
    {
        public ObservableCollection<Contact> Contacts { get; set; }
    }
    [Table("Contacts")]
    public class Contact
    {
        [JsonProperty("id")]
        [PrimaryKey]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("height")]
        public float Height { get; set; }

        [JsonProperty("biography")]
        public string Biography { get; set; }

        [JsonProperty("temperament")]
        public Temperaments Temperament { get; set; }

        [JsonProperty("educationPeriod")]
        [OneToOne]      // Many to one relationship with EducationPeriod
        public Educationperiod EducationPeriod { get; set; }

        [ForeignKey(typeof(Educationperiod))]
        public string EducationPeriodId { get; set; }
    }
    public enum Temperaments
    {
        melancholic, phlegmatic, sanguine, choleric
    }
    [Table("EducationPeriods")]
    public class Educationperiod
    {
        [PrimaryKey]
        public string Id { get; set; }
        [JsonProperty("start")]
        public DateTime Start { get; set; }

        [JsonProperty("end")]
        public DateTime End { get; set; }
    }
}
