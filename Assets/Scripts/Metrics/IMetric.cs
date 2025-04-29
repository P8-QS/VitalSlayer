using System.Collections.Generic;
using System.Linq;
using Effects;
using UnityEngine;

namespace Metrics
{
    public interface IMetric
    {
        public string Name { get; }
        public List<IEffect> Effects { get; }
        public Sprite Icon { get; }
        public string Text();
        public string Description();

        public string EffectsToString()
        {
            var texts = Effects.Select(e => e.Text()).ToList();

            if (texts.Count <= 1) return string.Join(", ", texts);

            // Join with comma and "and" for the last element
            var last = texts.Last();
            texts.RemoveAt(texts.Count - 1);
            return $"{string.Join(", ", texts)} and {last}";
        }
    }
}