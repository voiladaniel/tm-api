using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravianManager.Core;
using TravianManager.Core.Data;
using TravianManager.Core.DataProvider;

namespace TravianManager.BusinessLogic
{
    public class Calculator : ICalculator
    {
        private readonly ITemplateDataProvider _templateDataProvider;

        private readonly IPlanDataProvider _planDataProvider;

        private int TimeInterval;

        public Calculator(ITemplateDataProvider templateDataProvider, IPlanDataProvider planDataProvider)
        {
            _templateDataProvider = templateDataProvider;
            _planDataProvider = planDataProvider;
        }

        public TimeSpan CalculateDistance(Coordinate coordOrigin, Coordinate coordTarget, double speed, int arena)
        {
            var overBorderX = 0;
            var overBorderY = 0;

            overBorderX = 200 - Math.Abs(coordOrigin.XCoordinate) + 200 - Math.Abs(coordTarget.XCoordinate) + 1;
            overBorderY = coordOrigin.YCoordinate + (-1 * coordTarget.YCoordinate);
            var overBorderYOverY = 200 - Math.Abs(coordOrigin.YCoordinate) + 200 - Math.Abs(coordTarget.YCoordinate) + 1;
            var overBorderXOverY = coordTarget.XCoordinate + (-1 * coordOrigin.XCoordinate);

            var overBorderOverY = Math.Sqrt(overBorderYOverY * overBorderYOverY + overBorderXOverY * overBorderXOverY);
            var overBorder = Math.Sqrt(overBorderX * overBorderX + overBorderY * overBorderY);

            overBorder = overBorderOverY < overBorder ? overBorderOverY : overBorder;

            var fields = Math.Sqrt((coordOrigin.XCoordinate - coordTarget.XCoordinate) * (coordOrigin.XCoordinate - coordTarget.XCoordinate) +
                (coordOrigin.YCoordinate - coordTarget.YCoordinate) * (coordOrigin.YCoordinate - coordTarget.YCoordinate));

            if (overBorder < fields)
                fields = overBorder;

            var timeDistance = 0.0;

            if (fields > 20 && arena > 0)
            {
                var first20fields = (20.00 * 3600) / speed;

                var bonusSpeed = 100;
                for (int i = 0; i < arena; i++)
                {
                    bonusSpeed += 20;
                }
                var actualSpeed = (double)(speed * bonusSpeed) / 100;

                var arenaFields = (fields - 20) * 3600 / actualSpeed;

                timeDistance = Math.Round(first20fields + arenaFields);
            }
            else
            {
                timeDistance = Math.Round((fields * 3600) / speed);
            }

            return TimeSpan.FromSeconds(timeDistance);
        }

