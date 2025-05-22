using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;


namespace OlimpBack.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OlimpController : Controller
    {
        private readonly string _connectionString;
        public OlimpController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("Check-connection")]
        public async Task<IActionResult> CheckConnection()
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    return Ok("Connection to the database is successful.");
                }
                else
                {
                    return StatusCode(500, "Failed to connect to the database.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error connecting to database: {ex.Message}");
            }
        }

        
    }
}
