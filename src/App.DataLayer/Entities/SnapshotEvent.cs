using System;
using Microsoft.EntityFrameworkCore;

namespace App.DataLayer.Entities
{
    
    public class SnapshotEvent
    {
        public long Id {get;set;}
        public long Version{get;set;}
        public DateTime Timestamp {get;set;}
        public string EventType {get;set;}
        public string User {get;set;}
        public string Data {get;set;}
    }
}