        public Attacker GetSpies(int AttackerID, int SpyID, int TroopSpeed, int TournamentSquare)
        {
            var attackerToReturn = new Attacker();

            var attacker = _templateDataProvider.GetAttacker(AttackerID);
            var spy = _templateDataProvider.GetAccount(SpyID);

            var attackerCoordinates = new Coordinate
            {
                XCoordinate = Convert.ToInt32(attacker.Account.XCoord),
                YCoordinate = Convert.ToInt32(attacker.Account.YCoord)
            };

            var defenderCoordinates = new Coordinate
            {
                XCoordinate = Convert.ToInt32(spy.XCoord),
                YCoordinate = Convert.ToInt32(spy.YCoord)
            };

            attackerToReturn.Account = attacker.Account;

            var distance = CalculateDistance(attackerCoordinates, defenderCoordinates, TroopSpeed, TournamentSquare);

            var llistDefenders = new List<Defender>();

            foreach (var defender in attacker.Defender.OrderByDescending(x => x.DefenderID))
            {
                var dateVar = new DateTime();
                var date = Convert.ToDateTime(defender.AttackingTime).AddDays(10) - dateVar.Add(distance).AddSeconds(5);
                defender.AttackingTime = date.Hours.ToString("00") + ":" + date.Minutes.ToString("00") + ":" + date.Seconds.ToString("00");

                llistDefenders.Add(defender);
            }
            attackerToReturn.Defender = llistDefenders;

            return attackerToReturn;
        }
        public void RefreshDataPerAttacker(int AttackerID)
        {
            var attacker = _templateDataProvider.GetAttacker(AttackerID);
            TimeInterval = _templateDataProvider.GetSetting(attacker.TemplateID).TimeInterval;

            var attackerCoordinates = new Coordinate
            {
                XCoordinate = Convert.ToInt32(attacker.Account.XCoord.Trim()),
                YCoordinate = Convert.ToInt32(attacker.Account.YCoord.Trim())
            };

            var speedMultiplier = attacker.SpeedArtifact.Equals("0") ? 1.0 : Convert.ToDouble(attacker.SpeedArtifact.Remove(attacker.SpeedArtifact.Length - 1));

            //update Attacking time
            foreach (var defender in attacker.Defender.OrderByDescending(x => x.DefenderID))
            {
                var defenderCoordinates = new Coordinate
                {
                    XCoordinate = Convert.ToInt32(defender.Account.XCoord),
                    YCoordinate = Convert.ToInt32(defender.Account.YCoord)
                };

                var distance = CalculateDistance(attackerCoordinates, defenderCoordinates, attacker.TroopSpeed * speedMultiplier, attacker.TournamentSquare);
                var dateVar = new DateTime();
                var date = Convert.ToDateTime(defender.ArrivingTime).AddDays(10) - dateVar.Add(distance);
                defender.AttackingTime = date.Hours.ToString("00") + ":" + date.Minutes.ToString("00") + ":" + date.Seconds.ToString("00");
            }

            //update Attacking type
            foreach (var defender in attacker.Defender.OrderByDescending(x => x.DefenderID))
            {
                var notBeforeTime = String.IsNullOrEmpty(attacker.NotBeforeTime) ? 0 : DateTimeOffset.Parse(attacker.NotBeforeTime).ToUnixTimeSeconds();
                var attTime = DateTimeOffset.Parse(defender.AttackingTime).ToUnixTimeSeconds();

                var otherAttacks = attacker.Defender
                    .Where(x => !x.DefenderID.Equals(defender.DefenderID))
                    .Select(x => DateTimeOffset.Parse(x.AttackingTime)
                    .ToUnixTimeSeconds())
                    .Where(x => x < attTime)
                    .ToList();


                if (notBeforeTime > attTime && notBeforeTime > 0)
                {
                    defender.AttackType = 0;
                }
                else
                {
                    var difference = FindMinDifference(attTime, otherAttacks);
                    defender.AttackType = difference < TimeInterval ? otherAttacks.Count > 0 ? 0 : 1 : 1;
                }
            }

            _templateDataProvider.UpdateDefenders(attacker.Defender);

        }

        public async Task RefreshDataPerTemplate(int templateID)
        {
            try
            {
                var attackers = await _templateDataProvider.GetAttackers(templateID, 1);
                TimeInterval = _templateDataProvider.GetSetting(templateID).TimeInterval;

                foreach (var attacker in attackers)
                {
                    if(attacker.Account.Name == "Eraser")
                    {

                    }
                    try
                    {
                        var attackerCoordinates = new Coordinate
                        {
                            XCoordinate = Convert.ToInt32(attacker.Account.XCoord),
                            YCoordinate = Convert.ToInt32(attacker.Account.YCoord)
                        };

                        var speedMultiplier = attacker.SpeedArtifact.Equals("0") ? 1.0 : Convert.ToDouble(attacker.SpeedArtifact.Remove(attacker.SpeedArtifact.Length - 1));

                        foreach (var defender in attacker.Defender.OrderByDescending(x => x.DefenderID))
                        {
                            var defenderCoordinates = new Coordinate
                            {
                                XCoordinate = Convert.ToInt32(defender.Account.XCoord),
                                YCoordinate = Convert.ToInt32(defender.Account.YCoord)
                            };

                            var distance = CalculateDistance(attackerCoordinates, defenderCoordinates, attacker.TroopSpeed * speedMultiplier, attacker.TournamentSquare);
                            var dateVar = new DateTime();
                            var date = Convert.ToDateTime(defender.ArrivingTime).AddDays(10) - dateVar.Add(distance);
                            defender.AttackingTime = date.Hours.ToString("00") + ":" + date.Minutes.ToString("00") + ":" + date.Seconds.ToString("00");

                            var notBeforeTime = String.IsNullOrEmpty(attacker.NotBeforeTime) ? 0 : DateTimeOffset.Parse(attacker.NotBeforeTime).ToUnixTimeSeconds();
                            var attTime = DateTimeOffset.Parse(defender.AttackingTime).ToUnixTimeSeconds();

                            var otherAttacks = attacker.Defender
                                .Where(x => !x.DefenderID.Equals(defender.DefenderID))
                                .Select(x => DateTimeOffset.Parse(x.AttackingTime)
                                .ToUnixTimeSeconds())
                                .Where(x => x < attTime)
                                .ToList();


                            if (notBeforeTime > attTime && notBeforeTime > 0)
                            {
                                defender.AttackType = 0;
                            }
                            else
                            {
                                var difference = FindMinDifference(attTime, otherAttacks);
                                defender.AttackType = difference < TimeInterval ? otherAttacks.Count > 0 ? 0 : 1 : 1;
                            }

                            _templateDataProvider.UpdateDefender(defender);
                        }
                    }
                    catch(Exception e)
                    {
                        var er = e.ToString();
                    }
                }
            }
            catch(Exception ex)
            {
                var error = ex.ToString();
            }
        }

