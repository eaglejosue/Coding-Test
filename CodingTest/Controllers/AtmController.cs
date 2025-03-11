namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AtmController(
    IAtmService service,
    IInternalNotificationService notificationService) : ControllerBase
{
    private const string DefaultMessage = "Combinations available:";

    [HttpGet("payout/{amount}")]
    public async Task<IActionResult> GetPayoutCombinations(int amount)
    {
        var result = await service.GetPayoutCombinationsAsync(amount);
        var msgResult = DefaultMessage;

        if (notificationService.HasNotifications)
            msgResult = notificationService.Notifications.FirstOrDefault()!.Message;

        if (result.Count == 0)
            return BadRequest(new { Message = "Invalid amount. Unable to dispense the requested value with available denominations." });

        return Ok(new
        {
            Message = msgResult,
            Result = result
        });
    }
}
