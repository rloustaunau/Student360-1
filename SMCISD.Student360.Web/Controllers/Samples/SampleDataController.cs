using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SMCISD.Student360.Api.Controllers
{
    [ApiController, AllowAnonymous]
    [Route("api/samples/[controller]")]
    public class SampleDataController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var model = await Task.Run(() =>
                new {
                    dataResult = new List<Object> {
                                new { usi=10, firstName="Doug", lastName="Loyd", grade=12, cohort=2020, school="SM High School"  },
                                new { usi=11, firstName="Juan", lastName="Perez", grade=10, cohort=2020, school="SM High School"  },
                                new { usi=12, firstName="Mario", lastName="Lopez", grade=11, cohort=2020, school="SM High School"  },
                                new { usi=13, firstName="Maria", lastName="Velez", grade=10, cohort=2020, school="SM High School"  },
                                new { usi=14, firstName="Rafa", lastName="Juarez", grade=9, cohort=2020, school="SM High School"  },
                                new { usi=15, firstName="Jose", lastName="Martinez", grade=9, cohort=2020, school="SM High School"  },
                                new { usi=16, firstName="Damian", lastName="Navarro", grade=12, cohort=2020, school="SM High School"  },
                            },
                    totalCount = 100,
                    queryExcecutionMs = 10,
                    columns = new string[] { "Usi", "First Name", "Last Name", "School" }
                }
            );

            return Ok(model);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] object data) 
        {
            var model = await Task.Run(() =>
                new {
                    dataResult = new List<Object> {
                                new { usi=10, firstName="Ordered Doug", lastName="Loyd", grade=12, cohort=2020, school="SM High School"  },
                                new { usi=11, firstName="Juan", lastName="Perez", grade=10, cohort=2020, school="SM High School"  },
                                new { usi=12, firstName="Mario", lastName="Lopez", grade=11, cohort=2020, school="SM High School"  },
                                new { usi=13, firstName="Maria", lastName="Velez", grade=10, cohort=2020, school="SM High School"  },
                                new { usi=14, firstName="Rafa", lastName="Juarez", grade=9, cohort=2020, school="SM High School"  },
                                new { usi=15, firstName="Jose", lastName="Martinez", grade=9, cohort=2020, school="SM High School"  },
                                new { usi=16, firstName="Damian", lastName="Navarro", grade=12, cohort=2020, school="SM High School"  },
                            },
                    totalCount = 7,
                    queryExcecutionMs = 20,
                    columns = new string[] { "Usi", "First Name", "Last Name", "School" }
                }
            );
            return Ok(model);
        }
    }
}