        public async Task<IEnumerable<Target>> RefreshDataPerPlan(int PlanID)
        {
            var targets = await _planDataProvider.GetTargets(PlanID);
            var planData = _planDataProvider.GetNotAsyncPlanSetting(PlanID);
            var bufferTime = planData.TimeBuffer;
            var safeTime = planData.SafeTime;
            var serverSpeed = planData.ServerSpeed;

            foreach (var target in targets)
            {
                var planTargetCoordinates = new Coordinate
                {
                    XCoordinate = Convert.ToInt32(target.Account.XCoord),
                    YCoordinate = Convert.ToInt32(target.Account.YCoord)
                };

                var plandefenders = target.PlanDefender.ToList();
                foreach (var planDefender in plandefenders)
                {
                    var planDefenderCoordinates = new Coordinate
                    {
                        XCoordinate = Convert.ToInt32(planDefender.PlanAttacker.Account.XCoord),
                        YCoordinate = Convert.ToInt32(planDefender.PlanAttacker.Account.YCoord)
                    };

                    var speedMultiplier = planDefender.PlanAttacker.SpeedArtifact.Equals("0") ? 1.0 :
                    Convert.ToDouble(planDefender.PlanAttacker.SpeedArtifact.Remove(planDefender.PlanAttacker.SpeedArtifact.Length - 1));

                    var distance = CalculateDistance(planDefenderCoordinates, planTargetCoordinates, planDefender.PlanAttacker.TroopSpeed * serverSpeed * speedMultiplier, planDefender.PlanAttacker.TournamentSquare);

                    var dateVar = new DateTime();
                    var date = Convert.ToDateTime(planDefender.ArrivingTime).AddDays(10) - dateVar.Add(distance).AddSeconds(bufferTime);
                    planDefender.AttackingTime = date.Hours.ToString("00") + ":" + date.Minutes.ToString("00") + ":" + date.Seconds.ToString("00");
                }

                _planDataProvider.UpdatePlanDefenders(plandefenders);

            }
            //check for Attacker conflicts
            var planDefs = (from attacker in targets
                            .Select(x => x.PlanDefender).ToList()
                            from item in attacker
                            select item).ToList();


            foreach (var attacker in planDefs)
            {
                var attTime = DateTimeOffset.Parse(attacker.AttackingTime).ToUnixTimeSeconds();

                var otherAttacks = planDefs
                    .Where(x => !x.PlanDefenderID.Equals(attacker.PlanDefenderID))
                    .Select(x => DateTimeOffset.Parse(x.AttackingTime)
                    .ToUnixTimeSeconds())
                    .Where(x => x <= attTime)
                    .ToList();

                var difference = FindMinDifference(attTime, otherAttacks);

                attacker.AttackerConflict = difference < safeTime ?
                        otherAttacks.Count > 0 ? 1 : 0
                    : 0;
            }
            _planDataProvider.UpdatePlanDefenders(planDefs);

            return targets;
        }

