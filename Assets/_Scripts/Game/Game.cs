using System.Collections;
using System.Collections.Generic;
using _Scripts.GameEntities;
using _Scripts.Generators;
using _Scripts.Handlers;
using _Scripts.Models;
using _Scripts.Services.EventsService;
using _Scripts.Services.RandomService;
using UnityEngine;
using Zenject;

namespace _Scripts.Game
{
    public class Game : MonoBehaviour, IGameHandler, ILoseHandler
    {
        private readonly List<PresetsGameModel> _presetsGame = new();
        private readonly List<ContainerGameEntity> _containerGameEntity = new();

        private PresetsGameModel _presetsGameModel;
        private LoseWindow _loseWindow;
        private Score _score;

        private IRandomService _randomService;
        private IGeneratorEntity _generatorEntity;
        private ILogEventsService _logEventsService;

        [Inject]
        private void Initialize(List<PresetsGameModel> presetsGameModel, IRandomService randomService,
            List<ContainerGameEntity> containerGameEntity, LoseWindow loseWindow,
            IGeneratorEntity generatorEntity, Score score, ILogEventsService logEventsService)
        {
            _logEventsService = logEventsService;
            _score = score;
            _generatorEntity = generatorEntity;
            _loseWindow = loseWindow;
            _randomService = randomService;
            _presetsGame.AddRange(presetsGameModel); 
            _containerGameEntity.AddRange(containerGameEntity);

            StartNewGame();
        }
        
        public void CompleteLose() => 
            RestartGame();

        private void RestartGame() => 
            StartNewGame();

        public void ActivateLoseWindow() => 
            _loseWindow.EnableView();

        public void CreateNewGameEntityAfterUsedPrevious() => 
            GenerateMainEntity();

        private void LogEvents(string message) => 
            _logEventsService.LogEvents(message);

        private void StartNewGame()
        {
            LogEvents("Start_new_game");
            ScoreRestart();
            var presetGameModel = RandomPresetGameModel(_presetsGame);
            var presetContainerPosition = RandomPresetContainerGameEntityWithPositions(_containerGameEntity);
            GenerateEntity(presetGameModel, presetContainerPosition);
            GenerateMainEntity();
        }

        private void ScoreRestart() => 
            _score.RestartScore();

        private void GenerateMainEntity() => 
            _generatorEntity.GenerateMainEntity();

        private void GenerateEntity(PresetsGameModel presetsGameModel, ContainerGameEntity presetContainerPosition) => 
            _generatorEntity.GenerateEntity(presetsGameModel, presetContainerPosition);

        private PresetsGameModel RandomPresetGameModel(ICollection presetsGameModels) => 
            _presetsGame[GetRandomValue(0, presetsGameModels.Count)];

        private ContainerGameEntity RandomPresetContainerGameEntityWithPositions(ICollection containerGameEntities) => 
            _containerGameEntity[GetRandomValue(0, containerGameEntities.Count)];

        private int GetRandomValue(int minValue, int maxValue) => 
            _randomService.Next(minValue, maxValue);
    }
}