using Flights.Application.Common;
using Flights.Application.Flights.Commands;
using Flights.Application.Flights.Dtos;
using Flights.Application.Flights.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Flights.Api.Controllers
{
    [ApiController]
    [Route("api/flights")]
    public class FlightsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public FlightsController(IMediator mediator) => _mediator = mediator;


        /// <summary>
        /// Список рейсов (для всех ролей). Сортировка по Arrival ASC. Фильтры: Origin, Destination.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<FlightDto>), 200)]
        public async Task<IActionResult> Get([FromQuery] string? origin, [FromQuery] string? destination, CancellationToken ct)
        {
            var res = await _mediator.Send(new GetFlightsQuery(origin, destination), ct);
            return Ok(res.Value);
        }

        /// <summary>
        /// Добавление рейса (только Moderator)
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [Authorize(Roles = "Moderator")]
        [HttpPost]
        [ProducesResponseType(typeof(int), 200)]
        public async Task<IActionResult> Create([FromBody] CreateFlightDto dto, CancellationToken ct)
        {
            var res = await _mediator.Send(new CreateFlightCommand(dto), ct);
            return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
        }

        /// <summary>
        /// Редактирование статуса рейса (только Moderator)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [Authorize(Roles = "Moderator")]
        [HttpPatch("{id:int}/status")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> UpdateStatus([FromRoute] int id, [FromBody] UpdateFlightStatusDto dto, CancellationToken ct)
        {
            var res = await _mediator.Send(new UpdateFlightStatusCommand(id, dto), ct);
            return res.IsSuccess ? Ok() : NotFound(res.Error);
        }
    }
}