        public async Task<IEnumerable<Target>> RefreshDataPerPlanAndPlanAttacker(int PlanID, int PlanAttackerID)
        {
            var targets = await _planDataProvider.GetTargets(PlanID);
            var planData = _planDataProvider.GetNotAsyncPlanSetting(PlanID);
            var bufferTime = planData.TimeBuffer;
            var safeTime = planData.SafeTime;
            var serverSpeed = planData.ServerSpeed;

            foreach (var target in targets.Where(x => x.PlanDefender.Any(p => p.PlanAttackerID == PlanAttackerID)))
            {
                var planTargetCoordinates = new Coordinate
                {
                    XCoordinate = Convert.ToInt32(target.Account.XCoord),
                    YCoordinate = Convert.ToInt32(target.Account.YCoord)
                };

                var plandefenders = target.PlanDefender.ToList();
                foreach (var planDefender in plandefenders)
                {
                    var planDefenderCoordinates = new Coordinate
                    {
                        XCoordinate = Convert.ToInt32(planDefender.PlanAttacker.Account.XCoord),
                        YCoordinate = Convert.ToInt32(planDefender.PlanAttacker.Account.YCoord)
                    };

                    var speedMultiplier = planDefender.PlanAttacker.SpeedArtifact.Equals("0") ? 1.0 :
                    Convert.ToDouble(planDefender.PlanAttacker.SpeedArtifact.Remove(planDefender.PlanAttacker.SpeedArtifact.Length - 1));

                    var distance = CalculateDistance(planDefenderCoordinates, planTargetCoordinates, planDefender.PlanAttacker.TroopSpeed * serverSpeed * speedMultiplier, planDefender.PlanAttacker.TournamentSquare);

                    var dateVar = new DateTime();
                    var date = Convert.ToDateTime(planDefender.ArrivingTime).AddDays(10) - dateVar.Add(distance).AddSeconds(bufferTime);
                    planDefender.AttackingTime = date.Hours.ToString("00") + ":" + date.Minutes.ToString("00") + ":" + date.Seconds.ToString("00");
                }

                _planDataProvider.UpdatePlanDefenders(plandefenders);

            }
            //check for Attacker conflicts
            var planDefs = (from attacker in targets
                            .Select(x => x.PlanDefender.Where(p => p.PlanAttackerID == PlanAttackerID)).ToList()
                            from item in attacker
                            select item).ToList();


            foreach (var attacker in planDefs)
            {
                var attTime = DateTimeOffset.Parse(attacker.AttackingTime).ToUnixTimeSeconds();

                var otherAttacks = planDefs
                    .Where(x => !x.PlanDefenderID.Equals(attacker.PlanDefenderID))
                    .Select(x => DateTimeOffset.Parse(x.AttackingTime)
                    .ToUnixTimeSeconds())
                    .Where(x => x <= attTime)
                    .ToList();

                var difference = FindMinDifference(attTime, otherAttacks);

                attacker.AttackerConflict = difference < safeTime ?
                        otherAttacks.Count > 0 ? 1 : 0
                    : 0;
            }
            _planDataProvider.UpdatePlanDefenders(planDefs);

            return targets;
        }

        public async Task RefreshDataPerTarget(int TargetID)
        {
            var target = _planDataProvider.GetTarget(TargetID);
            var planID = target.PlanID ?? 0;
            var planData = _planDataProvider.GetNotAsyncPlanSetting(planID);
            var bufferTime = planData.TimeBuffer;
            var safeTime = planData.SafeTime;
            var serverSpeed = planData.ServerSpeed;

            var planTargetCoordinates = new Coordinate
            {
                XCoordinate = Convert.ToInt32(target.Account.XCoord),
                YCoordinate = Convert.ToInt32(target.Account.YCoord)
            };

            var plandefenders = target.PlanDefender.ToList();
            foreach (var planDefender in plandefenders)
            {
                var planDefenderCoordinates = new Coordinate
                {
                    XCoordinate = Convert.ToInt32(planDefender.PlanAttacker.Account.XCoord),
                    YCoordinate = Convert.ToInt32(planDefender.PlanAttacker.Account.YCoord)
                };

                var speedMultiplier = planDefender.PlanAttacker.SpeedArtifact.Equals("0") ? 1.0 :
                Convert.ToDouble(planDefender.PlanAttacker.SpeedArtifact.Remove(planDefender.PlanAttacker.SpeedArtifact.Length - 1));

                var distance = CalculateDistance(planDefenderCoordinates, planTargetCoordinates, planDefender.PlanAttacker.TroopSpeed * serverSpeed * speedMultiplier, planDefender.PlanAttacker.TournamentSquare);

                var dateVar = new DateTime();
                var date = Convert.ToDateTime(planDefender.ArrivingTime).AddDays(10) - dateVar.Add(distance).AddSeconds(bufferTime);
                planDefender.AttackingTime = date.Hours.ToString("00") + ":" + date.Minutes.ToString("00") + ":" + date.Seconds.ToString("00");
            }

            _planDataProvider.UpdatePlanDefenders(plandefenders);

            var targets = await _planDataProvider.GetTargets(planID);

            //check for Attacker conflicts
            var planDefs = (from attacker in targets
                            .Select(x => x.PlanDefender).ToList()
                            from item in attacker
                            select item).ToList();


            foreach (var attacker in planDefs)
            {
                var attTime = DateTimeOffset.Parse(attacker.AttackingTime).ToUnixTimeSeconds();

                var otherAttacks = planDefs
                    .Where(x => !x.PlanDefenderID.Equals(attacker.PlanDefenderID))
                    .Select(x => DateTimeOffset.Parse(x.AttackingTime)
                    .ToUnixTimeSeconds())
                    .Where(x => x <= attTime)
                    .ToList();

                var difference = FindMinDifference(attTime, otherAttacks);

                attacker.AttackerConflict = difference < safeTime ?
                        otherAttacks.Count > 0 ? 1 : 0
                    : 0;
            }
            _planDataProvider.UpdatePlanDefenders(planDefs);
        }

