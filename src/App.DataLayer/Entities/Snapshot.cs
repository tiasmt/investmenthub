using Microsoft.EntityFrameworkCore;

namespace App.DataLayer.Entities
{
    [Keyless]
    public class Snapshot
    {
        
        public long Version { get; set; } = 0;
        public Portfolio State;
        public Snapshot()
        {
            State = new Portfolio();
        }
    }
}