namespace Api.Controllers;

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
        if (customers == null || customers?.Count == 0) return NoContent();
        return Ok(BaseResponse.New(customers, notificationService.Notifications));
    }

    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<>), (int)HttpStatusCode.BadRequest)]
    public IActionResult PostCustomers([FromBody] List<Customer> models)
    {
        var customers = service.AddCustomers(models);

        var notificationWithStatusCode = notificationService.Notifications?.FirstOrDefault(f => f.HttpStatusCode != null);
        if (notificationService.HasNotifications && notificationWithStatusCode != null)
            return StatusCode(
                notificationWithStatusCode.HttpStatusCode.GetHashCode(),
                BaseResponse.New<object>(null, notificationService.Notifications)
            );

        if (customers == null || customers?.Count == 0)
            return BadRequest(BaseResponse.New<object>(null, notificationService.Notifications));

        if (notificationService.HasNotifications)
            return StatusCode(
                HttpStatusCode.MultiStatus.GetHashCode(),
                BaseResponse.New(customers, notificationService.Notifications)
            );

        return Ok(BaseResponse.New(customers));
    }
}
