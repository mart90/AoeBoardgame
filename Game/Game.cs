using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoeBoardgame
{
    /// <summary>
    /// Contains most of the game engine logic<br/>
    /// Inherited by Sandbox (single player) and MultiplayerGame
    /// </summary>
    abstract class Game : IUiWindow
    {
        public int WidthPixels { get; set; }
        public int HeightPixels { get; set; }

        public UiState CorrespondingUiState { get; set; }
        public UiState? NewUiState { get; set; }

        /// <summary>
        /// State of the on-map UI, barely used
        /// </summary>
        // TODO remove or expand to include all UI states
        public GameState State { get; set; }
        
        public List<Player> Players { get; set; }
        public List<GameMove> MoveHistory { get; set; }

        protected bool IsEnded { get; set; }
        protected string Result { get; set; }

        protected Map Map { get; set; }
        protected Popup Popup { get; set; }

        /// <summary>
        /// Appears below the map. Not rendered by ImGui
        /// </summary>
        private TextNotification _textNotification;

        /// <summary>
        /// Relevant when GameState = PlacingBuilding
        /// </summary>
        private Type _placingBuildingType;

        private readonly TextureLibrary _textureLibrary;
        private readonly FontLibrary _fontLibrary;
        protected readonly ResearchLibrary ResearchLibrary;
        protected readonly SoundEffectLibrary SoundEffectLibrary;

        public Game(
            TextureLibrary textureLibrary,
            FontLibrary fontLibrary,
            ResearchLibrary researchLibrary,
            SoundEffectLibrary soundEffectLibrary)
        {
            WidthPixels = 1920;
            HeightPixels = 1020;

            _fontLibrary = fontLibrary;
            _textureLibrary = textureLibrary;
            ResearchLibrary = researchLibrary;
            SoundEffectLibrary = soundEffectLibrary;

            MoveHistory = new List<GameMove>();
        }

        protected Player ActivePlayer => Players.Single(e => e.IsActive);
        
        protected Player InActivePlayer => Players.Single(e => !e.IsActive);

        /// <summary>
        /// Controls which player's economy is drawn in the top right
        /// </summary>
        protected virtual Player VisiblePlayer => ActivePlayer;
        
        protected bool IsMyTurn => ActivePlayer.IsLocalPlayer;

        private Tile GetTileByLocation(Point location) => Map.GetTileByLocation(location);

        /// <summary>
        /// For if we are viewing stats of an enemy unit/building
        /// </summary>
        private PlaceableObject ViewedObject => Map.ViewedTile?.Object;

        private PlaceableObject SelectedObject
        {
            get
            {
                PlaceableObject obj = Map.SelectedTile?.Object;

                if (obj == null)
                {
                    return null;
                }

                if (obj is IContainsUnits unitContainer)
                {
                    ICanFormGroup subSelectedUnit = unitContainer.SubSelectedUnit();
                    return (PlaceableObject)subSelectedUnit ?? (PlaceableObject)unitContainer;
                }

                else return obj;
            }
        }

        protected virtual void EndTurn()
        {
            ClearCurrentSelection();

            MoversTakeSteps();

            // TODO remove, shouldn't be needed
            DestroyEmptyArmies();

            ActivePlayer.ResetMovement();
            ActivePlayer.ResetResourcesGatheredLastTurn();

            GatherResources();
            ConsumeGold();

            AddMove(new GameMove
            {
                PlayerName = ActivePlayer.Name,
                IsEndOfTurn = true
            });

            PassTurnToNextPlayer();
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

        public virtual void StartTurn()
        {
            UpdateWonderTimer();

            UpdateQueues();

            if (ActivePlayer.IsPopulationRevolting)
            {
                HandleRevolt();
            }
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

                ActivePlayer.ResourceStockpile.Single(e => e.Resource == resource).Amount += gatherRate;
                ActivePlayer.ResourcesGatheredLastTurn.Single(e => e.Resource == resource).Amount += gatherRate;
            }
        }

        private void DestroyEmptyArmies()
        {
            IEnumerable<PlayerObject> allArmies = Players
                .SelectMany(e => e.OwnedObjects)
                .Where(e => e is Army);

            foreach (Army army in allArmies)
            {
                if (!army.Units.Any())
                {
                    army.Owner.OwnedObjects.Remove(army);
                }
            }
        }

        private void UpdateWonderTimer()
        {
            if (ActivePlayer.WonderTimer != null)
            {
                ActivePlayer.WonderTimer--;

                if (ActivePlayer.WonderTimer == 0)
                {
                    Result = ActivePlayer.Color == TileColor.Blue ? "b+w" : "r+w";
                    EndGame();
                }
            }
        }

        /// <summary>
        /// All military units consume 1 gold per turn
        /// </summary>
        private void ConsumeGold()
        {
            foreach (IConsumesGold consumer in ActivePlayer.OwnedObjects.FilterByType<IConsumesGold>())
            {
                ActivePlayer.ResourceStockpile.Single(e => e.Resource == Resource.Gold).Amount -= consumer.GoldConsumption;
                ActivePlayer.ResourcesGatheredLastTurn.Single(e => e.Resource == Resource.Gold).Amount -= consumer.GoldConsumption;
            }
        }

        private void HandleRevolt()
        {
            // TODO
        }

        protected void SetFogOfWar(Player player)
        {
            if (IsEnded)
            {
                return;
            }

            Map.ResetFogOfWar();

            foreach (Tile tile in player.VisibleTiles)
            {
                tile.HasFogOfWar = false;
            }
        }

        /// <summary>
        /// Sets VisibleTiles and RangeableTiles (if applicable) on the object
        /// </summary>
        private void UpdateVisibleAndRangeableTilesForObject(PlayerObject obj)
        {
            obj.VisibleTiles.Clear();

            // Skip for objects that are gathering or part of an army
            if (!Map.Tiles.Any(e => e.Object == obj))
            {
                return;
            }

            Tile originTile = Map.GetTileContainingObject(obj);

            List<Tile> tiles = new PathFinder(Map).GetAllTilesInRange(originTile, obj.LineOfSight).ToList();
            tiles.Add(originTile);

            foreach (Tile tile in tiles)
            {
                obj.VisibleTiles.Add(tile);

                if (tile.Object != null && tile.Object is PlayerObject playerObject && playerObject.Owner != ActivePlayer)
                {
                    tile.IsScouted = true;
                }
            }

            if (obj is IHasRange ranger)
            {
                ranger.RangeableTiles = Map.FindTilesInRangeOfTile(originTile, ranger.Range, ranger.HasMinimumRange);
            }

            SetFogOfWar(ActivePlayer);
        }

        private void UpdateQueues()
        {
            int currentOwnedObjectsCount = ActivePlayer.OwnedObjects.Count();

            // Not foreach because we may be adding owned objects during the loop
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
                CreateBuilding(builder.BuildingTypeQueued, builder.BuildingDestinationTile);

                builder.StopConstruction();
            }
            else if (queuer is ICanMakeUnits trainer && trainer.UnitTypeQueued != null)
            {
                CreateUnit(trainer.UnitTypeQueued, trainer);

                trainer.UnitTypeQueued = null;
            }
            else if (queuer is ICanMakeResearch researcher && researcher.ResearchQueued != null)
            {
                // Effects are in ResearchLibrary
                researcher.ResearchQueued.Effect(ActivePlayer);
                
                researcher.ResearchQueued = null;
            }
        }

        private void CreateBuilding(Type buildingType, Tile destinationTile)
        {
            PlayerObject building = ActivePlayer.AddAndGetPlaceableObject(buildingType);

            if (building is Mine mine)
            {
                mine.Resource = destinationTile.Type == TileType.GoldMine ? Resource.Gold :
                    destinationTile.Type == TileType.StoneMine ? Resource.Stone : Resource.Iron;
            }

            destinationTile.SetObject(building);

            UpdateVisibleAndRangeableTilesForObject(building);

            if (building is Wonder)
            {
                ActivePlayer.WonderTimer = 25;

                // Reveals the tile for the opponent (in fog of war)
                destinationTile.IsScouted = true;

                if (!IsMyTurn)
                {
                    Popup = new Popup
                    {
                        Message = "Your opponent built a wonder. If you don't destroy it within 25 turns, they will win the game.",
                        IsInformational = true
                    };
                }
            }
        }

        private void CreateUnit(Type unitType, ICanMakeUnits trainer)
        {
            PlayerObject unit = ActivePlayer.AddAndGetPlaceableObject(unitType);
            ICanMove mover = (ICanMove)unit;

            Tile originTile = Map.GetTileContainingObject((PlayerObject)trainer);
            Tile destinationTile = trainer.WayPoint;

            if (destinationTile != null)
            {
                IEnumerable<Tile> path = Map.FindPath(originTile, destinationTile, mover);

                if (path == null) // There's no path to the waypoint (usually because there's another object on it)
                {
                    // TODO improve user-friendliness
                    _textNotification = new TextNotification
                    {
                        FontColor = Color.Red,
                        Message = "A waypoint became invalid"
                    };

                    destinationTile = null;
                    trainer.WayPoint = null;
                }
                else if (path.First().IsEmpty)
                {
                    Tile firstTile = path.First();
                    firstTile.SetObject((PlayerObject)mover);

                    if (firstTile != destinationTile)
                    {
                        mover.DestinationTile = destinationTile;
                    }
                }
                else if (!unit.CanMergeWith(path.First().Object)) // TODO (Why) is this needed?
                {
                    _textNotification = new TextNotification
                    {
                        FontColor = Color.Red,
                        Message = "A waypoint became invalid"
                    };

                    destinationTile = null;
                    trainer.WayPoint = null;
                }
                else // There's a unit on the first tile in the path, and we can merge with it
                {
                    MergeMoverWithDestination(originTile, destinationTile, mover);
                }
            }

            if (destinationTile == null)
            {
                PlaceObjectOnAdjacentTile(unit, originTile);
            }

            UpdateVisibleAndRangeableTilesForObject(unit);
        }

        /// <summary>
        /// Searches for an available adjacent tile starting NorthEast and going clockwise
        /// </summary>
        private Tile PlaceObjectOnAdjacentTile(PlayerObject unit, Tile originTile)
        {
            Tile destinationTile;
            var pathFinder = new PathFinder(Map);
            IEnumerable<Tile> adjacentTiles = pathFinder.GetAdjacentTiles(originTile);

            destinationTile = adjacentTiles
                .Where(e => e.IsEmpty || (e.Object != null && unit.CanMergeWith(e.Object)))
                .FirstOrDefault();

            if (destinationTile == null)
            {
                _textNotification = new TextNotification
                {
                    FontColor = Color.Red,
                    Message = "A created unit had no possible tiles to spawn on"
                };

                // TODO money back?

                ActivePlayer.OwnedObjects.Remove(unit);
            }
            else if (destinationTile.IsEmpty)
            {
                destinationTile.SetObject(unit);
            }
            else
            {
                MergeMoverWithDestination(originTile, destinationTile, (ICanMove)unit);
            }

            return destinationTile;
        }

        /// <summary>
        /// Moves units towards their destinations if they still have steps left
        /// </summary>
        private void MoversTakeSteps()
        {
            List<ICanMove> movers = ActivePlayer.OwnedObjects.FilterByType<ICanMove>().ToList();

            foreach (ICanMove mover in movers)
            {
                if (mover.DestinationTile != null && !mover.HasSpentAllMovementPoints())
                {
                    Tile sourceTile = Map.GetTileContainingObject((PlayerObject)mover);
                    var path = Map.FindPath(sourceTile, mover.DestinationTile, mover);

                    if (path == null)
                    {
                        if (IsMyTurn)
                        {
                            // TODO highlight unit
                            _textNotification = new TextNotification
                            {
                                Message = "Some units' destination has become invalid. Their paths were reset",
                                FontColor = Color.Red
                            };
                        }
                        mover.DestinationTile = null;
                        continue;
                    }

                    ProgressOnPath(mover, path);
                    UpdateVisibleAndRangeableTilesForObject((PlayerObject)mover);
                }
            }
        }

        protected void PlaceStartingUnits()
        {
            Map.Tiles[255].SetObject(Players[0].AddAndGetPlaceableObject(typeof(TownCenter)));
            Map.Tiles[270].SetObject(Players[1].AddAndGetPlaceableObject(typeof(TownCenter)));

            Map.Tiles[254].SetObject(Players[0].AddAndGetPlaceableObject(typeof(Villager)));
            Map.Tiles[256].SetObject(Players[0].AddAndGetPlaceableObject(typeof(Villager)));
            Map.Tiles[280].SetObject(Players[0].AddAndGetPlaceableObject(typeof(Villager)));

            Map.Tiles[269].SetObject(Players[1].AddAndGetPlaceableObject(typeof(Villager)));
            Map.Tiles[271].SetObject(Players[1].AddAndGetPlaceableObject(typeof(Villager)));
            Map.Tiles[294].SetObject(Players[1].AddAndGetPlaceableObject(typeof(Villager)));

            Map.Tiles[230].SetObject(Players[0].AddAndGetPlaceableObject(typeof(Scout)));
            Map.Tiles[244].SetObject(Players[1].AddAndGetPlaceableObject(typeof(Scout)));

            IEnumerable<PlayerObject> startingUnits = Players.SelectMany(e => e.OwnedObjects);

            foreach (Scout scout in startingUnits.Where(e => e is Scout))
            {
                scout.AttackDamage = 0;
            }

            foreach (PlayerObject obj in startingUnits)
            {
                UpdateVisibleAndRangeableTilesForObject(obj);
            }

            // Bandaid to fix the issue where enemy tiles were set to IsScouted=true for the ActivePlayer by the above method call
            Map.Tiles[255].IsScouted = false;
            Map.Tiles[270].IsScouted = false;
            Map.Tiles[254].IsScouted = false;
            Map.Tiles[256].IsScouted = false;
            Map.Tiles[280].IsScouted = false;
            Map.Tiles[269].IsScouted = false;
            Map.Tiles[271].IsScouted = false;
            Map.Tiles[294].IsScouted = false;
            Map.Tiles[230].IsScouted = false;
            Map.Tiles[244].IsScouted = false;
        }

        /// <summary>
        /// Called when the user left clicks somewhere on the map
        /// </summary>
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

            if (tile.HasFogOfWar)
            {
                return;
            }

            if (!IsMyTurn || !ActivePlayer.OwnedObjects.Contains(tile.Object))
            {
                tile.IsViewed = true;
                return;
            }

            tile.IsSelected = true;

            if (tile.Object is ICanMakeUnits trainer && trainer.WayPoint != null)
            {
                trainer.WayPoint.SetTemporaryColor(TileColor.Orange);
            }

            if (tile.Object is ICanMove mover)
            {
                if (!(tile.Object is ICanMakeBuildings builder && builder.HasBuildingQueued()))
                {
                    State = GameState.MovingObject;

                    if (mover.DestinationTile != null)
                    {
                        var path = Map.FindPath(Map.GetTileContainingObject(tile.Object), mover.DestinationTile, mover);

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
                            path.SetTemporaryColor(TileColor.Orange);
                        }
                    }
                }
            }

            if (tile.Object is ICanMakeBuildings builder2 && builder2.HasBuildingQueued())
            {
                Map.Tiles.Single(e => e == builder2.BuildingDestinationTile).SetTemporaryColor(TileColor.Orange);
            }
        }

        /// <summary>
        /// Called when the user right clicks somewhere on the map
        /// </summary>
        public void RightClickTileByLocation(Point location)
        {
            if (SelectedObject == null)
            {
                return;
            }

            Tile destinationTile = GetTileByLocation(location);

            if (destinationTile == null)
            {
                return;
            }

            Tile originTile = Map.GetTileContainingObject(SelectedObject);

            if (!destinationTile.HasFogOfWar && destinationTile.Object != null && ((PlayerObject)SelectedObject).CanAttack(destinationTile.Object))
            {
                TryAttackObject(originTile, destinationTile, (IAttacker)SelectedObject, (IAttackable)destinationTile.Object);
                return;
            }

            if (SelectedObject is ICanMakeUnits trainer && destinationTile.SeemsAccessible)
            {
                SetWaypoint(trainer, originTile, destinationTile);
            }

            // TODO is this redundant?
            if (State != GameState.MovingObject)
            {
                return;
            }

            ICanMove mover = (ICanMove)SelectedObject;

            if (mover is ICanFormGroup && originTile.Object is IContainsUnits && mover.HasSpentAllMovementPoints())
            {
                return;
            }

            if (destinationTile == originTile)
            {
                mover.DestinationTile = null;
                return;
            }

            TryMoveObject(originTile, destinationTile, mover);
        }

        /// <summary>
        /// Sets the destination tile of units spawned from the given trainer (building)
        /// </summary>
        protected virtual void SetWaypoint(ICanMakeUnits trainer, Tile originTile, Tile destinationTile)
        {
            trainer.WayPoint = destinationTile;
            destinationTile.SetTemporaryColor(TileColor.Orange);

            AddMove(new GameMove
            {
                PlayerName = ActivePlayer.Name,
                OriginTileId = originTile.Id,
                DestinationTileId = destinationTile.Id,
                IsWaypoint = true
            });
        }

        /// <summary>
        /// Called when the user selects a moving unit and right clicks on an empty tile
        /// </summary>
        protected virtual void TryMoveObject(Tile originTile, Tile destinationTile, ICanMove mover)
        {
            var path = Map.FindPath(originTile, destinationTile, mover);

            if (path == null)
            {
                return;
            }

            if (mover.HasSpentAllMovementPoints() && originTile.Object != mover)
            {
                // This means we are trying to move a unit out of a group
                // To prevent unexpected behavior, only allow this if they have movement points left (don't set a destination that they will automatically move towards next turn)
                return;
            }

            int? moverHp = null;

            if (originTile.Object != mover)
            {
                // Unit is moving out of a group. Set its HP to identify it so the other client or replay viewer will know which unit to move.
                // (All other unit stats such as attack damage, range etc should be identical within a group)
                moverHp = ((PlayerObject)mover).HitPoints;
            }

            mover.DestinationTile = destinationTile;

            path.SetTemporaryColor(TileColor.Orange);

            if (!mover.HasSpentAllMovementPoints())
            {
                ProgressOnPath(mover, path);
                UpdateVisibleAndRangeableTilesForObject((PlayerObject)mover);
            }

            AddMove(new GameMove
            {
                PlayerName = ActivePlayer.Name,
                IsMovement = true,
                OriginTileId = originTile.Id,
                DestinationTileId = destinationTile.Id,
                SubselectedUnitHitpoints = moverHp
            });
        }

        /// <summary>
        /// Called when the user selects an attacking object and right clicks a tile with an enemy or neutral unit
        /// </summary>
        protected virtual void TryAttackObject(Tile originTile, Tile destinationTile, IAttacker attacker, IAttackable defender)
        {
            if (attacker.HasAttackedThisTurn)
            {
                _textNotification = new TextNotification
                {
                    FontColor = Color.Red,
                    Message = "Unit either already attacked this turn or spent all their movement points"
                };
                return;
            }

            if (attacker is Trebuchet && defender is ICanMove)
            {
                _textNotification = new TextNotification
                {
                    FontColor = Color.Red,
                    Message = "This unit can only attack buildings"
                };
                return;
            }

            int range;
            int damage = attacker.AttackDamage;
            bool boarFoughtBack = false;

            if (attacker is ICanMove mover)
            {
                mover.DestinationTile = null;
            }

            if ((attacker is Pikeman || attacker is Army army && army.Units[0] is Pikeman)
                && (defender is ICavalry || (defender is Army army2 && army2.Units[0] is ICavalry)))
            {
                damage *= 3;
            }

            if (attacker is IHasRange ranger)
            {
                range = ranger.Range;
                var armor = defender.RangedArmor - attacker.ArmorPierce;
                damage -= (armor > 0 ? armor : 0);
            }
            else
            {
                range = 1;
                var armor = defender.MeleeArmor - attacker.ArmorPierce;
                damage -= (armor > 0 ? armor : 0);
            }

            // TODO might be redundant, we should already know which tiles we can range
            IEnumerable<Tile> tilesInRange = Map.FindTilesInRangeOfTile(originTile, range, attacker is IHasRange ranger2 && ranger2.HasMinimumRange);

            if (!tilesInRange.Contains(destinationTile))
            {
                _textNotification = new TextNotification
                {
                    FontColor = Color.Red,
                    Message = "Object is out of range"
                };
                return;
            }

            if (damage < 0)
            {
                damage = 0;
            }

            bool defenderWasArmy = defender is Army;

            bool defenderDied = defender.TakeDamage(damage, destinationTile);

            if (!defenderDied && defenderWasArmy && !(destinationTile.Object is Army))
            {
                // The army was disbanded but there is a survivor
                UpdateVisibleAndRangeableTilesForObject((PlayerObject)destinationTile.Object);
            }

            if (defender is Boar)
            {
                boarFoughtBack = BoarDefense(destinationTile, originTile);
            }
            else if (defender is Deer)
            {
                DeerDefense(destinationTile, originTile);
            }

            if (defenderDied)
            {
                HandleObjectKilled(defender, attacker, destinationTile);
            }

            attacker.HasAttackedThisTurn = true;

            if (attacker is ICanMove mover3)
            {
                // Spend all movement points
                mover3.StepsTakenThisTurn = mover3.Speed;
            }

            if (IsMyTurn || defender is PlayerObject)
            {
                _textNotification = new TextNotification
                {
                    FontColor = Color.Green,
                    Message = $"The attack did {damage} damage"
                };

                if (boarFoughtBack)
                {
                    _textNotification.Message += ", but the boar fought back";
                }
            }

            AddMove(new GameMove
            {
                PlayerName = ActivePlayer.Name,
                IsAttack = true,
                DestinationTileId = destinationTile.Id,
                OriginTileId = originTile.Id
            });
        }

        /// <summary>
        /// Military victory is when one player has nothing left (no buildings or units)
        /// </summary>
        private void CheckForMilitaryVictory()
        {
            foreach (Player player in Players)
            {
                if (!player.OwnedObjects.Any())
                {
                    Result = player.Color == TileColor.Blue ? "r+c" : "b+c";
                    EndGame();
                    return;
                }
            }
        }

        protected virtual void EndGame()
        {
            IsEnded = true;

            // Reveal the map
            Map.Tiles.ForEach(e => e.HasFogOfWar = false);
        }

        private void HandleObjectKilled(IAttackable defender, IAttacker attacker, Tile defenderTile)
        {
            if (defender is Villager vill && vill.HasBuildingQueued())
            {
                // Removes the hammer
                vill.BuildingDestinationTile.BuildingUnderConstruction = false;
            }

            if (defender is IEconomicBuilding economicBuilding && economicBuilding.Units.Any())
            {
                // An economic building with (a) unit(s) in it died

                if (economicBuilding.Units.Count > 1)
                {
                    GathererGroup newGroup = InActivePlayer.AddAndGetPlaceableObject<GathererGroup>();

                    foreach (ICanFormGroup grouper in economicBuilding.Units)
                    {
                        newGroup.Units.Add(grouper);
                    }

                    newGroup.SetTexture();

                    if (economicBuilding is Farm)
                    {
                        defenderTile.SetObject(newGroup);
                    }
                    else
                    {
                        PlaceObjectOnAdjacentTile(newGroup, defenderTile);
                        defenderTile.SetObject(null);
                    }

                    UpdateVisibleAndRangeableTilesForObject(newGroup);
                }
                else
                {
                    PlayerObject survivor = (PlayerObject)economicBuilding.Units[0];

                    if (economicBuilding is Farm)
                    {
                        defenderTile.SetObject(survivor);
                    }
                    else
                    {
                        PlaceObjectOnAdjacentTile(survivor, defenderTile);
                        defenderTile.SetObject(null);
                    }

                    UpdateVisibleAndRangeableTilesForObject(survivor);
                }
            }
            else
            {
                defenderTile.SetObject(null);

                if (!(attacker is IHasRange) && !(defender is Mine || defender is LumberCamp))
                {
                    Map.MoveMover((ICanMove)attacker, defenderTile);

                    UpdateVisibleAndRangeableTilesForObject((PlayerObject)attacker);
                }
            }

            if (defender is PlayerObject playerObject)
            {
                if (defender is Wonder)
                {
                    playerObject.Owner.WonderTimer = null;
                }

                playerObject.Owner.OwnedObjects.Remove(playerObject);
                
                CheckForMilitaryVictory();
            }
            else if (defender is GaiaObject && attacker is ICanMove)
            {
                if (defender is Deer)
                {
                    ActivePlayer.ResourceStockpile.Single(e => e.Resource == Resource.Food).Amount += 50;
                }
                else if (defender is Boar)
                {
                    ActivePlayer.ResourceStockpile.Single(e => e.Resource == Resource.Food).Amount += 200;
                }
            }
        }

        /// <summary>
        /// Called when user destroys their own building voluntarily
        /// </summary>
        protected void DestroyOwnBuilding(Tile buildingTile)
        {
            PlayerObject building = (PlayerObject)buildingTile.Object;

            buildingTile.SetObject(null);
            building.Owner.OwnedObjects.Remove(building);

            AddMove(new GameMove
            {
                PlayerName = ActivePlayer.Name,
                IsDestroyBuilding = true,
                OriginTileId = buildingTile.Id
            });
        }

        protected void CancelQueue(IHasQueue queuer)
        {
            if (queuer is ICanMakeBuildings builder)
            {
                PlaceableObjectFactory factory = ActivePlayer.GetFactoryByObjectType(builder.BuildingTypeQueued);

                builder.StopConstruction();

                // Reimburse 50% of the cost
                foreach (ResourceCollection cost in factory.Cost)
                {
                    ActivePlayer.ResourceStockpile.Single(e => e.Resource == cost.Resource).Amount += cost.Amount / 2;
                }
            }
            else if (queuer is ICanMakeUnits trainer && trainer.HasUnitQueued())
            {
                PlaceableObjectFactory factory = ActivePlayer.GetFactoryByObjectType(trainer.UnitTypeQueued);

                trainer.StopQueue();

                // Reimburse 100%
                // TODO it's possible to "create" resources this way if combined with techs that reduce cost
                foreach (ResourceCollection cost in factory.Cost)
                {
                    ActivePlayer.ResourceStockpile.Single(e => e.Resource == cost.Resource).Amount += cost.Amount;
                }
            }
            else if (queuer is ICanMakeResearch researcher && researcher.HasResearchQueued())
            {
                // Reimburse 100%
                // TODO it's possible to "create" resources this way if combined with techs that reduce cost
                foreach (ResourceCollection cost in researcher.ResearchQueued.Cost)
                {
                    ActivePlayer.ResourceStockpile.Single(e => e.Resource == cost.Resource).Amount += cost.Amount;
                }

                // When we queue research it gets disabled so that we can't queue it again in a similar building
                // When we cancel, we need to re-allow it or it will forever be disabled
                AllowResearch(researcher.GetType(), researcher.ResearchQueued.ResearchEnum);
                
                researcher.StopQueue();
            }

            AddMove(new GameMove
            {
                PlayerName = ActivePlayer.Name,
                IsCancel = true,
                OriginTileId = Map.GetTileContainingObject((PlayerObject)queuer).Id
            });
        }

        private bool BoarDefense(Tile boarTile, Tile attackerTile)
        {
            var pathFinder = new PathFinder(Map);
            Boar boar = (Boar)boarTile.Object;

            if (pathFinder.GetAdjacentTiles(boarTile).Contains(attackerTile))
            {
                // Fight back if the attacker is within reach

                var defender = (PlayerObject)attackerTile.Object;

                int damage = boar.AttackDamage - defender.MeleeArmor;

                bool defenderDied = defender.TakeDamage(damage, attackerTile);
                
                if (defenderDied)
                {
                    HandleObjectKilled(defender, boar, attackerTile);
                }

                return true;
            }
            else if (boar.HitPoints > 0)
            {
                Tile retreatTile = pathFinder.GetRetreatingTile(boarTile, attackerTile);

                if (retreatTile != null)
                {
                    Map.MoveObjectSimple(boarTile, retreatTile);
                }
            }

            return false;
        }

        private void DeerDefense(Tile deerTile, Tile attackerTile)
        {
            Deer deer = (Deer)deerTile.Object;

            if (deer.HitPoints <= 0)
            {
                return;
            }

            var pathFinder = new PathFinder(Map);
            
            Tile retreatTile = pathFinder.GetRetreatingTile(deerTile, attackerTile);

            if (retreatTile != null)
            {
                Map.MoveObjectSimple(deerTile, retreatTile);
            }
        }

        protected virtual void QueueBuilding(Type buildingType, ICanMakeBuildings builder, Tile tile)
        {
            var factory = ActivePlayer.GetFactoryByObjectType(buildingType);

            ActivePlayer.PayCost(factory.Cost);

            builder.QueueTurnsLeft = factory.TurnsToComplete;
            builder.BuildingTypeQueued = buildingType;
            builder.BuildingDestinationTile = tile;

            ((ICanMove)builder).DestinationTile = null;

            tile.BuildingUnderConstruction = true;

            State = GameState.Default;

            AddMove(new GameMove
            {
                PlayerName = ActivePlayer.Name,
                OriginTileId = Map.GetTileContainingObject((PlaceableObject)builder).Id,
                DestinationTileId = tile.Id,
                IsQueueBuilding = true,
                BuildingTypeName = buildingType.Name
            });
        }

        protected virtual void TryQueueUnit(Type unitType, ICanMakeUnits trainer)
        {
            var factory = ActivePlayer.GetFactoryByObjectType(unitType);

            if (!ActivePlayer.TryPayCost(factory.Cost))
            {
                _textNotification = new TextNotification
                {
                    FontColor = Color.Red,
                    Message = "Not enough resources"
                };
                return;
            }

            trainer.UnitTypeQueued = unitType;
            trainer.QueueTurnsLeft = factory.TurnsToComplete;

            AddMove(new GameMove
            {
                PlayerName = ActivePlayer.Name,
                OriginTileId = Map.GetTileContainingObject((PlaceableObject)trainer).Id,
                IsQueueUnit = true,
                UnitTypeName = unitType.Name
            });
        }

        protected virtual void TryResearch(Research research, ICanMakeResearch researcher)
        {
            if (!ActivePlayer.TryPayCost(research.Cost))
            {
                _textNotification = new TextNotification
                {
                    FontColor = Color.Red,
                    Message = "Not enough resources"
                };
                return;
            }

            researcher.ResearchQueued = research;
            researcher.QueueTurnsLeft = research.TurnsToComplete;

            DisallowResearch(researcher.GetType(), research.ResearchEnum);

            AddMove(new GameMove
            {
                PlayerName = ActivePlayer.Name,
                OriginTileId = Map.GetTileContainingObject((PlaceableObject)researcher).Id,
                IsQueueResearch = true,
                ResearchId = research.ResearchEnum
            });
        }

        /// <summary>
        /// Called when starting the research, so that we can't queue it again somewhere else
        /// </summary>
        private void DisallowResearch(Type researcherType, ResearchEnum researchEnum)
        {
            foreach (ICanMakeResearch researcher in ActivePlayer.OwnedObjects.Where(e => e.GetType() == researcherType))
            {
                researcher.ResearchAllowedToMake.Remove(researchEnum);
            }

            ICanMakeResearchFactory factory = (ICanMakeResearchFactory)ActivePlayer.GetFactoryByObjectType(researcherType);
            factory.ResearchAllowedToMake.Remove(researchEnum);
        }

        /// <summary>
        /// Called when cancelling research
        /// </summary>
        private void AllowResearch(Type researcherType, ResearchEnum researchEnum)
        {
            foreach (ICanMakeResearch researcher in ActivePlayer.OwnedObjects.Where(e => e.GetType() == researcherType))
            {
                researcher.ResearchAllowedToMake.Add(researchEnum);
            }

            ICanMakeResearchFactory factory = (ICanMakeResearchFactory)ActivePlayer.GetFactoryByObjectType(researcherType);
            factory.ResearchAllowedToMake.Add(researchEnum);
        }

        /// <summary>
        /// Called every frame, if the user is not currently clicking a mouse button
        /// </summary>
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

        private void HandleHover()
        {
            var selectedObject = SelectedObject;

            if (selectedObject == null)
            {
                return;
            }

            if (State != GameState.PlacingBuilding && !(selectedObject is ICanMakeUnits))
            {
                ClearTemporaryTileColorsExceptPink();
            }

            Tile originTile = Map.SelectedTile;

            if (selectedObject is IHasRange ranger 
                && Map.HoveredTile.Object != null 
                && (
                    (Map.HoveredTile.Object is PlayerObject playerObject && playerObject.Owner != ActivePlayer)
                    || Map.HoveredTile.Object is GaiaObject)
                )
            {
                if (ranger.RangeableTiles.Contains(Map.HoveredTile))
                {
                    Map.HoveredTile.SetTemporaryColor(TileColor.Teal);
                }
            }

            if (State == GameState.MovingObject && selectedObject is ICanMove mover)
            {
                if (mover.DestinationTile != null)
                {
                    // TODO do we have to do this every frame?
                    var path = Map.FindPath(originTile, mover.DestinationTile, mover);

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
                        path.SetTemporaryColor(TileColor.Orange);
                    }
                }

                if (Map.HoveredTile == Map.SelectedTile)
                {
                    return;
                }

                IEnumerable<Tile> pathFromSelectedToHovered = Map.FindPathFromSelectedToHovered(mover);

                if (pathFromSelectedToHovered != null)
                {
                    pathFromSelectedToHovered.SetTemporaryColor(TileColor.Teal);
                }
            }
        }

        public void ClearCurrentSelection()
        {
            var selectedTile = Map.SelectedTile;
            
            if (selectedTile != null)
            {
                selectedTile.IsSelected = false;
            }

            var viewedTile = Map.ViewedTile;

            if (viewedTile != null)
            {
                viewedTile.IsViewed = false;
            }

            foreach (var tile in Map.Tiles)
            {
                tile.SetTemporaryColor(TileColor.Default);
            }

            State = GameState.Default;

            _placingBuildingType = null;
            _textNotification = null;

            foreach (ICanFormGroup grouper in ActivePlayer.OwnedObjects.Where(e => e is ICanFormGroup))
            {
                grouper.IsSubSelected = false;
            }
        }

        public void ClearCurrentHover()
        {
            var hoveredTile = Map.HoveredTile;
            if (hoveredTile != null)
            {
                hoveredTile.IsHovered = false;
            }

            if (State == GameState.MovingObject && SelectedObject is ICanMove)
            {
                ClearTemporaryTileColorsExceptPink();
            }
        }

        /// <summary>
        /// Pink is the color shown when the user presses the "show range" button
        /// </summary>
        protected void ClearTemporaryTileColorsExceptPink()
        {
            foreach (var tile in Map.Tiles.Where(e => e.TemporaryColor != TileColor.Pink))
            {
                tile.SetTemporaryColor(TileColor.Default);
            }
        }

        private void ShowValidBuildingDestinations(Type buildingType)
        {
            ClearTemporaryTileColorsExceptPink();
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

            Tile originTile = Map.GetTileContainingObject(SelectedObject);

            IEnumerable<Tile> adjacentTiles = new PathFinder(Map).GetAdjacentTiles(originTile);
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

        private Tile ProgressOnPath(ICanMove mover, IEnumerable<Tile> path)
        {
            Tile originTile = Map.GetTileContainingObject((PlaceableObject)mover);

            int steps = path.Count() >= mover.Speed - mover.StepsTakenThisTurn ? mover.Speed - mover.StepsTakenThisTurn : path.Count();

            Tile endTile = path.ToList()[-1 + steps]; // Path does not contain origin tile

            bool destinationReached = path.Last() == endTile;

            if (endTile.HasFogOfWar && endTile.Object != null)
            {
                for (int i = 0; i < steps; i++)
                {
                    mover.StepsTakenThisTurn++;

                    Map.MoveMover(mover, path.ToList()[i]);
                    UpdateVisibleAndRangeableTilesForObject((PlayerObject)mover);

                    if (!endTile.HasFogOfWar)
                    {
                        HandleDestinationInvalid(mover);
                        return null;
                    }
                }
            }
            else
            {
                mover.StepsTakenThisTurn += steps;
            }

            if (mover is IAttacker attacker && mover.HasSpentAllMovementPoints())
            {
                attacker.HasAttackedThisTurn = true;
            }

            if (endTile.Object != null)
            {
                if (!destinationReached // No passing through armies or economic buildings (for now?)
                    || endTile.Object is GaiaObject
                    || (endTile.Object is PlayerObject playerObject && playerObject.Owner != ActivePlayer))
                {
                    HandleDestinationInvalid(mover);
                    return null;
                }

                if (((PlayerObject)mover).CanMergeWith(endTile.Object))
                {
                    MergeMoverWithDestination(originTile, endTile, mover);
                }
                else
                {
                    HandleDestinationInvalid(mover);
                    return null;
                }
            }
            else
            {
                Map.MoveMover(mover, endTile);
                UpdateVisibleAndRangeableTilesForObject((PlayerObject)mover);
            }

            if (destinationReached)
            {
                mover.DestinationTile = null;
                endTile.SetTemporaryColor(TileColor.Default);

                if (SelectedObject != mover)
                {
                    State = GameState.Default;
                }
            }

            return endTile;
        }

        private void HandleDestinationInvalid(ICanMove mover)
        {
            if (IsMyTurn)
            {
                _textNotification = new TextNotification
                {
                    Message = "Some units' destinations have become invalid. Their paths were reset",
                    FontColor = Color.Red
                };
            }

            mover.DestinationTile = null;
        }

        /// <summary>
        /// Does not check merge legality, that is up to the caller
        /// </summary>
        private void MergeMoverWithDestination(Tile originTile, Tile endTile, ICanMove mover)
        {
            if (endTile.Object is IEconomicBuilding economicBuilding)
            {
                if (mover is GathererGroup group)
                {
                    group.ResourceGathering = economicBuilding.Resource;
                    economicBuilding.Units.AddRange(group.Units);
                }
                else
                {
                    ((ICanGatherResources)mover).ResourceGathering = economicBuilding.Resource;
                    economicBuilding.Units.Add((ICanFormGroup)mover);
                }
            }
            else if (endTile.Object is Army army)
            {
                if (mover is ICanFormGroup)
                {
                    army.Units.Add((ICanFormGroup)mover);
                    army.StepsTakenThisTurn = army.Units.Max(e => e.StepsTakenThisTurn);
                    ((PlayerObject)mover).VisibleTiles.Clear();
                }
                else if (mover is Army army2)
                {
                    army.Units.AddRange(army2.Units);
                    army2.Units.Clear();
                    army.StepsTakenThisTurn = army.Units.Max(e => e.StepsTakenThisTurn);
                }
            }
            else if (mover is ICanFormGroup grouper)
            {
                IContainsUnits newGroup = CreateNewGroup(grouper, endTile);

                endTile.SetObject((PlaceableObject)newGroup);

                newGroup.Units.ForEach(e => e.DestinationTile = null);

                foreach (PlayerObject unit in newGroup.Units)
                {
                    unit.VisibleTiles.Clear();
                }

                UpdateVisibleAndRangeableTilesForObject((PlayerObject)newGroup);
            }

            if (originTile.Object == mover)
            {
                originTile.SetObject(null);
            }
            else if (originTile.Object is IContainsUnits unitContainer && unitContainer.Units.Contains((ICanFormGroup)mover))
            {
                ((IContainsUnits)originTile.Object).Units.Remove((ICanFormGroup)mover);
            }

            originTile.IsSelected = false;
        }

        private IContainsUnits CreateNewGroup(ICanFormGroup mover, Tile destinationTile)
        {
            Army newGroup;

            if (mover is ICanGatherResources)
            {
                newGroup = ActivePlayer.AddAndGetPlaceableObject<GathererGroup>();
            }
            else if (mover is IHasRange)
            {
                newGroup = ActivePlayer.AddAndGetPlaceableObject<RangedArmy>();
            }
            else
            {
                newGroup = ActivePlayer.AddAndGetPlaceableObject<Army>();
            }

            newGroup.Units.Add(mover);
            newGroup.Units.Add((ICanFormGroup)destinationTile.Object);
            newGroup.StepsTakenThisTurn = newGroup.Units.Max(e => e.StepsTakenThisTurn);
            newGroup.SetTexture();

            return newGroup;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Map.Draw(spriteBatch, ActivePlayer);

            if (_textNotification != null)
            {
                spriteBatch.DrawString(_fontLibrary.DefaultFontBold, _textNotification.Message, new Vector2(10, Map.Height * 45 + 5), _textNotification.FontColor);
            }

            if (!WindowUtils.ApplicationIsActivated())
            {
                return;
            }

            ImGui.Begin("UI");
            ImGui.SetWindowSize(new System.Numerics.Vector2(500, 1060));
            ImGui.SetWindowPos(new System.Numerics.Vector2(1480, -20));

            if (Popup != null)
            {
                Popup.Draw();

                if (Popup.IsInteractedWith)
                {
                    Popup = null;
                }
            }
            else
            {
                DrawEconomy();
                if (!DrawObjectInformation())
                {
                    ImGui.Dummy(new System.Numerics.Vector2(500, 190));
                }

                if (IsMyTurn)
                {
                    if (!DrawObjectContents() && !DrawObjectActions())
                    {
                        ImGui.Dummy(new System.Numerics.Vector2(500, 400));
                    }

                    if (ImGui.Button("End turn", new System.Numerics.Vector2(200, 40)))
                    {
                        EndTurn();
                    }

                    if (this is MultiplayerGame && !IsEnded)
                    {
                        ImGui.Dummy(new System.Numerics.Vector2(500, 120));

                        if (ImGui.Button("Resign", new System.Numerics.Vector2(200, 40)))
                        {
                            Popup = new Popup
                            {
                                Message = "Are you sure?",
                                ActionOnConfirm = delegate
                                {
                                    Resign();
                                }
                            };
                        }
                    }
                    else
                    {
                        ImGui.Dummy(new System.Numerics.Vector2(500, 120));
                    }
                }
                else
                {
                    ImGui.Dummy(new System.Numerics.Vector2(500, 560));
                }

                if (this is Sandbox || IsEnded)
                {
                    if (ImGui.Button("Return to menu", new System.Numerics.Vector2(200, 40)))
                    {
                        Popup = new Popup
                        {
                            Message = "This exits the current game. Are you sure?",
                            ActionOnConfirm = delegate
                            {
                                NewUiState = UiState.MainMenu;
                            }
                        };
                    }
                }
            }

            ImGui.End();
        }

        protected virtual void Resign()
        {
            Result = ActivePlayer.Color == TileColor.Blue ? "r+r" : "b+r";

            AddMove(new GameMove
            {
                PlayerName = ActivePlayer.Name,
                IsResign = true
            });

            EndGame();
        }

        protected virtual void AddMove(GameMove move)
        {
            move.MoveNumber = MoveHistory.Count;
            MoveHistory.Add(move);
        }

        #region Control panel
        protected virtual void DrawEconomy()
        {
            IEnumerable<ResourceCollection> resources = VisiblePlayer.ResourceStockpile;
            IEnumerable<ResourceCollection> resourcesGathered = VisiblePlayer.ResourcesGatheredLastTurn;

            ImGui.BeginChild("Resources", new System.Numerics.Vector2(500, 180));
            ImGui.SetWindowFontScale(2f);

            int foodCount = resources.Single(e => e.Resource == Resource.Food).Amount;
            int foodGathered = resourcesGathered.Single(e => e.Resource == Resource.Food).Amount;
            DrawResourceLine("Food", foodCount, foodGathered, new System.Numerics.Vector4(1, 0, 0, 1));

            int activePlayerTurnCount = MoveHistory
                .Where(e => e.IsEndOfTurn && e.PlayerName == ActivePlayer.Name)
                .Count() + 1;

            ImGui.SameLine();
            ImGui.Dummy(new System.Numerics.Vector2(86, 0));
            ImGui.SameLine();
            ImGui.Text($"Turn {activePlayerTurnCount}");

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

        private bool DrawObjectInformation()
        {
            var obj = SelectedObject ?? ViewedObject;

            if (obj == null || !(obj is IAttackable attackableObject))
            {
                return false;
            }

            ImGui.BeginChild("Information", new System.Numerics.Vector2(500, 190));

            ImGui.Text(obj.UiName ?? "??NAME NOT SET??");

            ImGui.NewLine();

            DrawObjectStat("Hit points", $"{attackableObject.HitPoints}/{attackableObject.MaxHitPoints}");

            if (obj is IAttacker attacker)
            {
                string attackString = attacker.AttackDamage.ToString();
                attackString += attacker is IHasRange ? " (ranged)" : " (melee)";
                DrawObjectStat("Attack", attackString);
                DrawObjectStat("Armor pierce", attacker.ArmorPierce.ToString());
            }

            if (obj is IHasRange ranger)
            {
                DrawObjectStat("Range", ranger.Range.ToString());
                ImGui.SameLine();
                if (ImGui.Button("Show", new System.Numerics.Vector2(40, 20)))
                {
                    ranger.RangeableTiles.SetTemporaryColor(TileColor.Pink);
                }

                if (ranger.HasMinimumRange)
                {
                    DrawObjectStat("Minimum range", 2.ToString());
                }
            }

            DrawObjectStat("Armor", attackableObject.MeleeArmor.ToString());
            DrawObjectStat("Ranged armor", attackableObject.RangedArmor.ToString());

            if (obj is PlayerObject playerObject)
            {
                DrawObjectStat("Line of sight", playerObject.LineOfSight.ToString());
            }

            if (obj is ICanMove mover)
            {
                DrawObjectStat("Steps left", $"{mover.Speed - mover.StepsTakenThisTurn}/{mover.Speed}");
            }

            ImGui.EndChild();
            return true;
        }

        private void DrawObjectStat(string description, string value)
        {
            for (int i = description.Length; i < 15; i++)
            {
                description += " ";
            }

            ImGui.Text(description);
            ImGui.SameLine();
            ImGui.Text(value);
        }

        private bool DrawObjectContents()
        {
            var obj = SelectedObject ?? ViewedObject;

            if (obj == null || !(obj is IContainsUnits unitContainer))
            {
                return false;
            }

            ImGui.BeginChild("Contents", new System.Numerics.Vector2(500, 400));

            ImGui.Text("Units");

            foreach (PlayerObject unit in unitContainer.Units.Cast<PlayerObject>())
            {
                IntPtr texture = _textureLibrary.TextureToIntPtr(unit.Texture);

                ImGui.ImageButton(texture, new System.Numerics.Vector2(50, 50));
                if (ImGui.IsItemActive())
                {
                    unitContainer.Units.ForEach(e => e.IsSubSelected = false);
                    ((ICanFormGroup)unit).IsSubSelected = true;
                    State = GameState.MovingObject;
                }

                ImGui.SameLine();

                string str = $"HP {unit.HitPoints}/{unit.MaxHitPoints}";
                ICanMove mover = (ICanMove)unit;
                str += $"\nSteps {mover.Speed - mover.StepsTakenThisTurn}/{mover.Speed}";

                ImGui.Text(str);
            }

            ImGui.EndChild();
            return true;
        }

        private bool DrawObjectActions()
        {
            if (SelectedObject == null)
            {
                return false;
            }

            if (Map.GetTileContainingObject(SelectedObject)?.Object is IContainsUnits && SelectedObject is ICanFormGroup)
            {
                // Unit is part of a group. Its only possible action is to move out of the group
                return false;
            }

            var defaultButtonSize = new System.Numerics.Vector2(80, 40);
            ImGui.BeginChild("Actions", new System.Numerics.Vector2(500, 400));

            bool isBusy = false;

            if (SelectedObject is IHasQueue queuer && queuer.HasSomethingQueued())
            {
                isBusy = true;

                if (SelectedObject is ICanMakeBuildings builder2 && builder2.HasBuildingQueued())
                {
                    PlaceableObjectFactory factory = ActivePlayer.GetFactoryByObjectType(builder2.BuildingTypeQueued);
                    string buildingName = factory.UiName;
                    string str = $"\n\nBuilding a {buildingName.Replace('\n', ' ')}, {builder2.QueueTurnsLeft} turn(s) left";

                    ImGui.Text(str);
                }
                else if (SelectedObject is ICanMakeUnits trainer && trainer.HasUnitQueued())
                {
                    string unitName = ActivePlayer.GetFactoryByObjectType(trainer.UnitTypeQueued).UiName;
                    string str = $"\n\nTraining a {unitName}, {trainer.QueueTurnsLeft} turn(s) left";

                    ImGui.Text(str);
                }
                else if (SelectedObject is ICanMakeResearch researcher && researcher.HasResearchQueued())
                {
                    ImGui.Text($"\n\nResearching {researcher.ResearchQueued.UiName.Replace('\n', ' ')}, {researcher.QueueTurnsLeft} turn(s) left");
                }

                ImGui.NewLine();

                if (ImGui.Button("Cancel"))
                {
                    CancelQueue((IHasQueue)SelectedObject);
                }
            }

            if (!isBusy)
            {
                int i = 1;

                if (SelectedObject is ICanMakeBuildings builder)
                {
                    ImGui.Text("Build");

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

                    if (i % 4 != 0)
                    {
                        ImGui.NewLine();
                    }

                    ImGui.NewLine();
                }

                if (SelectedObject is ICanMakeUnits trainer)
                {
                    ImGui.Text("Train");

                    i = 1;
                    foreach (Type unitType in trainer.UnitTypesAllowedToMake)
                    {
                        PlaceableObjectFactory factory = ActivePlayer.GetFactoryByObjectType(unitType);

                        var buttonSize = defaultButtonSize;

                        if (factory.UiName.Length > 9)
                        {
                            buttonSize.X += 4 * (factory.UiName.Length - 9);
                        }

                        if (ImGui.Button(factory.UiName, buttonSize))
                        {
                            TryQueueUnit(unitType, trainer);
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

                    if (i % 4 != 0)
                    {
                        ImGui.NewLine();
                    }

                    ImGui.NewLine();
                }

                if (SelectedObject is ICanMakeResearch researcher)
                {
                    ImGui.Text("Research");

                    List<ResearchEnum> allowedResearch = new List<ResearchEnum>(researcher.ResearchAllowedToMake);

                    i = 1;
                    foreach (ResearchEnum researchEnum in allowedResearch)
                    {
                        Research research = ResearchLibrary.GetByResearchEnum(researchEnum);

                        var buttonSize = defaultButtonSize;

                        if (research.UiName.Length > 9)
                        {
                            buttonSize.X += 4 * (research.UiName.Length - 9);
                        }

                        if (ImGui.Button(research.UiName, buttonSize))
                        {
                            TryResearch(research, researcher);
                        }

                        if (ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip(research.TooltipString());
                        }

                        if (i % 4 != 0)
                        {
                            ImGui.SameLine();
                        }

                        i++;
                    }
                }

                if (!(SelectedObject is ICanMove))
                {
                    if (i % 4 != 0)
                    {
                        ImGui.NewLine();
                    }

                    ImGui.Dummy(new System.Numerics.Vector2(500, 100));

                    if (ImGui.Button("Destroy"))
                    {
                        var tile = Map.SelectedTile;

                        Popup = new Popup
                        {
                            Message = "Are you sure?",
                            IsInformational = false,
                            ActionOnConfirm = delegate
                            {
                                DestroyOwnBuilding(tile);
                            }
                        };
                    }
                }
            }

            ImGui.EndChild();
            return true;
        }
        #endregion
    }
}
