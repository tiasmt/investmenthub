using System;

namespace App.DataLayer.Entities
{
    public class Event
    {
        public long Id {get;set;}
        public DateTime Timestamp {get;set;}
        public string EventType {get;set;}
        public string User {get;set;}
        public string Data {get;set;}
    }
}