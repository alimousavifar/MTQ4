using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace MTQ4
{
    class Program
    {
        public static int rollerCoasterCapacity = 4;
        public static int passengersInLine = 8;
        public static SemaphoreSlim entrySem = new SemaphoreSlim(0,rollerCoasterCapacity);
        public static SemaphoreSlim fullSem = new SemaphoreSlim(0,rollerCoasterCapacity);
        public static SemaphoreSlim exitSem = new SemaphoreSlim(0,rollerCoasterCapacity);
        public static SemaphoreSlim emptySem = new SemaphoreSlim(0,rollerCoasterCapacity);

        static void Main(string[] args)
        {
            //Console.WriteLine("*****Main Begins****");
            //Task launcherTask = Task.Run(() => Launcher());
            //launcherTask.Wait();

            Task rollerCoasterTask = Task.Run(() => RollerCoster());

            List<Task> passengerTasks = new List<Task>();
            for (int i = 0; i < passengersInLine; i++)
            {
                Task passengerTask = Task.Run(() => Passenger());
                passengerTasks.Add(passengerTask);
            }
            rollerCoasterTask.Wait();
            Task.WaitAll(passengerTasks.ToArray());

            Console.WriteLine("Sleeping 5 s in Launcher");
            Thread.Sleep(5000);
        }


        private static void Passenger()
        {
            // Condition 1/Condition 4: wait to get on coaster until it is boarding and there is still room on coaster
            entrySem.Wait();
            Thread.Sleep(1500);
            Console.WriteLine("entrySem wait in Passenger");

            fullSem.Release();
            Thread.Sleep(1500);
            Console.WriteLine("Fullsem release in Passenger");
            // Condition 3: wait until ride is over
            exitSem.Wait();
            Thread.Sleep(1500);
            Console.WriteLine("exitSem wait in Passenger");

            // Condition 4: notify coaster that there is one less passenger
            emptySem.Release();
            Thread.Sleep(1500);
            Console.WriteLine("emptySem release in Passenger");
        }

        static void RollerCoster (){

            while (true)
            {
                //condition 1 semaphore boarding
                for (int i = 0; i < rollerCoasterCapacity; i++)
                {
                    entrySem.Release();
                    Console.WriteLine("entrySem release in RollerCoster {0}", i);
                }
                Console.WriteLine("All Entered");
                Thread.Sleep(1500);

                //Condition 2
                for (int i = 0; i < rollerCoasterCapacity; i++)
                {
                    fullSem.Wait();
                    Console.WriteLine("fullSem wait in RollerCoster {0}", i);
                }
                Console.WriteLine("All Boarded");
                Thread.Sleep(1500);

                //condition 3
                for (int i = 0; i < rollerCoasterCapacity; i++)
                {
                   exitSem.Release();
                   Console.WriteLine("exitSem release in RollerCoster {0}", i);
                }


                Console.WriteLine("Ride Begins");
                Thread.Sleep(1500);
                //condition 4
                for (int i = 0; i < rollerCoasterCapacity; i++)
                {
                   emptySem.Wait();
                   Console.WriteLine("emptySem wait in RollerCoster {0}",i);
                }

                Console.WriteLine("All Exited");
                Thread.Sleep(1500);
            }
        }
    }
}
