namespace CodingTest.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    public IActionResult PostCustomers([FromBody] List<Customer> models)
    {
        var customers = service.AddCustomers(models);
        
        if (notificationService.HasNotifications)
        {
            var notificationWithStatusCode = notificationService.Notifications.FirstOrDefault(f => f.HttpStatusCode != null);
            if (notificationWithStatusCode != null)
                return StatusCode(notificationWithStatusCode.HttpStatusCode.GetHashCode(), notificationService.Notifications);
        }

        if (customers?.Count == 0)
            return BadRequest(new { Message = notificationService.Notifications });

        return Ok(new
        {
            Message = notificationService.Notifications,
            Result = customers
        });
    }
}
