namespace CodingTest.Controllers;

[ApiController]
[Route("api/customer")]
public class CustomerController(
    ICustomerService service,
    IInternalNotificationService notificationService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<Customer>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> GetCustomers([FromQuery] CustomerFilters filters)
    {
        var customers = await service.GetAllAsync(filters);
        if (customers.Count == 0) return NoContent();
        return Ok(customers);
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> PostCustomers([FromBody] List<Customer> customers)
    {
        await service.AddCustomersAsync(customers);
        
        if (notificationService.HasNotifications)
        {
            var notificationWithStatusCode = notificationService.Notifications.FirstOrDefault(f => f.HttpStatusCode != null);
            if (notificationWithStatusCode != null)
                return StatusCode(notificationWithStatusCode.HttpStatusCode.GetHashCode(), BaseResponseError.New(notification.Notifications));

            return BadRequest(BaseResponseError.New(notification.Notifications));
        }

        return Ok();
    }
}
