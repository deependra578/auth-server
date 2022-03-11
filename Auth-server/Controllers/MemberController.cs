using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Auth_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class MemberController : ControllerBase
    {
        private readonly IJwtAuth jwtAuth;

        private readonly List<Member> lstMember = new List<Member>()
        {
            new Member{Id=1, Name="Shipra" },
            new Member {Id=2, Name="Avinash" },
            new Member{Id=3, Name="Deependra"}
        };
        public MemberController(IJwtAuth jwtAuth)
        {
            this.jwtAuth = jwtAuth;
        }
        // GET: api/<MemberController>
        [HttpGet]
        public IEnumerable<Data> Get()
        {
            using (System.IO.StreamReader r = new System.IO.StreamReader(Directory.GetCurrentDirectory() + @"\DB.json"))
            {
                string json1 = r.ReadToEnd();
                List<Data> items = JsonConvert.DeserializeObject<List<Data>>(json1);
                var a = items.Where(x => x.Id == "1").ToList();
                return items;
            }
        }

        // GET api/<MemberController>/5
        [HttpGet("{id}")]
        public Member GetMemberById(int id)
        {
            return lstMember.Find(x => x.Id == id);
        }

        // POST api/<MemberController>
        [AllowAnonymous]
        [HttpPost("authentication")]
        public IActionResult Authentication([FromBody] UserCredentials userCredential)
        {
            var token = jwtAuth.Authentication(userCredential.UserName, userCredential.Password);
            if (token == null)
                return Unauthorized();
            return Ok(token);
        }

        [HttpPost]
        public async Task<ActionResult> InsertDetails([FromBody] POC obj)
        {
            try
            {
                string path = Directory.GetCurrentDirectory() + @"\DB.json";
                if (string.IsNullOrEmpty(obj.Id))
                {
                    return BadRequest(new { code = HttpStatusCode.BadRequest, message = "Please Provide Id." });

                }
                if (string.IsNullOrEmpty(obj.Name))
                {
                    return BadRequest(new { code = HttpStatusCode.BadRequest, message = "Please Provide Name." });
                }
                List<Data> _data = new List<Data>();
                _data.Add(new Data()
                {
                    Id = obj.Id,
                    Name = obj.Name
                });
                string json = JsonConvert.SerializeObject(_data.ToArray());
                using (System.IO.StreamWriter writer = System.IO.File.AppendText(path))
                {
                    writer.Write(String.Format("{0}{1}", json, Environment.NewLine));
                }
                return Ok(new { code = HttpStatusCode.OK, message = "Details has been saved successfully." });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        // PUT api/<MemberController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] POC obj)
        {
            try
            {
                string _path = Directory.GetCurrentDirectory() + @"\DB.json";
                string json = System.IO.File.ReadAllText(_path);
                if (string.IsNullOrEmpty(obj.Id))
                {
                    return BadRequest(new { code = HttpStatusCode.BadRequest, message = "Please Provide Id." });



                }

                List<Data> items = JsonConvert.DeserializeObject<List<Data>>(json);
                var record = items.Where(x => x.Id == obj.Id).FirstOrDefault();
                if (record != null)
                {
                    if (!string.IsNullOrEmpty(obj.Name))
                    {
                        items[0].Name = obj.Name;
                    }

                }
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(items, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(_path, output);
                return Ok(new { code = HttpStatusCode.OK, message = "Details has been updated successfully." });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        // DELETE api/<MemberController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    public class Data
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class POC
    {
        public string[] arr { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
