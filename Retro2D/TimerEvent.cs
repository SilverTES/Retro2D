using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retro2D
{
    public class TimerEvent
    {
        bool[] _active;
        float[] _timers;
        float[] _tics;
        bool[] _on;

        float[] _factorTimes;

        public TimerEvent(int nbTimer)
        {
            _active = new bool[nbTimer];
            _timers = new float[nbTimer];
            _tics = new float[nbTimer];
            _on = new bool[nbTimer];

            _factorTimes = new float[nbTimer];

            for (int i = 0; i < nbTimer; i++)
                _factorTimes[i] = 1f;
        }

        public static float TimeToFrame(float hours, float minutes, float seconds)
        {
            return hours * 216000 + minutes * 3600 + seconds * 60;
        }

        public static float Time(float hours, float minutes, float seconds)
        {
            return 1f / (float)TimeToFrame(hours, minutes, seconds);
        }

        public void SetTimer(int timer, float tic)
        {
            _tics[timer] = tic;
        }
        public void SetTimeFactor(int timer, float timeFactor = 1f)
        {
            _factorTimes[timer] = timeFactor;
        }
        public float GetTimer(int timer)
        {
            return _timers[timer];
        }
        public void Start(int timer)
        {
            _active[timer] = true;
            _timers[timer] = 1f;
        }
        public void Pause(int timer)
        {
            _active[timer] = false;
        }
        public void Resume(int timer)
        {
            _active[timer] = true;
        }
        public void Stop(int timer)
        {
            _active[timer] = false;
            _timers[timer] = 1f;
        }
        public bool On(int timer)
        {
            return _on[timer];
        }

        public void Update()
        {
            for (int i = 0; i < _timers.Length; i++)
            {
                if (_active[i])
                    _timers[i] -= _tics[i] * _factorTimes[i];

                if (_timers[i] <= 0f)
                {
                    _on[i] = true;
                    _timers[i] = 1f;
                }
                else
                {
                    _on[i] = false;
                }

            }
        }

    }
}
