using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessService;
using DTO;
using Desafio.FCL.Enum;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ExecuteTest();
        }

        static void ExecuteTest()
        {
            using (TraceRoute tr = new TraceRoute())
            {
                var address = new List<Address>();

                address.Add(new Address
                {
                    City = "Sao Paulo",
                    StateProvince = "SP",
                    StreetName = "Rua da Mooca",
                    HouseNumber = 3000
                });

                address.Add(new Address
                {
                    City = "Sao Paulo",
                    StateProvince = "SP",
                    StreetName = "Avenida Paulista",
                    HouseNumber = 1439
                });

                var totals = tr.GetRouteTotals(address, RouteType.AvoidingTrafficJamRoute);

                if (totals != null)
                {
                    Console.WriteLine("Cálculo da rota:");
                    Console.WriteLine("Tempo total: " + totals.TotalTime);
                    Console.WriteLine("Distância total: " + totals.TotalDistance + "km");
                    Console.WriteLine("Total de combustível: R$" + totals.TotalFuelCost);
                    Console.WriteLine("Custo total: R$" + totals.TotalCost);
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("Nenhuma rota encontrada");
                    Console.ReadLine();
                }
            }
        }
    }
}
