﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private DataContext _dbcontext;

        public ValuesController(DataContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {

            var Allvalues = await  _dbcontext.Values.ToListAsync();
            return Ok(Allvalues);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var Allvalues = await _dbcontext.Values.SingleOrDefaultAsync(x=>x.Id == id);
            return Ok(Allvalues);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
