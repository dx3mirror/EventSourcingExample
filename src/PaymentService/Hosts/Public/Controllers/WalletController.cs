using Microsoft.AspNetCore.Mvc;
using PaymentService.Contracts.Requests;
using PaymentService.Handlers.Commands.WalletCreate;
using PaymentService.Handlers.Commands.WalletDeposit;
using PaymentService.Handlers.Querys.OwnerGetBalance;
using PaymentService.Handlers.Querys.WalletGetBalance;
using Wolverine;

namespace PaymentService.Hosts.Controllers
{
    [Route("wallets")]
    public class WalletController(IMessageBus bus) : ControllerBase
    {
        /// <summary>
        /// Шина сообщений для отправки команд.
        /// </summary>
        private readonly IMessageBus _bus = bus;

        [HttpPost] 
        [ProducesResponseType(typeof(CreateWalletResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateWalletRequest request, CancellationToken cancellationToken)
        {
            var cmd = new WalletCreateCommand(request.OwnerId);
            var walletId = await _bus.InvokeAsync<Guid>(cmd, cancellationToken);
            return Ok(walletId);
        }

        [HttpPost("/deposit")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Deposit([FromBody]DepositRequest request, CancellationToken cancellationToken)
        {
            var command = new WalletDepositCommand(request.WalletId, request.OwnerId, request.Amount);
            await _bus.InvokeAsync(command, cancellationToken);
            return NoContent();
        }

        [HttpGet("{walletId:guid}/balance")]
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetBalance([FromRoute]Guid walletId, [FromQuery] Guid ownerId, CancellationToken cancellationToken)
        {
            var balance = await _bus.InvokeAsync<decimal>(new WalletGetBalanceQuery(walletId, ownerId), cancellationToken);
            return Ok(balance);
        }

        [HttpGet("owner/{ownerId:guid}/balance")]
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetBalanceOwner([FromRoute] Guid ownerId, CancellationToken cancellationToken)
        {
            var balance = await _bus.InvokeAsync<decimal?>(new OwnerGetBalanceQuery(ownerId), cancellationToken);
            return Ok(balance);
        }

    }
}
