﻿using System;

namespace ZWaveCore.Core
{
    public class Deferral
    {
        private readonly Action _onComplete;

        public Deferral(Action onComplete)
        {
            _onComplete = onComplete;
        }

        public void SetComplete()
        {
            _onComplete();
        }
    }
}
