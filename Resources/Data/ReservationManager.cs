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
    public class ReservationManager
    {
        //Lists for flights, reservations, and airport
        private List<Flights> flights = new List<Flights>();
        private List<Reservations> reservations = new List<Reservations>();
        private List<Airport> airports = new List<Airport>();
        private readonly SQLiteAsyncConnection database;


        public ReservationManager(string dbPath)
        {
            //Constructor to initialize the database connection and create the Reservations table
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<Reservations>().Wait();

            //load flights, reservations, and airports
            LoadFlights();
            LoadReservations().Wait();
            LoadAirports();
        }

        //Load flights method extracting from flights.csv file
        private void LoadFlights()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Data", "flights.csv");
            string[] lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {

                string[] parts = line.Split(',');
                Flights flight = new Flights
                {
                    Flightcode = parts[0],
                    Airline = parts[1],
                    Originairportcode = parts[2],
                    Destinationairportcode = parts[3],
                    Dayofweek = parts[4],
                    Departuretime = parts[5],
                    AvailableSeats = int.Parse(parts[6]),
                    Cost = double.Parse(parts[7])
                };

                flights.Add(flight);
            }
        }


        //Load reservations from database

        public Task<List<Reservations>> LoadReservations()
        {
            //table reservations into list
            return database.Table<Reservations>().ToListAsync();
        }

        public Task<Reservations> LoadReservations(string reservationcode)
        {
            //get reservation by code , where code of reservation is equal to the code passed in
            return database.Table<Reservations>().Where(r => r.Reservationcode == reservationcode).FirstOrDefaultAsync();
        }

        //UPDATE

        //first see if data is already in the database
        public async Task<int> SaveReservation(Reservations reservation)
        {
            if (reservation.Reservationcode == null)
                throw new ArgumentNullException(nameof(reservation.Reservationcode));

            var existing = await database.Table<Reservations>()
                .Where(r => r.Reservationcode == reservation.Reservationcode)
                .FirstOrDefaultAsync();

            if (existing != null)
            {
                return await database.UpdateAsync(reservation);
            }
            else
            {
                return await database.InsertAsync(reservation);
            }
        }
        //Load Airports from airports.csv file

        private void LoadAirports()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Data", "airports.csv");
            string[] lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {

                string[] values = line.Split(',');
                Airport airport = new Airport
                {
                    Airportcode = values[0],
                    Airportname = values[1]
                };

                airports.Add(airport);
            }
        }

        //Find flights based on origin, destination, and day of the week
        public List<Flights> FindFlights(string originairportcode, string destinationairportcode, string dayofweek)
        {
            List<Flights> foundFlights = new List<Flights>();
            foreach (Flights flight in flights)
            {
                if (flight.Originairportcode == originairportcode &&
                    flight.Destinationairportcode == destinationairportcode &&
                    flight.Dayofweek == dayofweek &&
                    flight.AvailableSeats > 0)
                {
                    foundFlights.Add(flight);
                }
            }
            return foundFlights;
        }

        //Make Reservations Method
        public Reservations MakeReservation(Flights flight, string name, string citizenship)
        {
            //Exception Handling as per assignment requirements
            if (flight == null) throw new ArgumentNullException(nameof(flight), "No flight selected");
            if (flight.AvailableSeats <= 0) throw new InvalidOperationException("No available seats on this flight");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty", nameof(name));
            if (string.IsNullOrWhiteSpace(citizenship)) throw new ArgumentException("Citizenship cannot be empty", nameof(citizenship));


            // Generate a unique reservation code in the form LDDDD
            var random = new Random();
            char letter = (char)('A' + random.Next(0, 26));
            string digits = random.Next(10000, 99999).ToString();
            string reservationCode = $"{letter}{digits}";


            //Create a new reservation object
            var reservation = new Reservations
            {
                Reservationcode = reservationCode,
                Flightcode = flight.Flightcode,
                Airline = flight.Airline,
                Cost = flight.Cost,
                Name = name,
                Citizenship = citizenship,
                Status = true
            };

            //Save the reservation to the database
            SaveReservation(reservation).Wait();

            // Decrease available seats
            flight.AvailableSeats--;

            //Persist the updated reservations
            Persist();

            return reservation;

        }

        //Persist the reservations to the database
        public async Task Persist()
        {
            // Get all reservations from the database
            var allReservations = await LoadReservations();

            // Define the file path for the binary file
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Data", "reservations.dat");

            // Serialize the reservations list to JSON
            var json = System.Text.Json.JsonSerializer.Serialize(allReservations);

            // Write the JSON string to the file asynchronously
            await File.WriteAllTextAsync(filePath, json);
        }

        //Deletion of a reservation
        public async Task<int> DeleteReservation(string reservationCode)
        {
            var reservation = await LoadReservations(reservationCode);
            if (reservation != null)
                return await database.DeleteAsync(reservation);
            return 0;
        }

        //Get all reservations
        public List<Flights> GetAllFlights()
        {
            return new List<Flights>(flights);
        }
    }
}
