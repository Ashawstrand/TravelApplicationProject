using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

////Ashley Shaw-Strand
////Group 4
////July 16,2025
////The Traveless App is designed to help users search flights, make reservations, and manage existing reservations. Users can input origin, destination, and day to find available flights. 
////The users can then select a flight to reserve after inputting their name and citizenship. 
//The system generates a unique reservation code and saves details once a reservation is made.
//Users can search for their existing reservation details by inputting either their reservation code, airline, or name.
//This application uses SQLite database and local data files to store data.

namespace TravelApplication.Resources.Data
{
    public class Reservations
    {
        [PrimaryKey]
        public string Reservationcode { get; set; } // Format: LDDDD
        public string Flightcode { get; set; }
        public string Airline { get; set; }
        public double Cost { get; set; }
        public string Name { get; set; }
        public string Citizenship { get; set; }
        public bool Status { get; set; }

        public Reservations() { }

        public override string ToString()
        {
            return $"{Reservationcode} - {Flightcode} ({Airline}) - {Name} - ${Cost} - Status: {(Status ? "Confirmed" : "Cancelled")}";
        }
    }
}
