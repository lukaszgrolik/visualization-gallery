using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    class RandomWalk
    {
        private readonly List<float> changes = new List<float>();
        private readonly List<float> values = new List<float>();

        private float lastValue = 0; public float LastValue => lastValue;

        private System.Random prng;
        // private readonly float luck;
        // private readonly float skill;

        // public RandomWalk(System.Random prng, float luck, float skill)
        public RandomWalk(System.Random prng)
        {
            this.prng = prng;
            // this.luck = luck;
            // this.skill = skill;
        }

        public void Init()
        {
            values.Add(lastValue);
        }

        public float Tick()
        {
            // var baseDifficulty = .5f;
            // var luck = .5f;
            // var skill = .5f;
            // var chance = luck * skill;
            var chance = .5f;
            // var change = prng.NextDouble() > .5f ? -1 : 1;
            var change = prng.NextDouble() > chance ? -1 : 1;
            // var change = Mathf.Lerp(-1, 1, (float)prng.NextDouble());
            lastValue += change;

            changes.Add(change);
            values.Add(lastValue);

            return lastValue;
        }
    }
}