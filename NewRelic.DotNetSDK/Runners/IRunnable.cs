﻿using System.Collections.Generic;

namespace NewRelic.DotNetSDK.Runners
{
    public interface IRunnable
    {
        //// ----------------------------------------------------------------------------------------------------------
		 
        void Run(object arg);

        //// ---------------------------------------------------------------------------------------------------------- 
        
        LinkedList<Agent> Agents { get; set; }

        //// ---------------------------------------------------------------------------------------------------------- 
    }
}
