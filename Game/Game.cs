using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoeBoardgame
{
    class Game
    {
        public int WidthPixels { get; set; }
        public int HeightPixels { get; set; }

        public GameState State { get; set; }
        public List<Player> Players { get; set; }
        public List<GameMove> MoveHistory { get; set; }

        protected Map Map;

        protected bool IsMyTurn;

        private TextNotification _textNotification;

        private Type _placingBuildingType;

        private readonly TextureLibrary _textureLibrary;
        private readonly FontLibrary _fontLibrary;
        protected readonly ResearchLibrary ResearchLibrary;

        public Game(TextureLibrary textureLibrary, FontLibrary fontLibrary, ResearchLibrary researchLibrary)
        {
            WidthPixels = 1920;
            HeightPixels = 1020;

            _fontLibrary = fontLibrary;
            _textureLibrary = textureLibrary;
            ResearchLibrary = researchLibrary;

            MoveHistory = new List<GameMove>();
        }

        protected Player ActivePlayer => Players.Single(e => e.IsActive);

        protected virtual Player VisiblePlayer => ActivePlayer;

        private Tile GetTileByLocation(Point location) => Map.GetTileByLocation(location);

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

        protected virtual void StartTurn()
        {
            UpdateQueues();

            if (ActivePlayer.IsPopulationRevolting)
            {
                HandleRevolt();
            }

            SetRangeableTiles();
        }

        protected virtual void EndTurn()
        {
            ClearCurrentSelection();

            MoversTakeSteps();

            DestroyEmptyArmies();

            ActivePlayer.ResetAttackers();
            ActivePlayer.ResetResourcesGatheredLastTurn();

            GatherResources();
            ConsumeFood();

            MakeMove(new GameMove
            {
                PlayerName = ActivePlayer.Name,
                IsEndOfTurn = true
            });

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

        private void SetRangeableTiles()
        {
            foreach (IHasRange ranger in ActivePlayer.OwnedObjects.Where(e => e is IHasRange))
            {
                var tile = Map.FindTileContainingObject((PlayerObject)ranger);
                ranger.RangeableTiles = Map.FindTilesInRangeOfTile(tile, ranger.Range, ranger.HasMinimumRange);
            }
        }

        protected void SetFogOfWar(Player player)
        {
            Map.ResetFogOfWar();

            foreach (Tile tile in player.VisibleTiles)
            {
                tile.HasFogOfWar = false;
            }
        }

        private void UpdateVisibleTilesForObject(PlayerObject obj)
        {
            obj.VisibleTiles = new List<Tile>();

            // Skip objects that are gathering or part of an army
            if (!Map.Tiles.Any(e => e.Object == obj))
            {
                return;
            }

            Tile originTile = Map.FindTileContainingObject(obj);

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

            SetFogOfWar(ActivePlayer);
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
                CreateUnit(trainer.UnitTypeQueued, trainer);

                trainer.UnitTypeQueued = null;
            }
            else if (queuer is ICanMakeResearch researcher && researcher.ResearchQueued != null)
            {
                foreach (ICanMakeResearch similarResearcher in ActivePlayer.OwnedObjects.Where(e => e.GetType() == researcher.GetType()))
                {
                    similarResearcher.ResearchAllowedToMake.Remove(researcher.ResearchQueued.ResearchEnum);
                }

                ICanMakeResearchFactory factory = (ICanMakeResearchFactory)ActivePlayer.GetFactoryByObjectType(researcher.GetType());
                factory.ResearchAllowedToMake.Remove(researcher.ResearchQueued.ResearchEnum);

                researcher.ResearchQueued.Effect(ActivePlayer);
                researcher.ResearchQueued = null;
            }
        }

        private void PlaceBuilding(Type buildingType, Tile destinationTile)
        {
            destinationTile.BuildingUnderConstruction = false;

            PlayerObject building = ActivePlayer.AddAndGetPlaceableObject(buildingType);

            if (building is Mine mine)
            {
                mine.Resource = destinationTile.Type == TileType.GoldMine ? Resource.Gold :
                    destinationTile.Type == TileType.StoneMine ? Resource.Stone : Resource.Iron;
            }

            destinationTile.SetObject(building);

            UpdateVisibleTilesForObject(building);
        }

        private void CreateUnit(Type unitType, ICanMakeUnits trainer)
        {
            PlayerObject unit = ActivePlayer.AddAndGetPlaceableObject(unitType);
            ICanMove mover = (ICanMove)unit;

            Tile originTile = Map.FindTileContainingObject((PlayerObject)trainer);
            Tile destinationTile = trainer.WayPoint;

            if (destinationTile != null)
            {
                IEnumerable<Tile> path = Map.FindPath(originTile, destinationTile, mover);

                if (path == null)
                {
                    _textNotification = new TextNotification
                    {
                        FontColor = Color.Red,
                        Message = "A waypoint became invalid"
                    };

                    destinationTile = null;
                    trainer.WayPoint = null;
                }
                else if (path.First().Object == null)
                {
                    path.First().SetObject((PlayerObject)mover);
                    mover.DestinationTile = destinationTile;
                }
                else
                {
                    MergeMoverWithDestination(originTile, destinationTile, mover);
                }
            }

            if (destinationTile == null)
            {
                var pathFinder = new PathFinder(Map);
                IEnumerable<Tile> adjacentTiles = pathFinder.GetAdjacentTiles(originTile);

                if (!adjacentTiles.Any())
                {
                    _textNotification = new TextNotification
                    {
                        FontColor = Color.Red,
                        Message = "A trained unit had no possible tiles to be placed on"
                    };
                    ActivePlayer.OwnedObjects.Remove(unit);
                }
                else
                {
                    adjacentTiles.Where(e => e.SeemsAccessible).First().SetObject(unit);
                }
            }

            UpdateVisibleTilesForObject(unit);

            mover.DestinationTile = destinationTile;
        }

        private void MoversTakeSteps()
        {
            List<ICanMove> movers = ActivePlayer.OwnedObjects.FilterByType<ICanMove>().ToList();

            foreach (ICanMove mover in movers)
            {
                if (mover.DestinationTile != null && !mover.HasSpentAllMovementPoints())
                {
                    Tile sourceTile = Map.FindTileContainingObject((PlayerObject)mover);
                    var path = Map.FindPath(sourceTile, mover.DestinationTile, mover);

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

                    ProgressOnPath(mover, path);
                    UpdateVisibleTilesForObject((PlayerObject)mover);
                }

                mover.StepsTakenThisTurn = 0;

                if (mover is IContainsUnits group)
                {
                    group.Units.ForEach(e => e.StepsTakenThisTurn = 0);
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
                UpdateVisibleTilesForObject(obj);
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

            if (!ActivePlayer.OwnedObjects.Contains(tile.Object))
            {
                tile.IsViewed = true;
                tile.IsSelected = false;
                return;
            }

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
                        var path = Map.FindPath(Map.FindTileContainingObject(tile.Object), mover.DestinationTile, mover);

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
                Map.Tiles.Single(e => e == builder2.BuildingDestinationTile).SetTemporaryColor(TileColor.Orange);
            }
        }

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

            Tile originTile = Map.FindTileContainingObject(SelectedObject);

            if (SelectedObject is IAttacker attacker 
                && !destinationTile.HasFogOfWar
                && destinationTile.Object != null
                && (
                    (destinationTile.Object is PlayerObject defender && defender.Owner != ActivePlayer)
                    || destinationTile.Object is GaiaObject
                ))
            {
                TryAttackObject(originTile, destinationTile, attacker, (IAttackable)destinationTile.Object);
                return;
            }

            if (SelectedObject is ICanMakeUnits trainer && destinationTile.SeemsAccessible)
            {
                SetWaypoint(trainer, originTile, destinationTile);
            }

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

        protected virtual void SetWaypoint(ICanMakeUnits trainer, Tile originTile, Tile destinationTile)
        {
            trainer.WayPoint = destinationTile;
            destinationTile.SetTemporaryColor(TileColor.Orange);

            MakeMove(new GameMove
            {
                PlayerName = ActivePlayer.Name,
                OriginTileId = originTile.Id,
                DestinationTileId = destinationTile.Id,
                IsWaypoint = true
            });
        }

        protected virtual void TryMoveObject(Tile originTile, Tile destinationTile, ICanMove mover)
        {
            var path = Map.FindPath(originTile, destinationTile, mover);

            if (path == null)
            {
                return;
            }

            if (mover.HasSpentAllMovementPoints() && originTile.Object != mover)
            {
                // Don't move a unit out of a group unless we can do so immediately
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

            path.Highlight(TileColor.Orange);

            if (!mover.HasSpentAllMovementPoints())
            {
                ProgressOnPath(mover, path);
                UpdateVisibleTilesForObject((PlayerObject)mover);
            }

            MakeMove(new GameMove
            {
                PlayerName = ActivePlayer.Name,
                IsMovement = true,
                OriginTileId = originTile.Id,
                DestinationTileId = destinationTile.Id,
                SubselectedUnitHitpoints = moverHp
            });
        }

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
            int damage;
            bool boarFoughtBack = false;

            if (attacker is ICanMove mover)
            {
                mover.DestinationTile = null;
            }

            if (attacker is IHasRange ranger)
            {
                range = ranger.Range;
                var armor = defender.RangedArmor - attacker.ArmorPierce;
                damage = attacker.AttackDamage - (armor > 0 ? armor : 0);
            }
            else
            {
                range = 1;
                var armor = defender.MeleeArmor - attacker.ArmorPierce;
                damage = attacker.AttackDamage - (armor > 0 ? armor : 0);
            }

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

            bool defenderDied = defender.TakeDamage(attacker, damage);

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

            if (attacker is ICanMove mover3 && !(((PlayerObject)mover3).Owner.Civilization is France && attacker is ICavalry))
            {
                mover3.StepsTakenThisTurn = mover3.Speed;
            }

            _textNotification = new TextNotification
            {
                FontColor = Color.Green,
                Message = $"The attack did {damage} damage"
            };

            if (boarFoughtBack)
            {
                _textNotification.Message += ", but the boar fought back";
            }

            MakeMove(new GameMove
            {
                PlayerName = ActivePlayer.Name,
                IsAttack = true,
                DestinationTileId = destinationTile.Id,
                OriginTileId = originTile.Id
            });
        }

        private void HandleObjectKilled(IAttackable defender, IAttacker attacker, Tile defenderTile)
        {
            if (defender is IEconomicBuilding economicBuilding && economicBuilding.Units.Any())
            {
                if (economicBuilding.Units.Count > 1)
                {
                    GathererGroup newGroup = ActivePlayer.AddAndGetPlaceableObject<GathererGroup>();

                    foreach (ICanFormGroup grouper in economicBuilding.Units)
                    {
                        newGroup.Units.Add(grouper);
                    }

                    newGroup.SetTexture();

                    defenderTile.SetObject(newGroup);
                    UpdateVisibleTilesForObject(newGroup);
                }
                else
                {
                    PlayerObject survivor = (PlayerObject)economicBuilding.Units[0];

                    defenderTile.SetObject(survivor);
                    UpdateVisibleTilesForObject(survivor);
                }
            }
            else
            {
                defenderTile.SetObject(null);

                if (!(attacker is IHasRange))
                {
                    Map.MoveMover((ICanMove)attacker, defenderTile);
                    UpdateVisibleTilesForObject((PlayerObject)attacker);
                }
            }

            if (defender is PlayerObject playerObject)
            {
                playerObject.Owner.OwnedObjects.Remove(playerObject);
            }
            else if (defender is GaiaObject && !(attacker is TownCenter))
            {
                if (defender is Deer)
                {
                    ActivePlayer.ResourceCollection.Single(e => e.Resource == Resource.Food).Amount += 50;
                }
                else if (defender is Boar)
                {
                    ActivePlayer.ResourceCollection.Single(e => e.Resource == Resource.Food).Amount += 300;
                }
            }
        }

        private bool BoarDefense(Tile boarTile, Tile attackerTile)
        {
            var pathFinder = new PathFinder(Map);
            Boar boar = (Boar)boarTile.Object;

            if (pathFinder.GetAdjacentTiles(boarTile).Contains(attackerTile))
            {
                // Fight back
                var defender = (PlayerObject)attackerTile.Object;

                int damage = boar.AttackDamage - defender.MeleeArmor;

                bool defenderDied = defender.TakeDamage(boar, damage);
                
                if (defenderDied)
                {
                    attackerTile.SetObject(null);
                    defender.Owner.OwnedObjects.Remove(defender);
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

            MakeMove(new GameMove
            {
                PlayerName = ActivePlayer.Name,
                OriginTileId = Map.FindTileContainingObject((PlaceableObject)builder).Id,
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

            MakeMove(new GameMove
            {
                PlayerName = ActivePlayer.Name,
                OriginTileId = Map.FindTileContainingObject((PlaceableObject)trainer).Id,
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

        private void HandleHover()
        {
            var selectedObject = SelectedObject;

            if (selectedObject == null)
            {
                return;
            }

            if (State != GameState.PlacingBuilding && !(selectedObject is ICanMakeUnits))
            {
                ClearTemporaryTileColors();
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
                        path.Highlight(TileColor.Orange);
                    }
                }

                if (Map.HoveredTile == Map.SelectedTile)
                {
                    return;
                }

                IEnumerable<Tile> pathFromSelectedToHovered = Map.FindPathFromSelectedToHovered(mover);

                if (pathFromSelectedToHovered != null)
                {
                    pathFromSelectedToHovered.Highlight(TileColor.Teal);
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

            ClearTemporaryTileColors();

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
                ClearTemporaryTileColors();
            }
        }

        protected void ClearTemporaryTileColors()
        {
            foreach (var tile in Map.Tiles)
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

            Tile originTile = Map.FindTileContainingObject(SelectedObject);

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
            Tile originTile = Map.FindTileContainingObject((PlaceableObject)mover);

            int steps = path.Count() >= mover.Speed - mover.StepsTakenThisTurn ? mover.Speed - mover.StepsTakenThisTurn : path.Count();

            if (mover is Army army)
            {
                army.Units.ForEach(e => e.StepsTakenThisTurn += steps);
            }

            Tile endTile = path.ToList()[-1 + steps]; // Path does not contain origin tile

            bool destinationReached = path.Last() == endTile;

            if (endTile.HasFogOfWar && endTile.Object != null)
            {
                for (int i = 0; i < steps; i++)
                {
                    mover.StepsTakenThisTurn++;

                    Map.MoveMover(mover, path.ToList()[i]);
                    UpdateVisibleTilesForObject((PlayerObject)mover);

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

                MergeMoverWithDestination(originTile, endTile, mover);
            }
            else
            {
                Map.MoveMover(mover, endTile);
                UpdateVisibleTilesForObject((PlayerObject)mover);
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
            // TODO highlight unit?
            _textNotification = new TextNotification
            {
                Message = "Some units' destinations have become invalid. Their paths were reset",
                FontColor = Color.Red
            };
            mover.DestinationTile = null;
        }

        private void MergeMoverWithDestination(Tile originTile, Tile endTile, ICanMove mover)
        {
            if (endTile.Object is IEconomicBuilding economicBuilding
                && mover is ICanGatherResources gatherer
                && economicBuilding.Units.Count < economicBuilding.MaxUnits)
            {
                if (gatherer is GathererGroup group)
                {
                    if (group.Units.Count > economicBuilding.MaxUnits - economicBuilding.Units.Count)
                    {
                        throw new Exception($"Bug in pathfinding. Gatherer group is trying to merge with economic building but there's not enough space");
                    }

                    group.ResourceGathering = economicBuilding.Resource;
                    economicBuilding.Units.AddRange(group.Units);
                }
                else
                {
                    gatherer.ResourceGathering = economicBuilding.Resource;
                    economicBuilding.Units.Add((ICanFormGroup)gatherer);
                }

                if (originTile.Object == mover)
                {
                    originTile.SetObject(null);
                }

                originTile.IsSelected = false;
            }
            else if (endTile.Object is Army group
                && mover.GetType() == group.UnitType
                && group.Units.Count < group.MaxUnits)
            {
                group.Units.Add((ICanFormGroup)mover);
                group.StepsTakenThisTurn = group.Units.Max(e => e.StepsTakenThisTurn);

                if (originTile.Object == mover)
                {
                    originTile.SetObject(null);
                }
                else
                {
                    ((IContainsUnits)originTile.Object).Units.Remove((ICanFormGroup)mover);
                }

                originTile.IsSelected = false;
            }
            else if (mover is ICanFormGroup grouper
                && endTile.Object.GetType() == mover.GetType()
                && !(endTile.Object is ICanMakeBuildings builder && builder.HasBuildingQueued()))
            {
                IContainsUnits newGroup = CreateNewGroup(grouper, endTile);

                endTile.SetObject((PlaceableObject)newGroup);

                if (originTile.Object == mover)
                {
                    originTile.SetObject(null);
                }

                originTile.IsSelected = false;
            }
            else
            {
                throw new Exception($"Bug in pathfinding. Mover is trying to illegally merge with another object. Mover type is {mover.GetType().Name} and destination object type is {endTile.Object.GetType().Name}");
            }
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

        public virtual void Update(SpriteBatch spriteBatch)
        {
            Map.Draw(spriteBatch);

            if (_textNotification != null)
            {
                spriteBatch.DrawString(_fontLibrary.DefaultFontBold, _textNotification.Message, new Vector2(10, Map.Height * 45 + 5), _textNotification.FontColor);
            }

            ImGui.Begin("UI");
            ImGui.SetWindowSize(new System.Numerics.Vector2(500, 1060));
            ImGui.SetWindowPos(new System.Numerics.Vector2(1480, -20));

            DrawEconomy();
            if (!DrawObjectInformation())
            {
                ImGui.Dummy(new System.Numerics.Vector2(500, 120));
            }

            if (IsMyTurn)
            {
                if (!DrawObjectContents() && !DrawObjectActions())
                {
                    ImGui.Dummy(new System.Numerics.Vector2(500, 300));
                }

                if (ImGui.Button("End turn", new System.Numerics.Vector2(200, 40)))
                {
                    EndTurn();
                }
            }

            ImGui.End();
        }

        protected virtual void MakeMove(GameMove move)
        {
            move.MoveNumber = MoveHistory.Count;
            MoveHistory.Add(move);
        }

        #region Control panel
        protected virtual void DrawEconomy()
        {
            IEnumerable<ResourceCollection> resources = VisiblePlayer.ResourceCollection;
            IEnumerable<ResourceCollection> resourcesGathered = VisiblePlayer.ResourcesGatheredLastTurn;

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

        private bool DrawObjectInformation()
        {
            var obj = SelectedObject ?? ViewedObject;

            if (obj == null || !(obj is IAttackable attackableObject))
            {
                return false;
            }

            ImGui.BeginChild("Information", new System.Numerics.Vector2(500, 120));

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
                    ranger.RangeableTiles.Highlight(TileColor.Pink);
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

            ImGui.BeginChild("Contents", new System.Numerics.Vector2(500, 300));

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

            if (Map.FindTileContainingObject(SelectedObject)?.Object is IContainsUnits && SelectedObject is ICanFormGroup)
            {
                // Unit is part of a group. Its only possible action is to move out of the group
                return false;
            }

            var defaultButtonSize = new System.Numerics.Vector2(80, 40);
            ImGui.BeginChild("Actions", new System.Numerics.Vector2(500, 300));

            bool isBusy = false;

            if (SelectedObject is IHasQueue queuer && queuer.HasSomethingQueued())
            {
                isBusy = true;

                if (SelectedObject is ICanMakeBuildings builder2 && builder2.HasBuildingQueued())
                {
                    string buildingName = ActivePlayer.GetFactoryByObjectType(builder2.BuildingTypeQueued).UiName;
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

                // TODO draw hourglass?
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
                }

                ImGui.NewLine();

                if (i % 4 != 0)
                {
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
                }

                ImGui.NewLine();

                if (i % 4 != 0)
                {
                    ImGui.NewLine();
                }

                if (SelectedObject is ICanMakeResearch researcher)
                {
                    ImGui.Text("Research");

                    i = 1;
                    foreach (ResearchEnum researchEnum in researcher.ResearchAllowedToMake)
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
            }

            ImGui.EndChild();
            return true;
        }
        #endregion
    }
}
