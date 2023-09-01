namespace OData_mock.Controllers
{
    using Microsoft.AspNet.OData.Routing;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.OData.Query;
    using Microsoft.AspNetCore.OData.Routing.Controllers;
    using OData_mock.Models;
    using System;
    using System.Collections.Generic;
    using System.Net;

    [EnableQuery]
    public class OdatamocksController : ODataController
    {
        private readonly ILogger<OdatamocksController> _logger;
        static int NumberOfRequest = 0;
        private List<Odatamock> _data = new List<Odatamock>();
        public OdatamocksController(ILogger<OdatamocksController> logger)
        {
            _logger = logger;
        }


        static long concurrent = 0;
        [EnableQuery]
        [HttpGet()]
        [ODataRoute("Odatamocks")]
        public ActionResult Get()
        {
            long l = Interlocked.Increment(ref concurrent);
            //_logger.LogInformation($"Received a GET request number: {concurrent}");
            ActionResult actionResult = null;
            NumberOfRequest = Interlocked.Increment(ref NumberOfRequest);
            Console.WriteLine($"Requestnumber: {NumberOfRequest}");
            if (l == 5)
            {
                actionResult = new JsonResult("") { StatusCode = StatusCodes.Status429TooManyRequests };
                Console.WriteLine("Too many requests are occurring-----------------------------------------------------------------------------------");
            }
            else
            {
                Console.WriteLine($"Number of requests: {concurrent}");
                if (_data.Any() == false)
                {
                    _data.Add(new Odatamock(1, "Navnpaadata"));
                    //_data.Add(new Odatamock(2,"Navnpaaandetdata"));
                    Thread.Sleep(10);
                    actionResult = Ok(_data);
                }
            }
            Interlocked.Decrement(ref concurrent);
            return actionResult;
        }

        [EnableQuery]
        [HttpGet()]
        [ODataRoute("Odatamocks/({key})")]
        public ActionResult<Odatamock> Get([FromRoute] int key)
        {
            Odatamock odatamock = new Odatamock(1, "Mockdataname");

            Console.WriteLine(HttpStatusCode.OK);

            return Ok(odatamock);
        }




    }
}
