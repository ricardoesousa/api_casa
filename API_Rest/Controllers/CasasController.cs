using System;
using System.Linq;
using API_Rest.Data;
using API_Rest.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace API_Rest.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    //[Authorize]
    public class CasasController : ControllerBase
    {

        private readonly ApplicationDbContext database;
        public CasasController(ApplicationDbContext database)
        {
            this.database = database;
        }


        [HttpGet]
        public IActionResult Get()
        {
                if (database.Casas.Count() == 0)
            {
                Response.StatusCode = 404;
                return new ObjectResult(new { msg = "Não há casas cadastradas!" });
            }
            var casas = database.Casas.ToList();
            return Ok(casas);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                Casa casa = database.Casas.First(c => c.Id == id);
                return Ok(casa);
            }
            catch (Exception e)
            {
                Response.StatusCode = 404;
                return new ObjectResult(new { msg = "Casa não encontrada!" });
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] CasaTemp cTemp)
        {
            if (cTemp.Nome != null && cTemp.Endereco != null)
            {
                if (database.Casas.Any(c => c.Nome == cTemp.Nome))
                {
                    Response.StatusCode = 409;
                    return new ObjectResult(new { msg = "O nome da casa já foi utilizado, favor escolher outro" });
                }
                Casa c = new Casa();
                c.Nome = cTemp.Nome;
                c.Endereco = cTemp.Endereco;
                database.Casas.Add(c);
                database.SaveChanges();
                Response.StatusCode = 201;
                return new ObjectResult(new { msg = "Casa cadastrada com sucesso!" });
            }
            else
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "Todos os dados são de preenchimento obrigatório!" });
            }
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody]Casa casa)
        {
            casa.Id = id;
            if (casa.Id > 0)
            {
                try
                {
                    var c = database.Casas.First(ctemp => ctemp.Id == casa.Id);
                    if (c != null)
                    {
                        c.Nome = casa.Nome != null ? casa.Nome : c.Nome;
                        c.Endereco = casa.Endereco != null ? casa.Endereco : c.Endereco;
                        database.SaveChanges();
                        Response.StatusCode = 200;
                        return new ObjectResult(new { msg = "Casa alterada com sucesso!" });
                    }
                    else
                    {
                        Response.StatusCode = 404;
                        return new ObjectResult(new { msg = "casa não encontrada!" });
                    }

                }
                catch
                {
                    Response.StatusCode = 404;
                    return new ObjectResult(new { msg = "casa não encontrada!" });
                }
            }
            else
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "O Id da casa é inválido!" });

            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Casa casa)
        {
            casa.Id = id;
            if (casa.Id > 0)
            {
                try
                {
                    var c = database.Casas.First(ctemp => ctemp.Id == casa.Id);
                    if (casa.Nome != null && casa.Endereco != null)
                    {
                        c.Nome = casa.Nome;
                        c.Endereco = casa.Endereco;
                        database.SaveChanges();
                        Response.StatusCode = 200;
                        return new ObjectResult(new { msg = "Casa alterada com sucesso!" });
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        return new ObjectResult(new { msg = "todos os campos são de preenchimento obrigatório!" });
                    }

                }
                catch
                {
                    Response.StatusCode = 404;
                    return new ObjectResult(new { msg = "casa não encontrada!" });
                }
            }
            else
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "O Id da casa é inválido!" });

            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                Casa casa = database.Casas.First(c => c.Id == id);
                database.Casas.Remove(casa);
                database.SaveChanges();
                return Ok(new { msg = "Casa deletada com sucesso!" });
            }
            catch (Exception e)
            {
                Response.StatusCode = 404;
                return new ObjectResult(new { msg = "Casa não encontrada!" });
            }
        }

        [HttpGet("asc")]
        public IActionResult Listar_Asc()
        {

            if (database.Casas.Count() == 0)
            {
                Response.StatusCode = 404;
                return new ObjectResult(new { msg = "Não há casas cadastradas!" });
            }

            var casas = database.Casas.OrderBy(s => s.Nome).ToList();
            return Ok(casas);
        }

        [HttpGet("desc")]
        public IActionResult Listar_Desc()
        {
            if (database.Casas.Count() == 0)
            {
                Response.StatusCode = 404;
                return new ObjectResult(new { msg = "Não há casas cadastradas!" });
            }
            var casas = database.Casas.OrderByDescending(s => s.Nome).ToList();
            return Ok(casas);
        }

        [HttpGet("nome/{nome}")]
        public IActionResult Busca_Nome(string nome)
        {
            
            if (nome.Length > 0)
            {
                try
                {
                    Casa casa = database.Casas.First(c => c.Nome == nome);
                    return Ok(casa);
                }
                catch (Exception e)
                {
                    Response.StatusCode = 404;
                    return new ObjectResult(new { msg = "Nome não encontrado!" });
                }
            }
            else
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "O nome precisa ter, pelo menos, um caracter" });
            }
        }

        public class CasaTemp
        {
            public string Nome { get; set; }
            public string Endereco { get; set; }
        }
    }
}