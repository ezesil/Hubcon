﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubcon.Core.Models.Interfaces
{
    public interface IOnStreamReceived
    {
        public Delegate? GetCurrentEvent();
    }
}
