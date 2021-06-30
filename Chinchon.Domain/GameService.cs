using System;
using System.Collections.Generic;
using System.Linq;
using static Chinchon.Domain.CardsService;

namespace Chinchon.Domain
{
    public static class GameService
    {

        public static GameState? DeserializeGameState(string serializedGameState)
        {
            GameStateOptions gameStateOptions = new GameStateOptions();
            var properties = typeof(GameStateOptions).GetProperties();

            var lines = serializedGameState
                .Split(Environment.NewLine)
                .Where(x => !string.IsNullOrEmpty(x));

            foreach (var line in lines)
            {
                if (line == null)
                {
                    break;
                }

                var splittedLine = line.Split("=", 2);
                var key = splittedLine[0];
                var value = splittedLine[1];

                foreach (var property in properties)
                {
                    if (property.Name != key)
                    {
                        continue;
                    }

                    if (property.PropertyType.IsAssignableFrom(typeof(string)))
                    {
                        property.SetValue(gameStateOptions, value);
                        break;
                    }

                    if (property.PropertyType.IsAssignableFrom(typeof(bool)))
                    {
                        property.SetValue(gameStateOptions, bool.Parse(value));
                        break;
                    }

                    if (property.PropertyType.IsAssignableFrom(typeof(int)))
                    {
                        property.SetValue(gameStateOptions, int.Parse(value));
                        break;
                    }

                    if (property.PropertyType.IsAssignableFrom(typeof(IEnumerable<Card>)))
                    {
                        var cards = string.IsNullOrEmpty(value)
                                ? Enumerable.Empty<Card>()
                                : value.Split(";").Select(x => DeserializeCard(x));

                        property.SetValue(gameStateOptions, cards);
                        break;
                    }

                    if (property.PropertyType.IsAssignableFrom(typeof(Player)))
                    {
                        var player = string.IsNullOrEmpty(value)
                            ? null
                            : PlayerService.DeserializePlayer(value);

                        property.SetValue(gameStateOptions, player);
                        break;
                    }
                }
            }

            return new GameState(gameStateOptions);
        }
    }
}
