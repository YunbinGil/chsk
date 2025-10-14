using System;
using UnityEngine;

namespace chsk.Core.Data
{
    public enum TimeUnit { Seconds, Minutes, Hours }

    [Serializable]
    public struct Duration
    {
        [Min(0)] public int value;
        public TimeUnit unit;

        public int ToSeconds() => unit switch
        {
            TimeUnit.Seconds => value,
            TimeUnit.Minutes => value * 60,
            TimeUnit.Hours   => value * 3600,
            _ => value
        };

        public static Duration FromSeconds(int s) => new Duration { value = Mathf.Max(0, s), unit = TimeUnit.Seconds };
        public static Duration FromMinutes(int m) => new Duration { value = Mathf.Max(0, m), unit = TimeUnit.Minutes };
        public static Duration FromHours(int h) => new Duration { value = Mathf.Max(0, h), unit = TimeUnit.Hours };
        
        public override string ToString()
        {
            return unit switch
            {
                TimeUnit.Seconds => $"{value}초",
                TimeUnit.Minutes => $"{value}분",
                TimeUnit.Hours   => $"{value}시간",
                _ => $"{value}"
            };
        }
    }
}
