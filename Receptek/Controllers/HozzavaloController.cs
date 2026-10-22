using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Receptek.Models;

namespace Receptek.Controllers
{
    [ApiController]
    [Route("hozzavalo")]
    public class HozzavaloController : ControllerBase
    {
        public string ConnectionString = "server=localhost;database=receptek;uid=root;";

        [HttpGet("all")]
        public object GetAll()
        {
            List<Hozzavalo> lista = new List<Hozzavalo>();
            var conn = new MySqlConnection(ConnectionString);

            conn.Open();

            string sql = "SELECT * FROM hozzavalo";
            var cmd = new MySqlCommand(sql, conn);
            var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                var h = new Hozzavalo
                {
                    Id = dr.GetInt32(0),
                    IngredientName = dr.GetString(1),
                    Quantity = dr.GetString(2),
                    Description = dr.IsDBNull(3) ? "" : dr.GetString(3),
                    AddedTime = dr.GetDateTime(4),
                    UpdateTime = dr.GetDateTime(5),
                    SzakacsId = dr.GetInt32(6)
                };

                lista.Add(h);
            }

            conn.Close();

            return new { message = "Sikeres lekerdezes", result = lista };
        }

        [HttpGet("byid")]
        public object GetById(int id)
        {
            var conn = new MySqlConnection(ConnectionString);
            conn.Open();

            string sql = "SELECT * FROM hozzavalo WHERE Id = @id";
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            var dr = cmd.ExecuteReader();

            if (!dr.Read())
            {
                conn.Close();
                return NotFound(new { message = "Nincs ilyen hozzavalo" });
            }

            var h = new Hozzavalo
            {
                Id = dr.GetInt32(0),
                IngredientName = dr.GetString(1),
                Quantity = dr.GetString(2),
                Description = dr.IsDBNull(3) ? "" : dr.GetString(3),
                AddedTime = dr.GetDateTime(4),
                UpdateTime = dr.GetDateTime(5),
                SzakacsId = dr.GetInt32(6)
            };

            conn.Close();

            return Ok(new { message = "Sikeres lekerdezes", result = h });
        }
        [HttpPost("add")]
        public object AddNew([FromBody] Hozzavalo uj)
        {
            var conn = new MySqlConnection(ConnectionString);

            conn.Open();

            string sql = "INSERT INTO hozzavalo (IngredientName, Quantity, Description, AddedTime, UpdateTime, SzakacsId) VALUES (@name, @quantity, @description, @added, @update, @szakacsId)";

            var cmd = new MySqlCommand(sql, conn);

            var now = DateTime.Now;

            cmd.Parameters.AddWithValue("@name", uj.IngredientName);
            cmd.Parameters.AddWithValue("@quantity", uj.Quantity);
            cmd.Parameters.AddWithValue("@description", uj.Description);
            cmd.Parameters.AddWithValue("@added", now);
            cmd.Parameters.AddWithValue("@update", now);
            cmd.Parameters.AddWithValue("@szakacsId", uj.SzakacsId);

            cmd.ExecuteNonQuery();

            conn.Close();

            return new { message = "Sikeres hozzaadas" };
        }

        [HttpPut("update")]
        public object Update(int id, [FromBody] Hozzavalo uj)
        {
            var conn = new MySqlConnection(ConnectionString);
            conn.Open();

            string sql = "UPDATE hozzavalo SET IngredientName = @name, Quantity = @quantity, Description = @desc, UpdateTime = @ut, SzakacsId = @sid WHERE Id = @id";

            var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@name", uj.IngredientName);
            cmd.Parameters.AddWithValue("@quantity", uj.Quantity);
            cmd.Parameters.AddWithValue("@desc", uj.Description);
            cmd.Parameters.AddWithValue("@ut", DateTime.Now);
            cmd.Parameters.AddWithValue("@sid", uj.SzakacsId);
            cmd.Parameters.AddWithValue("@id", id);

            int sorok = cmd.ExecuteNonQuery();

            conn.Close();

            if (sorok == 0)
            {
                return NotFound("Nincs ilyen hozzavalo!");
            }

            return Ok("Sikeres modositas!");
        }


    }
}