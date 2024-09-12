# LightSensorSystem

## Running the Project

In order to run the system, follow these steps:

1. Clone the project repository
2. Build the solution
3. Launch the API (local db will be created as soon as you access endpoints that utilize it)
4. Start the light sensor simulator
5. Once started, the simulator begins taking measurements from the initial start time and continues to collect data every 15 minutes, subsequently sending this data to the server in one hour

To authenticate and obtain a JWT, use the `POST /clients/login` endpoint in the ClientController. Provide the client's login credentials `(Login & Password)` to receive the token. To register new clients, utilize the `POST /clients/new` endpoint.

## Technologies Used
- .NET 8 
- ASP.NET Core
- MS SQL (Local DB) & EF
- xUnit & Moq
- Automapper & Serilog
- Custom JWT Authentication

## About the Simulation
- The simulation considers surfaces illuminated on a clear sunny day. 
- To approximetly represent illuminance transision [Exponential interpolation](https://stackoverflow.com/questions/53629449/limited-exponential-growth-interpolation-algorithm#:~:text=Which-,results,-in%20the%20linear) was used. 
- Approximate values for sunrise, sunset, peak (Full daylight) conditions are sourced from the [Illuminance](https://en.wikipedia.org/wiki/Lux#:~:text=under%20various%20conditions%3A-,Illuminance,-(lux)) section
