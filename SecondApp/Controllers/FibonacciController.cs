using System;
using System.Web.Http;
using Common.Helpers;
using Common.Messages;
using EasyNetQ;

namespace SecondApp.Controllers
{
    public class FibonacciController : ApiController
    {
        private readonly IFibonacciHelper _fibonacciHelper;

        public FibonacciController()
        {
            _fibonacciHelper = new FibonacciHelper();
        }

        public IHttpActionResult GetNextFibonacciNumber(int number)
        {
            try
            {
                if (number <= 0) return BadRequest();

                var nextNumber = _fibonacciHelper.GetNextNumber(number);

                if (nextNumber <= 0) return BadRequest();

                using (var bus = RabbitHutch.CreateBus("host=localhost"))
                {
                    bus.PubSub.PublishAsync(new FibonacciMessage()
                    {
                        Number = nextNumber
                    });
                }

                return Ok(nextNumber);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }
    }
}
