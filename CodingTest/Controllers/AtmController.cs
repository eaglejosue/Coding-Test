namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AtmController(
    IAtmService service,
    IInternalNotificationService notificationService) : ControllerBase
{
    private const string DefaultMessage = "Combinations available:";

    [HttpGet("payout/{amount}")]
    [ProducesResponseType(typeof(BaseResponse<>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetPayoutCombinations(int amount)
    {
        var result = await service.GetPayoutCombinationsAsync(amount);
        var msgResult = DefaultMessage;

        if (notificationService.HasNotifications)
            msgResult = notificationService.Notifications.FirstOrDefault()!.Message;

        if (result.Count == 0)
            return BadRequest(BaseResponse.New<object>(null, [new InternalNotification("Invalid amount.", "Unable to dispense the requested value with available denominations.")]));

        return Ok(BaseResponse.New(result, [new InternalNotification("Success.", msgResult)]));
    }
}
