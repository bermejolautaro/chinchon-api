using Chinchon.API.Dtos;
using Chinchon.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chinchon.API.Controllers
{
    [ApiController]
    [Route("api/rpc/chinchon")]
    public class ChinchonRpcController : Controller
    {
        private readonly IGameRepository _gameRepository;

        public ChinchonRpcController(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        [HttpPost("game/{gameGuid}/players/{playerGuid}/pick-card-from-deck")]
        public async Task<ActionResult<PlayerStateDto>> PickCardFromDeck(Guid gameGuid, Guid playerGuid)
        {
            return (await _gameRepository.PickCardFromDeck(gameGuid, playerGuid))
                .Match<ActionResult<PlayerStateDto>>(
                    success: (state) => Ok(state),
                    error: (message) => BadRequest(message)
                );
        }

        [HttpPost("game/{gameGuid}/players/{playerGuid}/pick-card-from-pile")]
        public async Task<ActionResult<PlayerStateDto>> PickCardFromPile(Guid gameGuid, Guid playerGuid)
        {
            return (await _gameRepository.PickCardFromPile(gameGuid, playerGuid))
                .Match<ActionResult<PlayerStateDto>>(
                    success: (state) => Ok(state),
                    error: (message) => BadRequest(message)
                );
        }

        [HttpPost("game/{gameGuid}/players/{playerGuid}/discard-card")]
        public async Task<ActionResult<PlayerStateDto>> DiscardCard(Guid gameGuid, Guid playerGuid, CardDto card)
        {
            return (await _gameRepository.DiscardCard(gameGuid, playerGuid, new Card(card.Suit, card.Rank)))
                .Match<ActionResult<PlayerStateDto>>(
                    success: (state) => Ok(state),
                    error: (message) => BadRequest(message)
                );
        }

        [HttpPost("game/{gameGuid}/players/{playerGuid}/cut")]
        public async Task<ActionResult<PlayerStateDto>> Cut(Guid gameGuid, Guid playerGuid, CutDto cutDto)
        {
            if (!Group.TryCreateGroup(cutDto.FirstGroup.ToCards(), out Group? firstGroup))
            {
                firstGroup = null;
            }

            if (!Group.TryCreateGroup(cutDto.SecondGroup.ToCards(), out Group? secondGroup))
            {
                secondGroup = null;
            };

            return (await _gameRepository.Cut(gameGuid, playerGuid, firstGroup, secondGroup, cutDto.CardToCutWith?.ToCard()))
                .Match<ActionResult<PlayerStateDto>>(
                    success: (state) => Ok(state),
                    error: (message) => BadRequest(message)
                );
        }
    }

    public class CutDto
    {
        public IEnumerable<CardDto> FirstGroup { get; set; } = Enumerable.Empty<CardDto>();
        public IEnumerable<CardDto> SecondGroup { get; set; } = Enumerable.Empty<CardDto>();
        public CardDto? CardToCutWith { get; set; }
    }
}