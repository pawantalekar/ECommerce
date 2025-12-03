using AuthService.Api.Commands;
using AuthService.Api.Queries;
using Ecom.Application.AuthService.Application.DTO;
using Ecom.Application.ReviewService.Application.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecom.Gateway.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminApprovalController : ControllerBase
{
    private readonly IMediator _mediator;
    public AdminApprovalController(IMediator mediator) => _mediator = mediator;

    private Guid CurrentAdminId => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [Authorize(Roles = "Admin")]
    [HttpGet("reviews/pending")]
    public async Task<ActionResult<List<PendingReviewDto>>> GetPendingReviews()
        => Ok(await _mediator.Send(new GetPendingReviewsQuery()));


    [Authorize(Roles = "Admin")]
    [HttpPost("reviews/{id}/approve")]
    public async Task<IActionResult> ApproveReview(Guid id)
        => Ok(await _mediator.Send(new ApproveReviewCommand(id, CurrentAdminId)));


    [Authorize(Roles = "Admin")]
    [HttpPost("reviews/{id}/reject")]
    public async Task<IActionResult> RejectReview(Guid id)
        => Ok(await _mediator.Send(new RejectReviewCommand(id, CurrentAdminId)));


    [Authorize(Roles = "Admin")]
    [HttpGet("seller-requests/pending")]
    public async Task<ActionResult<List<SellerRequestDto>>> GetPendingSellerRequests()
        => Ok(await _mediator.Send(new GetPendingSellerRequestsQuery()));


    [Authorize(Roles = "Admin")]
    [HttpPost("seller-requests/{id}/approve")]
    public async Task<IActionResult> ApproveSellerRequest(Guid id)
        => Ok(await _mediator.Send(new ApproveSellerRequestCommand(id, CurrentAdminId)));


    [Authorize(Roles = "Admin")]
    [HttpPost("seller-requests/{id}/reject")]
    public async Task<IActionResult> RejectSellerRequest(Guid id)
        => Ok(await _mediator.Send(new RejectSellerRequestCommand(id, CurrentAdminId)));


    [HttpPost("seller-requests")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> CreateSellerRequest()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var userName = User.FindFirst(ClaimTypes.Name)?.Value
                       ?? User.FindFirst("name")?.Value
                       ?? "Unknown User";
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value
                        ?? User.FindFirst("email")?.Value
                        ?? "no-email@example.com";

        try
        {
            await _mediator.Send(new CreateSellerRequestCommand(userId, userName, userEmail));
            return Ok(new { message = "Your request to become a seller has been sent!" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch
        {
            return StatusCode(500, new { message = "An error occurred. Please try again." });
        }
    }
}