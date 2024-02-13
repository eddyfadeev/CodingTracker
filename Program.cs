using CodingTracker.enums;
using CodingTracker.models;
using CodingTracker.services;
using CodingTracker.views;

namespace CodingTracker;

class Program
{
    static void Main(string[] args)
    {
        CodingSession session = new CodingSession(DateTime.Now.AddHours(-12), DateTime.Now);

        Console.WriteLine(session.Duration.Hours);
    }
}