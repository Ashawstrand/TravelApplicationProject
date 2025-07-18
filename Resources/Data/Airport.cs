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
    public class Airport
    {
        public string Airportcode { get; set; }

        public string Airportname { get; set; }

        public override string ToString()
        {
            return $"{Airportcode} - {Airportname}";
        }
    }
}
