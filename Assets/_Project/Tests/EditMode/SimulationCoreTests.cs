using System.Collections.Generic;
using NUnit.Framework;

namespace GynaikoeidoPolis.Tests.EditMode
{
    public class SimulationCoreTests
    {
        [Test]
        public void AdvanceTick_After25Ticks_AdvancesOneHour()
        {
            SimulationClock clock = new SimulationClock();
            TimeChange lastChange = default;

            for (int i = 0; i < 25; i++)
            {
                lastChange = clock.AdvanceTick();
            }

            Assert.That(clock.CurrentTick, Is.EqualTo(25));
            Assert.That(clock.Hour, Is.EqualTo(1));
            Assert.That(lastChange.HourChanged, Is.True);
            Assert.That(lastChange.DayChanged, Is.False);
        }

        [Test]
        public void AdvanceTick_After24Hours_AdvancesOneDay()
        {
            SimulationClock clock = new SimulationClock();
            TimeChange lastChange = default;

            for (int i = 0; i < 25 * 24; i++)
            {
                lastChange = clock.AdvanceTick();
            }

            Assert.That(clock.CurrentTick, Is.EqualTo(600));
            Assert.That(clock.Hour, Is.Zero);
            Assert.That(clock.Day, Is.EqualTo(1));
            Assert.That(lastChange.HourChanged, Is.True);
            Assert.That(lastChange.DayChanged, Is.True);
        }

        [Test]
        public void CalculateDies_WhenFoodDropsBelowThreshold_KillsExpectedPeople()
        {
            GameState gameState = new GameState
            {
                StoredFood = GameState.FoodThreshold - 1,
                LivingAnthropoiCount = 20
            };

            bool someoneDied = gameState.CalculateDies();

            Assert.That(someoneDied, Is.True);
            Assert.That(gameState.LivingAnthropoiCount, Is.EqualTo(19));
            Assert.That(gameState.StoredFood, Is.EqualTo(-1));
        }

        [Test]
        [Timeout(5000)]
        public void Tick_AfterHumanDeaths_DoesNotLeaveMoreWorkersThanSurvivors()
        {
            FoodVaultState foodVault = new FoodVaultState("food-vault", true);
            List<BuildingState> buildings = new List<BuildingState>
            {
                new DepotState("food-depot", true, StoreType.Food),
                new DepotState("materials-depot", true, StoreType.Materials),
                foodVault
            };
            Simulation simulation = new Simulation(null, buildings);

            for (int i = 0; i < 11; i++)
            {
                Assert.That(foodVault.AssignAnthroposWorker(), Is.True);
            }

            simulation.GameState.WorkingAnthropoiCount = 11;
            simulation.GameState.StoredFood = -1100;

            for (int i = 0; i < 25; i++)
            {
                simulation.Tick();
            }

            Assert.That(
                simulation.GameState.WorkingAnthropoiCount,
                Is.LessThanOrEqualTo(simulation.GameState.LivingAnthropoiCount),
                "사망한 인간이 건물 노동자로 남아 있습니다.");
        }
    }
}
