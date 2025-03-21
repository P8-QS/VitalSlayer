using System;

namespace UI
{
    public class MetricsStringGenerator
    {
        public static string Steps(int stepsCount)
        {
            string formattedSteps = stepsCount.ToString("N0"); // Adds commas for readability
            int mapSize = stepsCount switch
            {
                < 4000 => 1,
                < 8000 => 2,
                _ => 3
            };

            return $"You have taken {formattedSteps} steps. This gives you {EffectStringGenerator.Map(mapSize)}.";
        }

        public static string StepsDescription(int stepsCount)
        {
            string formattedSteps = stepsCount.ToString("N0");
            return $"You have taken {formattedSteps} steps yesterday, contributing to your movement activity.";
        }

        public static string Sleep(TimeSpan duration)
        {
            
            int hours = duration.Hours;
            int minutes = duration.Minutes;
            
            int hallucinationLevel = hours switch
            {
                < 3 => 0,
                < 5 => 1,
                _ => 2
            };

            string effect = hallucinationLevel > 0 ? EffectStringGenerator.Hallucination(hallucinationLevel) : EffectStringGenerator.AttackSpeed();
            return $"You have slept {hours} hours and {minutes} minutes. This gives you {effect}.";
        }

        public static string SleepDescription(TimeSpan duration)
        {
            int hours = duration.Hours;
            int minutes = duration.Minutes;
            
            return $"You slept for {hours} hours and {minutes} minutes yesterday. Proper rest is essential for recovery and focus.";
        }

        public static string ScreenTime(int hours, int minutes)
        {
            int totalMinutes = (hours * 60) + minutes;
            int fogLevel = totalMinutes switch
            {
                < 120 => 0,  // Less than 2 hours (no fog)
                < 240 => 1,  // 2 - 4 hours
                _ => 2       // More than 4 hours
            };

            string effect = fogLevel > 0 ? EffectStringGenerator.Fog(fogLevel) : EffectStringGenerator.Scouting();
            return $"You have spent {hours} hours and {minutes} minutes on your phone. This gives you {effect}.";
        }

        public static string ScreenTimeDescription(int hours, int minutes)
        {
            return $"You spent {hours} hours and {minutes} minutes on your phone yesterday. Consider balancing screen time for better focus.";
        }
    }
}
