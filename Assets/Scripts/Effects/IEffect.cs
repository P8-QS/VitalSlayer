using NUnit.Framework.Internal.Commands;
using UnityEngine;

namespace Effects
{
    public interface IEffect
    {
        public string Name { get; }
        public int Level { get; set; }
        public Sprite Icon { get; }
        public string Text();
        public string Description();
        public void Apply();
    }
}