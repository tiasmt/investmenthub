namespace App.DataLayer.Entities
{

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