        public async Task RefreshDataPerPlanDefender(int PlanDefenderID)
        {
            var planDefender = _planDataProvider.GetPlanDefender(PlanDefenderID);
            var planData = _planDataProvider.GetNotAsyncPlanSetting(planDefender.PlanID);
            var bufferTime = planData.TimeBuffer;
            var safeTime = planData.SafeTime;

            var planTargetCoordinates = new Coordinate
            {
                XCoordinate = Convert.ToInt32(planDefender.Account.XCoord),
                YCoordinate = Convert.ToInt32(planDefender.Account.YCoord)
            };

            var speedMultiplier = planDefender.PlanAttacker.SpeedArtifact.Equals("0") ? 1.0 : 
                Convert.ToDouble(planDefender.PlanAttacker.SpeedArtifact.Remove(planDefender.PlanAttacker.SpeedArtifact.Length - 1));

            //update Attacking time

            var planDefenderCoordinates = new Coordinate
            {
                XCoordinate = Convert.ToInt32(planDefender.PlanAttacker.Account.XCoord),
                YCoordinate = Convert.ToInt32(planDefender.PlanAttacker.Account.YCoord)
            };

            var distance = CalculateDistance(planDefenderCoordinates, planTargetCoordinates, planDefender.PlanAttacker.TroopSpeed * speedMultiplier, planDefender.PlanAttacker.TournamentSquare);
            
            var dateVar = new DateTime();
            var date = Convert.ToDateTime(planDefender.ArrivingTime).AddDays(10) - dateVar.Add(distance).AddSeconds(bufferTime);
            planDefender.AttackingTime = date.Hours.ToString("00") + ":" + date.Minutes.ToString("00") + ":" + date.Seconds.ToString("00");

            _planDataProvider.UpdatePlanDefender(planDefender);

            //check for Attacker conflicts
            var targets = await _planDataProvider.GetTargets(planDefender.PlanID);

            var planDefs = (from attacker in targets
                            .Where(x => x.PlanDefender.Any(p => p.PlanAttackerID == planDefender.PlanAttackerID))
                            .Select(x => x.PlanDefender).ToList()
                            from item in attacker
                            select item).ToList();


            foreach (var attacker in planDefs)
            {
                var attTime = DateTimeOffset.Parse(attacker.AttackingTime).ToUnixTimeSeconds();

                var otherAttacks = planDefs
                    .Where(x => !x.PlanDefenderID.Equals(attacker.PlanDefenderID))
                    .Select(x => DateTimeOffset.Parse(x.AttackingTime)
                    .ToUnixTimeSeconds())
                    .Where(x => x < attTime)
                    .ToList();

                var difference = FindMinDifference(attTime, otherAttacks);

                attacker.AttackerConflict = difference < safeTime ? 
                        otherAttacks.Count > 0 ? 1 : 0 
                    : 0;
            }

            _planDataProvider.UpdatePlanDefenders(planDefs);

        }


        public long FindMinDifference(List<long> list)
        {
            list.Sort();

            long diff = long.MaxValue;

            for (int i = 0; i < list.Count - 1; i++)
                if (list[i + 1] - list[i] < diff)
                    diff = list[i + 1] - list[i];

            return diff;
        }

        public long FindMinDifference(long attack, List<long> otherAttacks)
        {
            otherAttacks.Sort();

            var diffValue = default(long);
            foreach(var item in otherAttacks)
            {
                if(attack > item)
                {
                    diffValue = item >= attack ? item - attack : attack - item;

                    if (diffValue < TimeInterval)
                        return diffValue;
                }
            }

            return diffValue;
        }

    }
}
