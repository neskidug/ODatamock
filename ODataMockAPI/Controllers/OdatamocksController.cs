namespace OData_mock.Controllers
{
    using Interfaces;
    using Microsoft.AspNet.OData.Routing;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.OData.Query;
    using Microsoft.AspNetCore.OData.Routing.Controllers;
    using OData_mock.Models;
    using System;
    using System.Collections.Generic;
    using System.Net;

    [EnableQuery]
    public class OdatamocksController : ODataController, IOdatamocksController
    {
        private readonly ILogger<OdatamocksController> _logger;
        static int _numberOfRequest = 0;
        private List<Odatamock> _data = new List<Odatamock>();

        public OdatamocksController(ILogger<OdatamocksController> logger)
        {

            _logger = logger;
            _logger.LogInformation("I am creating a controller now");
        }


        static long concurrent = 0;
        [EnableQuery]
        [HttpGet()]
        [ODataRoute("Odatamocks")]
        public ActionResult Get()
        {
            long l = Interlocked.Increment(ref concurrent);
            ActionResult actionResult = null;
            if (l >= 5)
            {
                actionResult = new JsonResult("") { StatusCode = StatusCodes.Status429TooManyRequests };
                Console.WriteLine("Too many requests are occurring-----------------------------------------------------------------------------------");
            }
            else
            {
                if (_data.Any() == false)
                {
                    _data.Add(new Odatamock(1, "Navnpaadata"));
                    //_data.Add(new Odatamock(2,"Navnpaaandetdata"));
                    actionResult = Ok(_data);
                }
            }
            Thread.Sleep(10);
            Interlocked.Decrement(ref concurrent);
            return actionResult;
        }

        //[EnableQuery]
        //[HttpGet()]
        //[ODataRoute("({key})")]
        //public ActionResult<Odatamock> Get([FromRoute] int key)
        //{
        //    _logger.LogInformation("This is a message log");
        //    Odatamock odatamock = new Odatamock(1, "Mockdataname");
        //    Thread.Sleep(key);

        //    return Ok(odatamock);
        //}


        [EnableQuery]
        [HttpGet()]
        [ODataRoute("({key})")]
        public ActionResult<Odatamock> Get([FromRoute] int key)
        {
            _numberOfRequest = Interlocked.Increment(ref _numberOfRequest);
            _logger.LogInformation($"{_numberOfRequest}");
            long l = Interlocked.Increment(ref concurrent);
            ActionResult actionResult = null;
            if (l >= 51)
            {
                _logger.LogCritical("Too many requests are occurring-----------------------------------------------------------------------------------");
                actionResult = new JsonResult("") { StatusCode = StatusCodes.Status429TooManyRequests };
            }
            else
            {
                Odatamock odatamock = new Odatamock(1, "Mockdataname");
                actionResult = Ok(odatamock);
            }
            Thread.Sleep(key);
            Interlocked.Decrement(ref concurrent);

            return actionResult;
        }

    }
}
