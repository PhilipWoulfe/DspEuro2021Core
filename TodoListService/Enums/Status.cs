using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoListService.Enums
{
    public enum Status
    {
        SCHEDULED,
        LIVE,
        IN_PLAY,
        PAUSED,
        FINISHED,
        POSTPONED,
        SUSPENDED,
        CANCELED
    }
}
