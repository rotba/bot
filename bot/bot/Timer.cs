using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bot
{
    class Timer
    {
        private static System.Timers.Timer aTimer;
        int sec = 1000;
        int num_of_sec = 10;
        Thread broadcast_thread;

        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("hadas");
        }

        private static void SetTimer()
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(200);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            //aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        //static void Main(string[] args)
        //{
        //    SetTimer();

        //    Console.WriteLine("\nPress the Enter key to exit the application...\n");
        //    Console.WriteLine("The application started at {0:HH:mm:ss.fff}", DateTime.Now);
        //    Console.ReadLine();
        //    aTimer.Stop();
        //    aTimer.Dispose();

        //    Console.WriteLine("Terminating the application...");
        //}
    }
}
