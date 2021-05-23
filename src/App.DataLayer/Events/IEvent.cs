namespace App.DataLayer.Events
{
    public interface IEvent
    {
        string User {get;}
        string EventType { get; }
    }
}