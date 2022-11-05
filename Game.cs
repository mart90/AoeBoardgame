using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AoeBoardgame
{
    class Game
    {
        public GameState State { get; set; }
        public List<Player> Players { get; set; }
        public List<Move> MoveHistory { get; set; }

        private readonly Map _map;

        private TextNotification _textNotification;

        private Type _placingBuildingType;

        private readonly FontLibrary _fontLibrary;

        public Game(List<Player> players, Map map, FontLibrary fontLibrary)
        {
            Players = players;
            _map = map;
            _fontLibrary = fontLibrary;

            players[0].IsActive = true;
            State = GameState.MyTurn;

            MoveHistory = new List<Move>();
        }

        public Player ActivePlayer => Players.Single(e => e.IsActive);

        public Tile GetTileByLocation(Point location) => _map.GetTileByLocation(location);

        private PlaceableObject SelectedObject => _map.SelectedTile?.Object;

        public void StartTurn()
        {
            SetFogOfWar();

            UpdateQueues();

            if (ActivePlayer.IsPopulationRevolting)
            {
                HandleRevolt();
            }
        }

        public void EndTurn()
        {
            MoverTakeSteps();

            ActivePlayer.ResourcesGatheredLastTurnReset();
            GatherResources();
            ConsumeFood();

            // TODO queues

            PassTurnToNextPlayer();
        }

        private void GatherResources()
        {
            foreach (ICanGatherResources gatherer in ActivePlayer.OwnedObjects.FilterByType<ICanGatherResources>())
            {
                Resource? resource = gatherer.ResourceGathering;

                if (resource == null)
                {
                    continue;
                }

                int gatherRate = ActivePlayer.GatherRates.Single(e => e.Resource == resource).GatherRate;

                ActivePlayer.ResourceCollection.Single(e => e.Resource == resource).Amount += gatherRate;
                ActivePlayer.ResourcesGatheredLastTurn.Single(e => e.Resource == resource).Amount += gatherRate;
            }
        }

        private void ConsumeFood()
        {
            foreach (IConsumesFood consumer in ActivePlayer.OwnedObjects.FilterByType<IConsumesFood>())
            {
                ActivePlayer.ResourceCollection.Single(e => e.Resource == Resource.Food).Amount -= consumer.FoodConsumption;
                ActivePlayer.ResourcesGatheredLastTurn.Single(e => e.Resource == Resource.Food).Amount -= consumer.FoodConsumption;
            }
        }

        private void HandleRevolt()
        {
            // TODO
        }

        private void SetFogOfWar()
        {
            _map.ResetFogOfWar();

            foreach (PlayerObject obj in ActivePlayer.OwnedObjects)
            {
                List<Tile> tiles = new PathFinder(_map).GetAllTilesInRange(_map.GetTileByObject(obj), obj.LineOfSight).ToList();
                tiles.ForEach(e => e.HasFogOfWar = false);
            }
        }

        private void UpdateQueues()
        {
            int currentOwnedObjectsCount = ActivePlayer.OwnedObjects.Count();

            for (int i = 0; i < currentOwnedObjectsCount; i++)
            {
                if (ActivePlayer.OwnedObjects[i] is IHasQueue queuer && queuer.HasSomethingQueued())
                {
                    queuer.QueueTurnsLeft--;

                    if (queuer.QueueTurnsLeft == 0)
                    {
                        ResolveEndQueue(queuer);
                    }
                }
            }
        }

        private void ResolveEndQueue(IHasQueue queuer)
        {
            if (queuer is ICanMakeBuildings builder && builder.BuildingTypeQueued != null)
            {
                PlaceBuilding(builder.BuildingTypeQueued, builder.BuildingDestinationTile);

                builder.BuildingTypeQueued = null;
                builder.BuildingDestinationTile = null;
            }
            else if (queuer is ICanMakeUnits trainer && trainer.UnitTypeQueued != null)
            {
                // TODO
            }
            else if (queuer is ICanMakeResearch researcher && researcher.ResearchQueued != null)
            {
                // TODO
            }
        }

        private void PlaceBuilding(Type buildingType, Tile destinationTile)
        {
            destinationTile.BuildingUnderConstruction = false;

            PlayerObject building = ActivePlayer.AddAndGetPlaceableObject(buildingType);

            destinationTile.SetObject(building);
        }

        private void MoverTakeSteps()
        {
            foreach (ICanMove mover in ActivePlayer.OwnedObjects.FilterByType<ICanMove>())
            {
                if (mover.DestinationTile != null && !mover.HasSpentAllMovementPoints())
                {
                    Tile sourceTile = _map.GetTileByObject((PlayerObject)mover);
                    var path = _map.FindPath(sourceTile, mover.DestinationTile, mover);

                    if (path == null)
                    {
                        // TODO highlight unit
                        _textNotification = new TextNotification
                        {
                            Message = "The highlighted units' destination has become invalid. Their paths were reset",
                            FontColor = Color.Red
                        };
                        mover.DestinationTile = null;
                        continue;
                    }

                    Tile endTile = _map.ProgressOnPath(mover, path);

                    if (endTile == mover.DestinationTile)
                    {
                        ResolveReachedDestination(endTile, (PlayerObject)mover);
                    }
                }

                mover.StepsTakenThisTurn = 0;
            }
        }

        public Tile ProgressOnPath(ICanMove mover, Tile originTile, IEnumerable<Tile> path)
        {
            int steps = path.Count() >= mover.Speed ? mover.Speed : path.Count();
            mover.StepsTakenThisTurn += steps;

            Tile endTile = path.ToList()[-1 + steps];
            _map.MoveObject(originTile, endTile);

            return endTile;
        }

        private void ResolveReachedDestination(Tile endTile, PlayerObject mover)
        {
            if (endTile.Object is IEconomicBuilding building && mover is ICanGatherResources gatherer)
            {
                building.Gatherers.Add(gatherer);
                gatherer.ResourceGathering = building.Resource;
            }
        }

        public void LeftClickTileByLocation(Point location)
        {
            Tile tile = GetTileByLocation(location);
            if (tile == null)
            {
                return;
            }

            if (State == GameState.PlacingBuilding)
            {
                if (tile.TemporaryColor == TileColor.Teal) // Valid destination
                {
                    QueueBuilding(_placingBuildingType, (ICanMakeBuildings)SelectedObject, tile);
                }
                else
                {
                    ClearCurrentSelection();
                }
            }

            ClearCurrentSelection();

            tile.IsSelected = true;

            if (tile.Object is IHasRange objectWithRange)
            {
                IEnumerable<Tile> tilesInRange = _map.FindTilesInRangeOfSelected(objectWithRange.Range);

                if (tilesInRange != null)
                {
                    tilesInRange.Highlight(TileColor.Pink);
                }
            }

            if (!ActivePlayer.OwnedObjects.Contains(tile.Object))
            {
                tile.IsSelected = false;
                return;
            }

            if (tile.Object is ICanMove mover)
            {
                if (!(tile.Object is ICanMakeBuildings builder && builder.HasBuildingQueued()))
                {
                    State = GameState.MovingObject;

                    if (mover.DestinationTile != null)
                    {
                        var path = _map.FindPath(_map.GetTileByObject(tile.Object), mover.DestinationTile, mover);

                        if (path == null)
                        {
                            _textNotification = new TextNotification
                            {
                                Message = "The unit's destination has become invalid. Its path was reset",
                                FontColor = Color.Red
                            };
                            mover.DestinationTile = null;
                        }
                        else
                        {
                            path.Highlight(TileColor.Orange);
                        }
                    }
                }
            }

            if (tile.Object is ICanMakeBuildings builder2 && builder2.HasBuildingQueued())
            {
                _map.Tiles.Single(e => e == builder2.BuildingDestinationTile).SetTemporaryColor(TileColor.Orange);
            }
        }

        public void RightClickTileByLocation(Point location)
        {
            if (State != GameState.MovingObject)
            {
                return;
            }

            ICanMove mover = (ICanMove)SelectedObject;

            Tile sourceTile = _map.GetTileByObject(SelectedObject);
            Tile destinationTile = GetTileByLocation(location);

            if (destinationTile == sourceTile)
            {
                mover.DestinationTile = null;
                return;
            }

            var path = _map.FindPath(sourceTile, destinationTile, mover);

            if (path == null)
            {
                return;
            }

            mover.DestinationTile = destinationTile;

            path.Highlight(TileColor.Orange);

            if (!mover.HasSpentAllMovementPoints())
            {
                Tile endTile = _map.ProgressOnPath(mover, path);
                SetFogOfWar();

                if (endTile == mover.DestinationTile)
                {
                    ResolveReachedDestination(endTile, (PlayerObject)mover);
                }
            }

            MoveHistory.Add(new Move
            {
                PlayerName = ActivePlayer.Name,
                IsMovement = true,
                SourceTileId = sourceTile.Id,
                DestinationTileId = destinationTile.Id
            });
        }

        public void QueueBuilding(Type buildingType, ICanMakeBuildings builder, Tile tile)
        {
            var factory = ActivePlayer.GetFactoryByObjectType(buildingType);

            ActivePlayer.PayCost(factory.Cost);

            builder.QueueTurnsLeft = factory.TurnsToComplete;
            builder.BuildingTypeQueued = buildingType;
            builder.BuildingDestinationTile = tile;

            tile.BuildingUnderConstruction = true;

            State = GameState.MyTurn;
        }

        public void HoverOverTileByLocation(Point location)
        {
            ClearCurrentHover();

            var tile = GetTileByLocation(location);
            if (tile != null)
            {
                tile.IsHovered = true;
                HandleHover();
            }
        }

        public void HandleHover()
        {
            var selectedObject = _map.SelectedTile?.Object;

            if (selectedObject == null)
            {
                return;
            }

            if (State == GameState.MovingObject && selectedObject is ICanMove mover)
            {
                ClearTemporaryTileColors();

                if (mover.DestinationTile != null)
                {
                    var path = _map.FindPath(_map.GetTileByObject(selectedObject), mover.DestinationTile, mover);

                    if (path == null)
                    {
                        _textNotification = new TextNotification
                        {
                            Message = "The unit's destination has become invalid. Its path was reset",
                            FontColor = Color.Red
                        };
                        mover.DestinationTile = null;
                    }
                    else
                    {
                        path.Highlight(TileColor.Orange);
                    }
                }

                if (_map.HoveredTile == _map.SelectedTile)
                {
                    return;
                }

                IEnumerable <Tile> pathFromSelectedToHovered = _map.FindPathFromSelectedToHovered(mover);

                if (pathFromSelectedToHovered != null)
                {
                    pathFromSelectedToHovered.Highlight(TileColor.Teal);
                }
            }
        }

        public void PlaceStartingTownCenters()
        {
            // For now (maybe ever) assume 2 players
            for (int i = 0; i < 2; i++)
            {
                var player = Players[i];
                var tc = player.AddAndGetPlaceableObject<TownCenter>();

                int tileRow = _map.Height / 2;
                int tileColumn = i == 0 ? _map.Width / 5 : _map.Width - _map.Width / 5 - 1;
                var tile = _map.Tiles[tileRow * _map.Width + tileColumn];

                tile.SetObject(tc);
            }

            _map.Tiles[360].SetObject(Players[0].AddAndGetPlaceableObject<Villager>());
            _map.Tiles[120].SetObject(Players[1].AddAndGetPlaceableObject<Villager>());

            var builder = (ICanMakeBuildings) Players[0].OwnedObjects.Find(e => e is ICanMakeBuildings);

            if (_map.Tiles[361].SeemsAccessible)
            {
                Players[0].MakeBuilding<Tower>(builder, _map.Tiles[361]);
            }
        }

        public void ClearCurrentSelection()
        {
            var selectedTile = _map.SelectedTile;
            if (selectedTile != null)
            {
                selectedTile.IsSelected = false;
            }

            ClearTemporaryTileColors();

            State = GameState.MyTurn;

            _placingBuildingType = null;
            _textNotification = null;
        }

        public void ClearCurrentHover()
        {
            var hoveredTile = _map.HoveredTile;
            if (hoveredTile != null)
            {
                hoveredTile.IsHovered = false;
            }

            if (State == GameState.MovingObject && SelectedObject is ICanMove)
            {
                ClearTemporaryTileColors();
            }
        }

        private void ClearTemporaryTileColors()
        {
            foreach (var tile in _map.Tiles)
            {
                tile.SetTemporaryColor(TileColor.Default);
            }
        }

        private void PassTurnToNextPlayer()
        {
            var activePlayerId = Players.IndexOf(ActivePlayer);

            ActivePlayer.IsActive = false;

            if (activePlayerId == Players.Count - 1)
            {
                Players[0].IsActive = true;
            }
            else
            {
                Players[activePlayerId + 1].IsActive = true;
            }

            StartTurn();
        }

        private void ShowValidBuildingDestinations(Type buildingType)
        {
            ClearTemporaryTileColors();
            _textNotification = null;

            PlaceableObjectFactory factory = ActivePlayer.GetFactoryByObjectType(buildingType);

            if (!ActivePlayer.CanAfford(factory.Cost))
            {
                _textNotification = new TextNotification
                {
                    Message = "Not enough resources",
                    FontColor = Color.Red
                };
                return;
            }

            Tile originTile = _map.GetTileByObject(SelectedObject);

            IEnumerable<Tile> adjacentTiles = new PathFinder(_map).GetAdjacentTiles(originTile);
            var validPlacementTiles = new List<Tile>();

            if (buildingType == typeof(LumberCamp))
            {
                validPlacementTiles = adjacentTiles
                    .Where(e => e.Type == TileType.Forest && e.Object == null)
                    .ToList();
            }
            else if (buildingType == typeof(Mine))
            {
                var mineTypes = new List<TileType>
                {
                    TileType.GoldMine,
                    TileType.IronMine,
                    TileType.StoneMine
                };

                validPlacementTiles = adjacentTiles
                    .Where(e => mineTypes.Contains(e.Type) && e.Object == null)
                    .ToList();
            }
            else
            {
                validPlacementTiles = adjacentTiles
                    .Where(e => e.Type == TileType.Dirt && e.Object == null)
                    .ToList();
            }

            if (validPlacementTiles.Count() == 0)
            {
                _textNotification = new TextNotification
                {
                    Message = "No valid destination tiles for this building",
                    FontColor = Color.Red
                };
            }

            State = GameState.PlacingBuilding;
            _placingBuildingType = buildingType;
            validPlacementTiles.ForEach(e => e.SetTemporaryColor(TileColor.Teal));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _map.Draw(spriteBatch);

            if (_textNotification != null)
            {
                spriteBatch.DrawString(_fontLibrary.DefaultFontBold, _textNotification.Message, new Vector2(10, _map.Height * 45 + 5), _textNotification.FontColor);
            }

            ImGui.Begin("UI");
            ImGui.SetWindowSize(new System.Numerics.Vector2(500, 1060));
            ImGui.SetWindowPos(new System.Numerics.Vector2(1480, -20));

            DrawEconomy();
            DrawActionsTab();

            if (ImGui.Button("End turn", new System.Numerics.Vector2(200, 40)))
            {
                MoveHistory.Add(new Move { IsEndOfTurn = true });
                EndTurn();
            }

            ImGui.End();
        }

        private void DrawEconomy()
        {
            IEnumerable<ResourceCollection> resources = ActivePlayer.ResourceCollection;
            IEnumerable<ResourceCollection> resourcesGathered = ActivePlayer.ResourcesGatheredLastTurn;

            ImGui.BeginChild("Resources", new System.Numerics.Vector2(500, 180));
            ImGui.SetWindowFontScale(2f);

            int foodCount = resources.Single(e => e.Resource == Resource.Food).Amount;
            int foodGathered = resourcesGathered.Single(e => e.Resource == Resource.Food).Amount;
            DrawResourceLine("Food", foodCount, foodGathered, new System.Numerics.Vector4(1, 0, 0, 1));

            int woodCount = resources.Single(e => e.Resource == Resource.Wood).Amount;
            int woodGathered = resourcesGathered.Single(e => e.Resource == Resource.Wood).Amount;
            DrawResourceLine("Wood", woodCount, woodGathered, new System.Numerics.Vector4(.4f, .2f, .2f, 1));

            int goldCount = resources.Single(e => e.Resource == Resource.Gold).Amount;
            int goldGathered = resourcesGathered.Single(e => e.Resource == Resource.Gold).Amount;
            DrawResourceLine("Gold", goldCount, goldGathered, new System.Numerics.Vector4(1, 1, 0, 1));

            int ironCount = resources.Single(e => e.Resource == Resource.Iron).Amount;
            int ironGathered = resourcesGathered.Single(e => e.Resource == Resource.Iron).Amount;
            DrawResourceLine("Iron", ironCount, ironGathered, new System.Numerics.Vector4(0, 0, 1, 1));

            int stoneCount = resources.Single(e => e.Resource == Resource.Stone).Amount;
            int stoneGathered = resourcesGathered.Single(e => e.Resource == Resource.Stone).Amount;
            DrawResourceLine("Stone", stoneCount, stoneGathered, new System.Numerics.Vector4(.5f, .5f, .5f, 1));

            ImGui.EndChild();
        }

        private void DrawResourceLine(string resourceName, int resourceCount, int gatheredLastTurn, System.Numerics.Vector4 colorVector)
        {
            string text = "";
            for (int i = resourceCount.ToString().Length; i < 4; i++)
            {
                text += " ";
            }
            ImGui.Text(text);
            ImGui.SameLine();

            ImGui.TextColored(colorVector, resourceCount.ToString());
            ImGui.SameLine();

            text = "(";
            if (gatheredLastTurn >= 0)
            {
                text += "+";
            }
            text += gatheredLastTurn.ToString() + ")";
            ImGui.TextColored(colorVector, text);
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Gained or lost last turn");
            }
            ImGui.SameLine();

            text = "";
            for (int i = gatheredLastTurn < 0 ? gatheredLastTurn.ToString().Length - 1 : gatheredLastTurn.ToString().Length; i < 2; i++)
            {
                text += " ";
            }
            ImGui.Text(text);
            ImGui.SameLine();

            ImGui.TextColored(colorVector, resourceName);
        }

        private void DrawActionsTab()
        {
            var defaultButtonSize = new System.Numerics.Vector2(80, 40);
            ImGui.BeginChild("Actions", new System.Numerics.Vector2(500, 300));

            if (SelectedObject is IHasQueue queuer && queuer.HasSomethingQueued())
            {
                if (SelectedObject is ICanMakeBuildings builder2 && builder2.HasBuildingQueued())
                {
                    string buildingName = ActivePlayer.GetFactoryByObjectType(builder2.BuildingTypeQueued).UiName;
                    string str = $"Building a {buildingName}";
                }
            }

            if (SelectedObject is ICanMakeBuildings builder)
            {
                ImGui.Text("Build");

                int i = 1;
                foreach (Type buildingType in builder.BuildingTypesAllowedToMake)
                {
                    PlaceableObjectFactory factory = ActivePlayer.GetFactoryByObjectType(buildingType);

                    var buttonSize = defaultButtonSize;

                    if (factory.UiName.Length > 9)
                    {
                        buttonSize.X += 4 * (factory.UiName.Length - 9);
                    }

                    if (ImGui.Button(factory.UiName, buttonSize))
                    {
                        ShowValidBuildingDestinations(buildingType);
                    }

                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip(factory.TooltipString());
                    }

                    if (i % 4 != 0)
                    {
                        ImGui.SameLine();
                    }
                    
                    i++;
                }
            }
            if (SelectedObject is ICanMakeUnits)
            {

            }
            if (SelectedObject is ICanMakeResearch)
            {

            }

            ImGui.EndChild();
        }
    }
}
