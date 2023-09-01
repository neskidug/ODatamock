using Microsoft.AspNetCore.Mvc;
using OData_mock.Models;

namespace OData_mock.Interfaces
{
    public interface IOdatamocksController
    {
        ActionResult Get();

        ActionResult<Odatamock> Get([FromRoute] int key);
    }
}